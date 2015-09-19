using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetProtocolCompressor : IProtocolCompressor
    {
        public virtual byte[] Encode(IProtocol protocol)
        {
            return null;
        }

        public virtual IProtocol Decode(byte[] ascii)
        {
            return null;
        }
    }
}
