using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public class MCCompressor : IProtocolCompressor
    {
        public byte[] Encode(IProtocol protocol)
        {
            return null;
        }

        public IProtocol Decode(byte[] ascii, Type type)
        {
            return null;
        }
    }
}
