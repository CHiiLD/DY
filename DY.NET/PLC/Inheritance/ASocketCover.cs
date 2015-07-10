/*
 * 작성자: CHILD	
 * 기능: 통신에 필요한 최소한의 기능들을 인터페이스로 구현
 * 날짜: 2015-03-25
 */
using System;
using System.Collections.Concurrent;

namespace DY.NET
{
    /// <summary>
    /// 추상 소켓 클래스
    /// </summary>
    public abstract class ASocketCover : ISocketCover
    {
        protected ASocketCover()
        {

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
        /// 스레드 세이프 프로토콜 전송 대기 큐
        /// </summary>
        protected ConcurrentQueue<IProtocol> ProtocolStandByQueue = new ConcurrentQueue<IProtocol>();
        protected IProtocol ReqeustProtocol;

        public abstract bool Connect();
        public abstract void Close();
        public abstract void Send(IProtocol protocol);
        public abstract bool IsConnected();

        public virtual void Dispose()
        {
            ProtocolStandByQueue = null;
            SendedProtocolSuccessfully = null;
            ReceivedProtocolSuccessfully = null;
        }

        public const int BUFFER_SIZE = 4096;
        protected byte[] Buf = new byte[BUFFER_SIZE];
        protected int BufIdx;
        protected volatile bool IsWait = false;

        /// <summary>
        /// 데이터를 성공적으로 전송하였을 때 호출되는 이벤트
        /// </summary>
        public EventHandler<DataReceivedEventArgs> SendedProtocolSuccessfully { get; set; }

        /// <summary>
        /// 데이터를 성공적으로 전송받았을 때 호출되는 이벤트
        /// </summary>
        public EventHandler<DataReceivedEventArgs> ReceivedProtocolSuccessfully { get; set; }

        public void SendedProtocolSuccessfullyEvent(IProtocol iProtocol)
        {
            if (SendedProtocolSuccessfully != null)
            {
                var cold_pt = System.Threading.Volatile.Read(ref iProtocol);
                SendedProtocolSuccessfully(this, new DataReceivedEventArgs(cold_pt));
            }
        }

        public void ReceivedProtocolSuccessfullyEvent(IProtocol iProtocol)
        {
            if (ReceivedProtocolSuccessfully != null)
            {
                var cold_pt = System.Threading.Volatile.Read(ref iProtocol);
                ReceivedProtocolSuccessfully(this, new DataReceivedEventArgs(cold_pt));
            }
        }
    }
}