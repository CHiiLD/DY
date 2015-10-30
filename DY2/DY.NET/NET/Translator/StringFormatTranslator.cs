using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public static class StringFormatTranslator
    {
        public static byte[] StringToByteArray(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        //public static string ByteArrayToString(byte[] code)
        //{
        //    return BitConverter.ToString(code);
        //}

        public static string ByteArrayToString(params byte[] code)
        {
            return BitConverter.ToString(code);
        }
    }
}
