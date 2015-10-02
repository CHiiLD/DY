using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace DY.NET
{
    public abstract class AProtocolStream : IProtocolStream
    {
        protected IProtocolCompressor m_Compressor;
        protected byte[] Buffer;
        protected System.IO.Stream Stream;

        public int ReceiveTimeout { get; set; }
        public int SendTimeout { get; set; }

        public AProtocolStream()
        {
            SendTimeout = -1;
            ReceiveTimeout = -1;
        }

        public virtual Stream GetStream()
        {
            return this.Stream;
        }

        public abstract Task<bool> OpenAsync();
        public abstract Task CloseAsync();
        public abstract bool IsOpend();
        public abstract bool OnceMoreRead(int index);

        public virtual void DiscardInBuffer()
        {

        }

        public virtual async Task<IProtocol> SendAsync(IProtocol protocol)
        {
            byte[] code = m_Compressor.Encode(protocol);
            Stream stream = GetStream();

            CancellationTokenSource cts = new CancellationTokenSource();
            Task write_task = stream.WriteAsync(code, 0, code.Length, cts.Token);
            if (await Task.WhenAny(write_task, Task.Delay(SendTimeout, cts.Token)) != write_task)
            {
                cts.Cancel();
                throw new WriteTimeoutException();
            }
            await write_task;

            cts = new CancellationTokenSource();
            int idx = 0;
            Array.Clear(this.Buffer, 0, this.Buffer.Length);
            DiscardInBuffer();
            do
            {
                Task read_task = stream.ReadAsync(this.Buffer, idx, this.Buffer.Length - idx, cts.Token);
                if (await Task.WhenAny(read_task, Task.Delay(ReceiveTimeout, cts.Token)) != read_task)
                {
                    cts.Cancel();
                    throw new ReadTimeoutException();
                }
                idx += await (Task<int>)read_task;
            } while (OnceMoreRead(idx));

            var buffer = new byte[idx];
            System.Buffer.BlockCopy(this.Buffer, 0, buffer, 0, buffer.Length);
            return m_Compressor.Decode(buffer);
        }

        public void Dispose()
        {

        }
    }
}
