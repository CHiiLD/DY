using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public enum MC3ECommand : ushort
    {
        NONE = 0x0000,
        R = 0x0401,
        W = 0x1401
    }

    public static class MC3ECommandExtension
    {
        public static byte[] ToASCII()
        {
            return null;
        }

        public static byte[] ToBinary()
        {
            return null;
        }
    }
}
