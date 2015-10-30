using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.NET.LSIS.XGT;

namespace DY.NET.Mitsubishi.MELSEC
{
    public class MCEFrameStream : XGTFEnetStream, IMCStreamProperties
    {
        public MCEFrameStream(string hostname, int port, IMCProtocolCompressor compressor)
            : base(hostname, port)
        {
            Compressor = compressor;
        }

        public new IMCProtocolCompressor Compressor
        {
            get;
            set;
        }

        public MCProtocolFormat Format
        {
            get
            {
                return Compressor.Format;
            }
            set
            {
                Compressor.Format = value;
            }
        }

        protected override void InitializeCompressor()
        {

        }

        protected override bool Continue(int idx)
        {
            return false;
        }

        /// <summary>
        /// XGT와 비동기로 통신을 주고 받는다.
        /// </summary>
        /// <param name="protocol">요청 프로토콜</param>
        /// <returns>응답 프로토콜</returns>
        public override async Task<IProtocol> SendAsync(IProtocol protocol)
        {
            if (!base.Connected)
                await OpenAsync();
            byte[] code = Compressor.Encode(protocol);
            await WriteAsync(code);
            int size = await ReadAsync();
            byte[] buffer = new byte[size];
            System.Buffer.BlockCopy(this.ReadBuffer, 0, buffer, 0, buffer.Length);
            return Compressor.Decode(buffer, protocol);
        }
    }
}
