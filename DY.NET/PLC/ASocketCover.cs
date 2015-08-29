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
        protected readonly AsyncLock Locker = new AsyncLock();

        private int m_ReadTimeout;
        private int m_ReadTimeoutMaximum;
        private int m_ReadTimeoutMinimum;

        private int m_WriteTimeout;
        private int m_WriteTimeoutMaximum;
        private int m_WriteTimeoutMinimum;

        protected readonly byte[] EMPTY_BYTE = new byte[1];
        public const int BUFFER_SIZE = 4096;
        protected byte[] BaseBuffer = new byte[BUFFER_SIZE];
        protected Stream BaseStream { get; set; }
        
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
        protected abstract bool DoReadAgain(AProtocol request, int idx);
        protected abstract AProtocol CreateResponseProtocol(AProtocol request, byte[] recv_data);
        protected abstract void DiscardInBuffer();

        /// <summary>
        /// 생성자 
        /// </summary>
        protected ASocketCover()
        {
            WriteTimeoutMaximum = 500;
            ReadTimeoutMaximum = 500;

            WriteTimeoutMinimum = 50;
            ReadTimeoutMinimum = 50;

            WriteTimeout = 250;
            ReadTimeout = 250;
        }

        private bool IsPassibleCommunication()
        {
            bool isConnected = true;
            if (!IsConnected()) //CONNECT
           { 
                Close();
                if (!Connect())
                    isConnected = false;
            }
            return isConnected;
        }

        private async Task<bool> WritePorotocol(AProtocol p)
        {
            bool isWrote = true;
            CancellationTokenSource cts = new CancellationTokenSource();

            Task write_task = BaseStream.WriteAsync(p.ASCII, 0, p.ASCII.Length, cts.Token);
            if (await Task.WhenAny(write_task, Task.Delay(WriteTimeout, cts.Token)) != write_task)
                isWrote = false;
            else
                await write_task;

            if (!cts.IsCancellationRequested)
                cts.Cancel();

            await BaseStream.FlushAsync();
            
            return isWrote;
        }

        private async Task<byte[]> ReadProtocol(AProtocol p)
        {
            int idx = 0;
            byte[] buffer = null;
            CancellationTokenSource cts = new CancellationTokenSource();
            DiscardInBuffer();
            do
            {
                if (cts.IsCancellationRequested)
                    break;
                Task read_task = BaseStream.ReadAsync(BaseBuffer, idx, BUFFER_SIZE - idx, cts.Token);
                if (await Task.WhenAny(read_task, Task.Delay(ReadTimeout, cts.Token)) != read_task)
                {
                    idx = 0;
                    break;
                }
                int read_size = await (Task<int>)read_task;
                if (read_size <= 0)
                    System.Diagnostics.Debug.Assert(false);
                idx += read_size;
            } while (DoReadAgain(p, idx));

            if (!cts.IsCancellationRequested)
                cts.Cancel();

            if(idx != 0)
            {
                buffer = new byte[idx];
                Buffer.BlockCopy(BaseBuffer, 0, buffer, 0, buffer.Length);
            }
            return buffer;
        }

        /// <summary>
        /// 비동기 통신으로 PLC와 통신하여 요청 메세지를 보내고 응답 메세지를 받는다
        /// </summary>
        /// <param name="request">XGTCnetProtocol 요청 프로토콜</param>
        /// <returns>Delivery</returns>
        public async Task<Delivery> PostAsync(IProtocol request)
        {
            Delivery delivery = new Delivery();
            if (!IsPassibleCommunication())
                return delivery.Packing(DeliveryError.DISCONNECT);

            using (await Locker.LockAsync())
            {
                do
                {
                    Array.Clear(BaseBuffer, 0, BUFFER_SIZE);
                    AProtocol p = request as AProtocol;
                    if (!await WritePorotocol(p))
                    {
                        delivery.Error = DeliveryError.WRITE_TIMEOUT;
                        break;
                    }
                    byte[] recv_data = await ReadProtocol(p);
                    if (recv_data == null)
                    {
                        delivery.Error = DeliveryError.READ_TIMEOUT;
                        break;
                    }
                    delivery.Package = CreateResponseProtocol(p, recv_data);
                } while (false);
            }
            return delivery.Packing();
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