using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public enum MC3ESubHeader : ushort
    {
        NONE = 0x0000,
        REQUEST = 0x5000,
        RESPONSE = 0xD000
    }
}