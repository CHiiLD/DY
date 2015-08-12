/*
 * 작성자: CHILD	
 * 기능: 통신에 필요한 최소한의 기능들을 인터페이스로 구현
 * 날짜: 2015-03-25
 */
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
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
        //BUFFER
        public const int STREAM_BUFFER_SIZE = 4096;
        protected byte[] StreamBuffer = new byte[STREAM_BUFFER_SIZE];
        protected int StreamBufferIndex;
        private Stream m_BaseStream;
        private IAsyncResult m_ReadIAsyncResult;
        //소켓 스트림
        protected Stream BaseStream
        {
            get
            {
                return m_BaseStream;
            }
            set
            {
                m_BaseStream = value;
                m_ReadIAsyncResult = m_BaseStream.BeginRead(StreamBuffer, 0, STREAM_BUFFER_SIZE, OnRead, null);
            }
        }

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

        /****************************************************************************************/

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

        public abstract bool Connect();
        public abstract void Close();
        public abstract bool IsConnected();
        public abstract Task<long> PingAsync();

        protected abstract bool DoReadAgain(AProtocol request);
        protected abstract AProtocol ReportResponseProtocol(AProtocol request, byte[] recv_data);
        /// <summary>
        /// 비동기 통신으로 PLC와 통신하여 요청 메세지를 보내고 응답 메세지를 받는다
        /// </summary>
        /// <param name="request">XGTCnetProtocol 요청 프로토콜</param>
        /// <returns>Delivery</returns>
        public async Task<Delivery> PostAsync(IProtocol request)
        {
            Delivery delivery = new Delivery();
            AProtocol r = request as AProtocol;
            int read_size = StreamBufferIndex = 0;
            if (!IsConnected())
            {
                Close();
                LOG.Trace(Description + " 재연결시도 ...");
                if (!Connect())
                {
                    delivery.Error = DeliveryError.DISCONNECT;
                    return delivery.Packing();
                }
            }
            do
            {
                BaseStream.EndRead(m_ReadIAsyncResult);
                int write_ret = await WriteAsync(r.ASCIIProtocol, 0, r.ASCIIProtocol.Length);
                if (write_ret < 0)
                {
                    delivery.Error = (DeliveryError)write_ret;
                    break;
                }
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
                if (delivery.Error != DeliveryError.SUCCESS)
                    break;
                byte[] recv_data = new byte[StreamBufferIndex];
                Buffer.BlockCopy(StreamBuffer, 0, recv_data, 0, recv_data.Length);
                delivery.Package = ReportResponseProtocol(r, recv_data);
                m_ReadIAsyncResult = m_BaseStream.BeginRead(StreamBuffer, 0, STREAM_BUFFER_SIZE, OnRead, null);
            } while (false);
            m_ReadIAsyncResult = m_BaseStream.BeginRead(StreamBuffer, 0, STREAM_BUFFER_SIZE, OnRead, null);
            return delivery.Packing();
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

        private void OnRead(IAsyncResult ar)
        {
            try
            {
                BaseStream.EndRead(ar);
            }
            catch (IOException io_exception)
            {
                LOG.Trace(Description + " 서버측에서 연결을 해제");
                Close();
                return;
            }
            m_ReadIAsyncResult = m_BaseStream.BeginRead(StreamBuffer, 0, STREAM_BUFFER_SIZE, OnRead, null);
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