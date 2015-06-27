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

        public abstract bool Connect();
        public abstract bool Close();
        public abstract void Send(IProtocol protocolFrame);
        public abstract bool IsOpen();

        public virtual void Dispose()
        {
            ProtocolStandByQueue = null;
            OnSendedSuccessfully = null;
            OnReceivedSuccessfully = null;
        }

        /// <summary>
        /// 데이터를 성공적으로 전송하였을 때 호출되는 이벤트
        /// </summary>
        public EventHandler<DataReceivedEventArgs> OnSendedSuccessfully;

        /// <summary>
        /// 데이터를 성공적으로 전송받았을 때 호출되는 이벤트
        /// </summary>
        public EventHandler<DataReceivedEventArgs> OnReceivedSuccessfully;
    }
}