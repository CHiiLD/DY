﻿/*
 * 작성자: CHILD	
 * 기능: 통신에 필요한 최소한의 기능들을 인터페이스로 구현
 * 날짜: 2015-03-25
 */

using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Nito.AsyncEx;
using NLog;

namespace DY.NET
{
    /// <summary>
    /// 추상 소켓 클래스
    /// </summary>
    public abstract class ASocketCover : ISocketCover
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private readonly AsyncLock m_AsyncLock = new AsyncLock();

        private int m_ReadTimeout;
        private int m_ReadTimeoutMaximum;
        private int m_ReadTimeoutMinimum;

        private int m_WriteTimeout;
        private int m_WriteTimeoutMaximum;
        private int m_WriteTimeoutMinimum;

        protected readonly byte[] EMPTY_BYTE = new byte[1];
        //BUFFER
        public const int BUFFER_SIZE = 4096;
        protected byte[] BaseBuffer = new byte[BUFFER_SIZE];
        //protected int BufferIndex;
        //소켓 스트림
        protected Stream BaseStream { get; set; }
        
        //ITAG
        public int Tag { get; set; }
        public string Description { get; set; }
        public object UserData { get; set; }
        /// <summary>
        /// Connect, Close 이벤트 발생 시 호출
        /// </summary>
        public EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged { get; set; }

        public int ReadTimeoutMaximum
        {
            get
            {
                return m_ReadTimeoutMaximum;
            }
            set
            {
                if (0 <= value && value >= ReadTimeoutMinimum)
                    m_ReadTimeoutMaximum = value;
            }
        }

        public int ReadTimeoutMinimum
        {
            get
            {
                return m_ReadTimeoutMinimum;
            }
            set
            {
                if (0 <= value && value <= ReadTimeoutMaximum)
                    m_ReadTimeoutMinimum = value;
            }
        }

        public int WriteTimeoutMaximum
        {
            get
            {
                return m_WriteTimeoutMaximum;
            }
            set
            {
                if (0 <= value && ReadTimeoutMinimum <= value)
                    m_WriteTimeoutMaximum = value;
            }
        }

        public int WriteTimeoutMinimum
        {
            get
            {
                return m_WriteTimeoutMinimum;
            }
            set
            {
                if (0 <= value && value <= WriteTimeoutMaximum)
                    m_WriteTimeoutMinimum = value;
            }
        }

        public int WriteTimeout
        {
            get
            {
                return m_WriteTimeout;
            }
            set
            {
                if (WriteTimeoutMinimum <= value && value <= WriteTimeoutMaximum)
                    m_WriteTimeout = value;
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
                if (ReadTimeoutMinimum <= value && value <= ReadTimeoutMaximum)
                    m_ReadTimeout = value;
            }
        }

        public abstract bool Connect();
        public abstract void Close();
        public abstract bool IsConnected();
        protected abstract bool DoReadAgain(AProtocol request);
        protected abstract AProtocol CreateResponseProtocol(AProtocol request, byte[] recv_data);

        /// <summary>
        /// 생성자 
        /// </summary>
        protected ASocketCover()
        {
            WriteTimeoutMaximum = 500;
            ReadTimeoutMaximum = 500;

            WriteTimeoutMinimum = 0;
            ReadTimeoutMinimum = 0;

            WriteTimeout = 250;
            ReadTimeout = 250;
        }

        /// <summary>
        /// 비동기 통신으로 PLC와 통신하여 요청 메세지를 보내고 응답 메세지를 받는다
        /// </summary>
        /// <param name="request">XGTCnetProtocol 요청 프로토콜</param>
        /// <returns>Delivery</returns>
        public async Task<Delivery> PostAsync(IProtocol request)
        {
            Delivery delivery = new Delivery();
            using (await m_AsyncLock.LockAsync())
            {
                AProtocol p = request as AProtocol;
                if (!IsConnected()) //CONNECT
                {
                    Close();
                    if (!Connect())
                        return delivery.Packing(DeliveryError.DISCONNECT);
                }
                CancellationTokenSource wcts = new CancellationTokenSource(WriteTimeout);
                await BaseStream.WriteAsync(p.ASCIIProtocol, 0, p.ASCIIProtocol.Length, wcts.Token);
                if (wcts.IsCancellationRequested)
                    return delivery.Packing(DeliveryError.WRITE_TIMEOUT);

                int idx = 0;
                CancellationTokenSource rcts = new CancellationTokenSource(ReadTimeout);
                do
                {
                    int read_size = await BaseStream.ReadAsync(BaseBuffer, idx, BUFFER_SIZE - idx, rcts.Token);
                    if (rcts.IsCancellationRequested)
                        return delivery.Packing(DeliveryError.READ_TIMEOUT);
                    if (read_size <= 0)
                        System.Diagnostics.Debug.Assert(false);
                    idx += read_size;
                } while (DoReadAgain(p));
                byte[] recv_data = new byte[idx];
                Buffer.BlockCopy(BaseBuffer, 0, recv_data, 0, recv_data.Length);
                delivery.Package = CreateResponseProtocol(p, recv_data);
            }
            return delivery.Packing(DeliveryError.SUCCESS);
        }

        /// <summary>
        /// 메모리 해제
        /// </summary>
        public virtual void Dispose()
        {
            ConnectionStatusChanged = null;
            BaseStream = null;
            BaseBuffer = null;
            UserData = null;
        }

        /// <summary>
        /// ConnectionStatusChanged 이벤트 호출
        /// </summary>
        /// <param name="isConnected">연결 상태</param>
        public void ConnectionStatusChangedEvent(bool isConnected)
        {
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs(isConnected));
        }
    }
}