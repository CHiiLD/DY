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
    public class XGTCnetStream : SerialPort, IProtocolStream
    {
        private int m_ReceiveTimeout;
        private int m_SendTimeout;
        protected XGTCnetCompressor Compressor = new XGTCnetCompressor();
        protected byte[] ReadBuffer;

        public int ReceiveTimeout
        {
            get
            {
                return m_ReceiveTimeout;
            }
            set
            {
                m_ReceiveTimeout = value >= 0 ? 0 : -1;
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
                m_SendTimeout = value >= 0 ? 0 : -1;
            }
        }

        public XGTCnetStream(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            ReadBuffer = new byte[base.ReadBufferSize];
            ReceiveTimeout = -1;
            SendTimeout = -1;
        }

        public virtual Stream GetStream()
        {
            return base.BaseStream;
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public virtual async Task OpenAsync()
        {
            base.Open();
        }

        public virtual async Task CloseAsync()
        {
            base.Close();
        }

        public virtual bool IsOpend()
        {
            return base.IsOpen;
        }

        public virtual async Task<IProtocol> SendAsync(IProtocol protocol)
        {
            byte[] code = Compressor.Encode(protocol);
            Stream stream = GetStream();

            CancellationTokenSource cts = new CancellationTokenSource();
            base.DiscardOutBuffer();
            Task write_task = stream.WriteAsync(code, 0, code.Length, cts.Token);
            if (await Task.WhenAny(write_task, Task.Delay(SendTimeout, cts.Token)) != write_task)
            {
                cts.Cancel();
                throw new WriteTimeoutException();
            }
            await write_task;

            cts = new CancellationTokenSource();
            int idx = 0;
            Array.Clear(this.ReadBuffer, 0, this.ReadBuffer.Length);
            base.DiscardInBuffer();
            do
            {
                Task read_task = stream.ReadAsync(this.ReadBuffer, idx, this.ReadBuffer.Length - idx, cts.Token);
                if (await Task.WhenAny(read_task, Task.Delay(ReceiveTimeout, cts.Token)) != read_task)
                {
                    cts.Cancel();
                    throw new ReadTimeoutException();
                }
                idx += await (Task<int>)read_task;
            } while (ReadBuffer[idx - 1] != XGTCnetHeader.ETX.ToByte());

            var buffer = new byte[idx];
            System.Buffer.BlockCopy(this.ReadBuffer, 0, buffer, 0, buffer.Length);
            return Compressor.Decode(buffer);
        }
    }
}
