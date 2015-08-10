/*
 * 작성자: CHILD	
 * 기능: 통신에 필요한 최소한의 기능들을 인터페이스로 구현
 * 날짜: 2015-03-25
 */
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using NLog;

namespace DY.NET
{
    /// <summary>
    /// 추상 소켓 클래스
    /// </summary>
    public abstract class ASocketCover : ISocketCover
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public const int BUF_SIZE = 4096;
        protected byte[] Buf = new byte[BUF_SIZE];
        protected int BufIdx;
        //요청 프로토콜 임시 저장 변수
        protected IProtocol ReqeustProtocolPointer;
        //스레드 세이프 프로토콜 전송 대기 큐
        protected ConcurrentQueue<IProtocol> ProtocolStandByQueue;
        //요청 프로토콜 대기 여부
        protected volatile bool ReqPossible;
        //소켓 스트림
        protected Stream SocketStream;
        //타임아웃
        public int WriteTimeout { get; set; }
        public int ReadTimeout { get; set; }
        protected Timer TimeoutTimer;

        public int Tag { get; set; }
        public string Description { get; set; }
        public object UserData { get; set; }

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
            TimeoutTimer = new Timer();
            TimeoutTimer.Elapsed += OnTimeoutElapsed;
        }

        public abstract bool Connect();
        public abstract void Close();
        public abstract void Send(IProtocol protocol);
        public abstract bool IsConnected();
        public abstract Task<long> PingAsync();

        public virtual void Dispose()
        {
            ProtocolStandByQueue = null;
            SendedProtocolSuccessfully = null;
            ReceivedProtocolSuccessfully = null;
            ReqeustProtocolPointer = null;
            ConnectionStatusChanged = null;
            SocketStream = null;

            TimeoutTimer.Stop();
            TimeoutTimer.Dispose();
            TimeoutTimer = null;

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

        /// <summary>
        /// 시리얼 포트가 Write를 할 때 타이머를 작동시켜, OnDataRecieve가 호출 되기전 타임아웃이 된다면 타이머 호출
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnTimeoutElapsed(object sender, ElapsedEventArgs e)
        {
            var timer = sender as Timer;
            timer.Stop();
            LOG.Debug(Description + " Timeout: " + timer.Interval);
            PrepareTransmission();
            SendNextProtocol();
        }

        protected void PrepareTransmission()
        {
            ReqeustProtocolPointer = null;
            BufIdx = 0;
            ReqPossible = true;
        }

        protected void SendNextProtocol()
        {
            IProtocol temp = null;
            if (ProtocolStandByQueue.TryDequeue(out temp))
                Send(temp);
        }
    }
}