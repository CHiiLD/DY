using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public enum MC3EHeader : ushort
    {
        NONE = 0x0000,
        REQUEST = 0x5000,
        RESPONSE = 0xD000
    }

    public static class MC3EHeaderExtension
    {
        public static byte[] ToBinary(this MC3EHeader header)
        {
            byte[] result = new byte[2];
            switch (header)
            {
                case MC3EHeader.REQUEST:
                    result[0] = 0x50;
                    result[1] = 0x00;
                    break;
                case MC3EHeader.RESPONSE:
                    result[0] = 0xD0;
                    result[1] = 0x00;
                    break;
            }
            return result;
        }

        public static byte[] ToASCII(this MC3EHeader header)
        {
            byte[] result = new byte[4];
            switch(header)
            {
                case MC3EHeader.REQUEST:
                    result[0] = 0x35;
                    result[1] = 0x30;
                    result[2] = 0x30;
                    result[3] = 0x30;
                    break;
                case MC3EHeader.RESPONSE:
                    result[0] = 0x44;
                    result[1] = 0x30;
                    result[2] = 0x30;
                    result[3] = 0x30;
                    break;
            }
            return result;
        }
    }
}