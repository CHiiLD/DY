using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IProtocolCompressor
    {
        byte[] Encode(IProtocol protocol);
        IProtocol Decode(byte[] ascii, IProtocol request);
    }
}