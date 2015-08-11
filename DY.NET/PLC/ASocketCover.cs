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
        //요청 프로토콜 임시 저장 변수
        protected IProtocol ReqeustProtocolPointer;
        //소켓 스트림
        protected Stream BaseStream;
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
            int byte_size = await BaseStream.ReadAsync(buffer, offset, count, t_src.Token);
            if (t_src.IsCancellationRequested)
                return (int)DeliveryError.READ_TIMEOUT;
            else
                return byte_size;
        }

        public abstract bool Connect();
        public abstract void Close();
        public abstract bool IsConnected();
        public abstract Task<long> PingAsync();

        protected abstract bool DoReadAgain();
        protected abstract AProtocol ReportResponseProtocol(AProtocol request, byte[] recv_data);

        /// <summary>
        /// 비동기 통신으로 PLC와 통신하여 요청 메세지를 보내고 응답 메세지를 받는다
        /// </summary>
        /// <param name="request">XGTCnetProtocol 요청 프로토콜</param>
        /// <returns>Delivery</returns>
        public async Task<Delivery> PostAsync(IProtocol request)
        {
            Delivery delivery = new Delivery();

            AProtocol reqt = request as AProtocol;
            if (reqt == null)
                throw new ArgumentException("Protocol not match XGTCnetProtocol type");

            if (!IsConnected())
            {
                if (!Connect())
                {
                    delivery.Error = DeliveryError.DISCONNECT;
                    return delivery.Packing();
                }
            }
            // WRITE WORK //
            CancellationTokenSource t_src = new CancellationTokenSource(WriteTimeout);
            int write_ret = await WriteAsync(reqt.ASCIIProtocol, 0, reqt.ASCIIProtocol.Length);
            if (write_ret < 0)
            {
                delivery.Error = (DeliveryError)write_ret;
                return delivery.Packing();
            }

            // READ WORK //
            StreamBufferIndex = 0;
            int size;
            do
            {
                size = await ReadAsync(StreamBuffer, StreamBufferIndex, STREAM_BUFFER_SIZE - StreamBufferIndex);
                if (size < 0)
                {
                    delivery.Error = (DeliveryError)size;
                    return delivery.Packing();
                }
                StreamBufferIndex += size;
            } while (DoReadAgain());
            byte[] recv_data = new byte[StreamBufferIndex];
            Buffer.BlockCopy(StreamBuffer, 0, recv_data, 0, recv_data.Length);

            delivery.Package = ReportResponseProtocol(reqt, recv_data);
            return delivery.Packing();
        }

        /// <summary>
        /// 메모리 해제
        /// </summary>
        public virtual void Dispose()
        {
            ReqeustProtocolPointer = null;
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