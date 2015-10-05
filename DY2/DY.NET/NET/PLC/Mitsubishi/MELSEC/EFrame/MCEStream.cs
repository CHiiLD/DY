using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.NET.LSIS.XGT;

namespace DY.NET.Mitsubishi.MELSEC
{
    public class MCEStream : XGTFEnetStream, IMCAdditionProperties
    {
        public MCEStream(string hostname, int port, IProtocolCompressorWithFormat compressor)
            : base(hostname, port)
        {
            Compressor = compressor;
        }

        public new IProtocolCompressorWithFormat Compressor
        {
            get;
            set;
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

        }

        protected override bool Continue(int idx)
        {
            return false;
        }
    }
}
