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
        private int m_ReceiveTimeout;
        private int m_SendTimeout;
        private int m_ConnectTimeout;
        protected IProtocolCompressor Compressor = new XGTCnetCompressor();
        protected byte[] ReadBuffer;

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
            ReceiveTimeout = SendTimeout = OpenTimeout = -1;
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
            Task connect_task = Task.Run(() => { base.Open(); });
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
            return base.IsOpen;
        }

        /// <summary>
        /// XGT와 비동기로 통신을 주고 받는다.
        /// </summary>
        /// <param name="protocol">요청 프로토콜</param>
        /// <returns>응답 프로토콜</returns>
        public virtual async Task<IProtocol> SendAsync(IProtocol protocol)
        {
            byte[] code = Compressor.Encode(protocol);
            Stream stream = GetStream();
            
            base.DiscardOutBuffer();
            CancellationTokenSource cts = new CancellationTokenSource(SendTimeout);
            try
            {
                await stream.WriteAsync(code, 0, code.Length, cts.Token);
            }
            catch (TimeoutException exception)
            {
                throw new WriteTimeoutException(exception);
            }

            int idx = 0;
            Array.Clear(this.ReadBuffer, 0, this.ReadBuffer.Length);
            cts = new CancellationTokenSource(ReceiveTimeout);
            base.DiscardInBuffer();
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
            } while (ReadBuffer[idx - 1] != XGTCnetHeader.ETX.ToByte());

            byte[] buffer = new byte[idx];
            System.Buffer.BlockCopy(this.ReadBuffer, 0, buffer, 0, buffer.Length);
            return Compressor.Decode(buffer, protocol.ItemType);
        }
    }
}
