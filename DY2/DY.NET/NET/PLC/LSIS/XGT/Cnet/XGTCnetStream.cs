using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet Protocol Communication
    /// </summary>
    public class XGTCnetStream : SerialPort, IProtocolStream
    {
        private int m_InputTimeout;
        private int m_OutputTimeout;
        private int m_ConnectTimeout;
        protected IProtocolCompressor Compressor = new XGTCnetCompressor();
        protected byte[] ReadBuffer;
        public ushort LocalPort { get; set; }

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
                return m_ConnectTimeout;
            }
            set
            {
                m_ConnectTimeout = value >= 0 ? value : -1;
            }
        }

        protected XGTCnetStream()
        {
            ReadBuffer = new byte[base.ReadBufferSize];
            InputTimeout = OutputTimeout = OpenTimeout = -1;
        }

        public XGTCnetStream(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : this()
        {
            PortName = portName;
            BaudRate = baudRate;
            base.Parity = parity;
            DataBits = dataBits;
            base.StopBits = stopBits;
        }

        /// <summary>
        /// SerialPort의 BaseStream을 반환한다.
        /// </summary>
        public virtual Stream GetStream()
        {
            return base.BaseStream;
        }

        /// <summary>
        /// XGT와 비동기로 연결을 시도한다.
        /// </summary>
        public virtual async Task OpenAsync()
        {
            Task openTask = Task.Run(() => { base.Open(); });
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
            base.Close();
        }

        /// <summary>
        /// 포트와 물리적인 연결이 있었는지 질의한다.
        /// </summary>
        public virtual bool IsOpend()
        {
            return base.IsOpen;
        }

        public virtual async Task WriteAsync(byte[] buffer)
        {
            base.DiscardOutBuffer();
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
            base.DiscardInBuffer();
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
            } while (ReadBuffer[idx - 1] != ControlChar.ETX.ToByte());
            return idx;
        }

        /// <summary>
        /// XGT와 비동기로 통신을 주고 받는다.
        /// </summary>
        /// <param name="protocol">요청 프로토콜</param>
        /// <returns>응답 프로토콜</returns>
        public virtual async Task<IProtocol> SendAsync(IProtocol protocol)
        {
            byte[] code = Compressor.Encode(protocol);
            await WriteAsync(code);
            int size = await ReadAsync();
            byte[] buffer = new byte[size];
            System.Buffer.BlockCopy(this.ReadBuffer, 0, buffer, 0, buffer.Length);
            return Compressor.Decode(buffer, protocol.Type);
        }
    }
}