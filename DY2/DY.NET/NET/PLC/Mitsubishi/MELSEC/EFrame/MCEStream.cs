using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.NET.LSIS.XGT;

namespace DY.NET.Mitsubishi.MELSEC
{
    public class MCEStream : XGTFEnetStream, IMCStream
    {
        public MCEStream(string hostname, int port)
            : base(hostname, port)
        {
        }

        public MCEStream(string hostname, int port, IProtocolCompressorWithFormat compressor)
            : base(hostname, port)
        {
            Compressor = compressor;
        }

        public MCProtocolFormat Format
        {
            get
            {
                IProtocolCompressorWithFormat comFmt = Compressor as IProtocolCompressorWithFormat;
                return comFmt.Format;
            }
            set
            {
                IProtocolCompressorWithFormat comFmt = Compressor as IProtocolCompressorWithFormat;
                comFmt.Format = value;
            }
        }

        protected override void InitializeCompressor()
        {
            Compressor = new MC3ECompressor();
        }

        protected override bool Continue(int idx)
        {
            return false;
        }
    }
}
