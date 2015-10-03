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
        private int m_OpenTimeout;
        private int m_InputTimeout;
        private int m_OutputTimeout;
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

        public int InputTimeout
        {
            get
            {
                return m_InputTimeout;
            }
            set
            {
                m_InputTimeout = value >= 0 ? value : -1;
            }
        }

        public int OutputTimeout
        {
            get
            {
                return m_OutputTimeout;
            }
            set
            {
                m_OutputTimeout = value >= 0 ? value : -1;
            }
        }

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
            Task openTask = base.ConnectAsync(Hostname, Port);
            if (await Task.WhenAny(openTask, Task.Delay(OpenTimeout)) == openTask)
            {
                await openTask;
            }
            else
            {
                Close();
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// XGT와 비동기로 연결을 해제한다.
        /// </summary>
        /// <returns></returns>
        public new virtual void Close()
        {
            //base.Close();
            //base.Client.Disconnect(true);
            Client.Close();
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

        public virtual async Task WriteAsync(byte[] buffer)
        {
            Task writeTask = GetStream().WriteAsync(buffer, 0, buffer.Length);
            if (await Task.WhenAny(writeTask, Task.Delay(OutputTimeout)) == writeTask)
            {
                await writeTask;
            }
            else
            {
                Close();
                await OpenAsync();
                throw new WriteTimeoutException();
            }
        }

        public virtual async Task<int> ReadAsync()
        {
            int idx = 0;
            Array.Clear(this.ReadBuffer, 0, this.ReadBuffer.Length);
            Stream stream = GetStream();
            do
            {
                Task<int> readTask = stream.ReadAsync(this.ReadBuffer, idx, this.ReadBuffer.Length - idx);
                if (await Task.WhenAny(readTask, Task.Delay(InputTimeout)) == readTask)
                {
                    idx += await readTask;
                }
                else
                {
                    Close();
                    await OpenAsync();
                    throw new ReadTimeoutException();
                }
            } while (Continue(idx));
            return idx;
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
            byte[] code = Compressor.Encode(protocol);
            await WriteAsync(code);
            int size = await ReadAsync();
            byte[] buffer = new byte[size];
            System.Buffer.BlockCopy(this.ReadBuffer, 0, buffer, 0, buffer.Length);
            return Compressor.Decode(buffer, protocol.Type);
        }
    }
}