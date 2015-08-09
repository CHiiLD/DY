﻿/*
 * 작성자: CHILD	
 * 기능: 통신에 필요한 최소한의 기능들을 인터페이스로 구현
 * 날짜: 2015-03-25
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace DY.NET
{
    /// <summary>
    /// 추상 소켓 클래스
    /// </summary>
    public abstract class ASocketCover : ISocketCover
    {
        public const int BUF_SIZE = 4096;
        protected IProtocol ReqeustProtocolPointer;

        /// 스레드 세이프 프로토콜 전송 대기 큐
        /// </summary>
        protected ConcurrentQueue<IProtocol> ProtocolStandByQueue;
        protected volatile bool ReqPossible;
        protected byte[] Buf = new byte[BUF_SIZE];
        protected int BufIdx;
        
        /// <summary>
        /// 소켓 스트림
        /// </summary>
        protected Stream SocketStream;

        private int m_WriteTimeout;
        private int m_ReadTimeout;

        public int WriteTimeout
        {
            get
            {
                return m_WriteTimeout;
            }
            set
            {
                m_WriteTimeout = value;
                if(SocketStream != null)
                    SocketStream.WriteTimeout = value;
            }
        }
        public int ReadTimeout
        {
            get
            {
                return m_ReadTimeout;
            }
            set
            {
                m_ReadTimeout = value;
                if (SocketStream != null)
                    SocketStream.ReadTimeout = value;
            }
        }

        public int Tag
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public object UserData
        {
            get;
            set;
        }

        /// <summary>
        /// Connect, Close 이벤트 발생 시 호출
        /// </summary>
        public EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged { get; set; }

        /// <summary>
        /// 데이터를 성공적으로 전송하였을 때 호출되는 이벤트
        /// </summary>
        public EventHandler<ProtocolReceivedEventArgs> SendedProtocolSuccessfully { get; set; }

        /// <summary>
        /// 데이터를 성공적으로 전송받았을 때 호출되는 이벤트
        /// </summary>
        public EventHandler<ProtocolReceivedEventArgs> ReceivedProtocolSuccessfully { get; set; }

        protected ASocketCover()
        {
            ProtocolStandByQueue = new ConcurrentQueue<IProtocol>();
            ReqPossible = true;
        }

        public abstract bool Connect();
        public abstract void Close();
        public abstract void Send(IProtocol protocol);
        public abstract bool IsConnected();

        public virtual void Dispose()
        {
            ProtocolStandByQueue = null;
            SendedProtocolSuccessfully = null;
            ReceivedProtocolSuccessfully = null;
            ReqeustProtocolPointer = null;
            ConnectionStatusChanged = null;
            SocketStream = null;

            Buf = null;
            BufIdx = 0;
        }

        public void SendedProtocolSuccessfullyEvent(IProtocol iProtocol)
        {
            if (SendedProtocolSuccessfully != null)
            {
                var cold_pt = System.Threading.Volatile.Read(ref iProtocol);
                SendedProtocolSuccessfully(this, new ProtocolReceivedEventArgs(cold_pt));
            }
        }

        public void ReceivedProtocolSuccessfullyEvent(IProtocol iProtocol)
        {
            if (ReceivedProtocolSuccessfully != null)
            {
                var cold_pt = System.Threading.Volatile.Read(ref iProtocol);
                ReceivedProtocolSuccessfully(this, new ProtocolReceivedEventArgs(cold_pt));
            }
        }

        public void ConnectionStatusChangedEvent(bool isConnected)
        {
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs(isConnected));
        }
    }
}