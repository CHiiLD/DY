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
        private int m_ReceiveTimeout;
        private int m_SendTimeout;
        private int m_ConnectTimeout;
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

        public int ReceiveingTimeout
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

        public int SendingTimeout
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

        public int ConnectingTimeout
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

        public XGTFEnetStream()
        {
            ReadBuffer = new byte[base.ReceiveBufferSize];
            ReceiveingTimeout = SendingTimeout = ConnectingTimeout = -1;
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
            if (await Task.WhenAny(connect_task, Task.Delay(ConnectingTimeout)) == connect_task)
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

        /// <summary>
        /// XGT와 비동기로 통신을 주고 받는다.
        /// </summary>
        /// <param name="protocol">요청 프로토콜</param>
        /// <returns>응답 프로토콜</returns>
        public virtual async Task<IProtocol> SendAsync(IProtocol protocol)
        {
            byte[] code = Compressor.Encode(protocol);
            Stream stream = GetStream();

            CancellationTokenSource cts = new CancellationTokenSource();
            Task write_task = stream.WriteAsync(code, 0, code.Length, cts.Token);
            if (await Task.WhenAny(write_task, Task.Delay(SendingTimeout, cts.Token)) != write_task)
            {
                cts.Cancel();
                throw new WriteTimeoutException();
            }
            await write_task;

            cts = new CancellationTokenSource();
            int idx = 0;
            ushort body_sum = 0;
            ushort target_sum = 0;
            Array.Clear(this.ReadBuffer, 0, this.ReadBuffer.Length);
            do
            {
                Task read_task = stream.ReadAsync(this.ReadBuffer, idx, this.ReadBuffer.Length - idx, cts.Token);
                if (await Task.WhenAny(read_task, Task.Delay(ReceiveingTimeout, cts.Token)) != read_task)
                {
                    cts.Cancel();
                    throw new ReadTimeoutException();
                }
                idx += await (Task<int>)read_task;
                if (idx >= 18 && target_sum == 0)
                    target_sum = (ushort)XGTFEnetTranslator.ToValue(new byte[] { ReadBuffer[17], ReadBuffer[16] }, typeof(ushort));
                else
                    continue;
                for (int i = 20; i < idx; i++)
                    body_sum += ReadBuffer[i];
            } while (target_sum != body_sum);

            var buffer = new byte[idx];
            System.Buffer.BlockCopy(this.ReadBuffer, 0, buffer, 0, buffer.Length);
            return Compressor.Decode(buffer);
        }
    }
}