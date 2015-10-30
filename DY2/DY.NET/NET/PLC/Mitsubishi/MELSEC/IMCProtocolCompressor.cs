using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public interface IMCProtocolCompressor : IProtocolCompressor
    {
        MCProtocolFormat Format { get; set; }
    }
}