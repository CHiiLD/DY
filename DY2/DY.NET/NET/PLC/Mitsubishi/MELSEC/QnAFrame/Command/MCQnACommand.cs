using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public enum MCQnACommand : ushort
    {
        NONE = 0x0000,
        READ = 0x0401,
        WRITE = 0x1401
    }

    public static class MCQnACommandExtension
    {
        public static byte[] ToASCII(this MCQnACommand command)
        {
            return null;
        }

        public static byte[] ToBinary(this MCQnACommand command)
        {
            return null;
        }
    }
}
