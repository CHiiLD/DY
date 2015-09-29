using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace DY.NET.LSIS.XGT
{
    public class XGTFEnetStream : TcpClient, IProtocolStream
    {
#if false
        private int m_ReceiveTimeout;
        private int m_SendTimeout;
#endif
        private int m_OpenTimeout;
        protected IProtocolCompressor Compressor = new XGTFEnetCompressor();
        protected byte[] ReadBuffer;

        public string Hostname
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

#if false
        public int ReceiveTimeout
        {
            get
            {
                return m_ReceiveTimeout;
            }
            set
            {
                m_ReceiveTimeout = value >= 0 ? value : -1;
            }
        }

        public int SendTimeout
        {
            get
            {
                return m_SendTimeout;
            }
            set
            {
                m_SendTimeout = value >= 0 ? value : -1;
            }
        }
#endif

        public int OpenTimeout
        {
            get
            {
                return m_OpenTimeout;
            }
            set
            {
                m_OpenTimeout = value >= 0 ? value : -1;
            }
        }

        public XGTFEnetStream()
        {
            ReadBuffer = new byte[base.ReceiveBufferSize];
            ReceiveTimeout = SendTimeout = OpenTimeout = -1;
        }

        public XGTFEnetStream(string hostname, int port = (int)XGTFEnetPort.TCP)
            : this()
        {
            Hostname = hostname;
            Port = port;
        }

        /// <summary>
        /// SerialPort의 BaseStream을 반환한다.
        /// </summary>
        public new virtual Stream GetStream()
        {
            return base.GetStream();
        }

        /// <summary>
        /// XGT와 비동기로 연결을 시도한다.
        /// </summary>
        public virtual async Task OpenAsync()
        {
            Task connect_task = base.ConnectAsync(Hostname, Port);
            if (await Task.WhenAny(connect_task, Task.Delay(OpenTimeout)) == connect_task)
                await connect_task;
            else
                throw new TimeoutException();
        }

        /// <summary>
        /// XGT와 비동기로 연결을 해제한다.
        /// </summary>
        /// <returns></returns>
        public new virtual void Close()
        {
            base.Close();
        }

        /// <summary>
        /// 포트와 물리적인 연결이 있었는지 질의한다.
        /// </summary>
        public virtual bool IsOpend()
        {
            return base.Connected;
        }

        private bool Continue(int idx)
        {
            ushort bodyLen = 0;
            ushort targetLen = 0;

            if (idx < XGTFEnetCompressor.HEADER_SIZE)
                return true;
            targetLen = (ushort)XGTFEnetTranslator.ToValue(new byte[] { ReadBuffer[XGTFEnetCompressor.HEADER_LENGTH_IDX1], ReadBuffer[XGTFEnetCompressor.HEADER_LENGTH_IDX2] }, typeof(ushort));
            bodyLen = (ushort)(idx - XGTFEnetCompressor.HEADER_SIZE);
            return bodyLen != targetLen;
        }

        /// <summary>
        /// XGT와 비동기로 통신을 주고 받는다.
        /// </summary>
        /// <param name="protocol">요청 프로토콜</param>
        /// <returns>응답 프로토콜</returns>
        public virtual async Task<IProtocol> SendAsync(IProtocol protocol)
        {
            if (!base.Connected)
                await OpenAsync();

            Stream stream = GetStream();
            byte[] code = Compressor.Encode(protocol);
            CancellationTokenSource cts = new CancellationTokenSource(SendTimeout);
            try
            {
                await stream.WriteAsync(code, 0, code.Length, cts.Token);
            }
            catch(TimeoutException exception)
            {
                throw new WriteTimeoutException(exception);
            }
            int idx = 0;
            Array.Clear(this.ReadBuffer, 0, this.ReadBuffer.Length);
            cts = new CancellationTokenSource(ReceiveTimeout);
            do
            {
                try
                {
                    idx += await stream.ReadAsync(this.ReadBuffer, idx, this.ReadBuffer.Length - idx, cts.Token);
                }
                catch (TimeoutException exception)
                {
                    throw new ReadTimeoutException(exception);
                }
            } while (Continue(idx));
            byte[] buffer = new byte[idx];
            System.Buffer.BlockCopy(this.ReadBuffer, 0, buffer, 0, buffer.Length);
            return Compressor.Decode(buffer, protocol.Type);
        }
    }
}