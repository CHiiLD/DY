/*
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

        protected readonly byte[] EMPTY_BYTE = new byte[1];
        //BUFFER
        public const int STREAM_BUFFER_SIZE = 4096;
        protected byte[] StreamBuffer = new byte[STREAM_BUFFER_SIZE];
        protected int StreamBufferIndex;
        //소켓 스트림
        protected Stream BaseStream { get; set; }
        //타임아웃
        public int WriteTimeout { get; set; }
        public int ReadTimeout { get; set; }
        //ITAG
        public int Tag { get; set; }
        public string Description { get; set; }
        public object UserData { get; set; }
        
        /// <summary>
        /// Connect, Close 이벤트 발생 시 호출
        /// </summary>
        public EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged { get; set; }

        public abstract bool Connect();
        public abstract void Close();
        public abstract bool IsConnected();
        public abstract Task<long> PingAsync();
        protected abstract bool DoReadAgain(AProtocol request);
        protected abstract AProtocol CreateResponseProtocol(AProtocol request, byte[] recv_data);

        /// <summary>
        /// 생성자 
        /// </summary>
        protected ASocketCover()
        {
            WriteTimeout = -1;
            ReadTimeout = -1;
        }

        public async Task<int> WriteAsync(byte[] buffer, int offset, int count)
        {
            var t_src = new CancellationTokenSource(WriteTimeout);
            await BaseStream.WriteAsync(buffer, offset, count, t_src.Token);
            if (t_src.IsCancellationRequested)
                return (int)DeliveryError.WRITE_TIMEOUT;
            else
                return (int)DeliveryError.SUCCESS;
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            var t_src = new CancellationTokenSource(ReadTimeout);
            int size = await BaseStream.ReadAsync(buffer, offset, count, t_src.Token);
            if (t_src.IsCancellationRequested)
                return (int)DeliveryError.READ_TIMEOUT;
            else
                return size;
        }

        /// <summary>
        /// 비동기 통신으로 PLC와 통신하여 요청 메세지를 보내고 응답 메세지를 받는다
        /// </summary>
        /// <param name="request">XGTCnetProtocol 요청 프로토콜</param>
        /// <returns>Delivery</returns>
        public async Task<Delivery> PostAsync(IProtocol request)
        {
            Delivery delivery = new Delivery();
            // AsyncLock can be locked asynchronously
            using (await m_AsyncLock.LockAsync())
            {
                AProtocol r = request as AProtocol;
                StreamBufferIndex = 0;
                do
                {
                    if (!IsConnected())
                    {
                        if (!TryReconnect(delivery))
                            break;
                    }
                    int write_ret = await WriteAsync(r.ASCIIProtocol, 0, r.ASCIIProtocol.Length);
                    if (write_ret < 0)
                    {
                        delivery.Error = (DeliveryError)write_ret;
                        break;
                    }
                    await WaitResponsePostAsync(delivery, r);
                } while (false);
            }
            return delivery.Packing();
        }

        private bool TryReconnect(Delivery delivery)
        {
            Close();
            LOG.Trace(Description + " 재연결시도 중...");
            if (!Connect())
            {
                LOG.Trace(Description + " 재연결시도 실패");
                delivery.Error = DeliveryError.DISCONNECT;
                return false;
            }
            LOG.Trace(Description + " 재연결시도 성공");
            return true;
        }

        private async Task WaitResponsePostAsync(Delivery delivery, AProtocol r)
        {
            int read_size = 0;
            do
            {
                read_size = await ReadAsync(StreamBuffer, StreamBufferIndex, STREAM_BUFFER_SIZE - StreamBufferIndex);
                if (read_size < 0)
                {
                    delivery.Error = (DeliveryError)read_size;
                    break;
                }
                StreamBufferIndex += read_size;
            } while (DoReadAgain(r));
            if (delivery.Error == DeliveryError.SUCCESS)
            {
                byte[] recv_data = new byte[StreamBufferIndex];
                Buffer.BlockCopy(StreamBuffer, 0, recv_data, 0, recv_data.Length);
                delivery.Package = CreateResponseProtocol(r, recv_data);
            }
        }
        /// <summary>
        /// 메모리 해제
        /// </summary>
        public virtual void Dispose()
        {
            ConnectionStatusChanged = null;
            BaseStream = null;
            StreamBuffer = null;
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