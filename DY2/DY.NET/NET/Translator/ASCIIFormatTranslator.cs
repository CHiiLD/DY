using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public static class ASCIIFormatTranslator
    {
        /// <summary>
        /// 정수를 16진수로 변환한 뒤 각 자리수를 ASCII 문자로 치환하여 반환
        /// 256(10) => 0xFF(16) => { 0x45, 0x45 }
        /// boolean(bit) 타입은 byte(0x30 or 0x31)로 반환
        /// </summary>
        /// <param name="type"></param>
        /// <param name="integer">정수</param>
        /// <returns>ASCII데이터</returns>
        public static byte[] IntegerToHexASCII(Type type, object integer)
        {
            if (integer == null || type == null)
                throw new ArgumentNullException();

            string hex = string.Empty;
            if (type == typeof(Boolean))
                hex = (bool)integer == true ? "01" : "00";
            else if (type == typeof(Byte) || type == typeof(SByte))
                hex = String.Format("{0:X2}", integer);
            else if (type == typeof(Int16) || type == typeof(UInt16))
                hex = String.Format("{0:X4}", integer);
            else if (type == typeof(Int32) || type == typeof(UInt32))
                hex = String.Format("{0:X8}", integer);
            else if (type == typeof(Int64) || type == typeof(UInt64))
                hex = String.Format("{0:X16}", integer);
            else
                throw new ArgumentException("Unsupported data types.");

            byte[] result = new byte[hex.Length];
            for (int i = 0; i < hex.Length; i++)
                result[i] = (byte)hex[i];
            return result;
        }

        /// <summary>
        /// ASCII타입 데이터를 16진 정수로 해석하여 정수 반환
        /// {0x45, 0x45} => FF => 0xFF
        /// </summary>
        /// <param name="type">변환할 정수 타입</param>
        /// <param name="code">ASCII데이터</param>
        /// <returns>정수</returns>
        public static object HexASCIIToInteger(Type type, byte[] code)
        {
            const int BASE = 16;
            if (code == null || type == null)
                throw new ArgumentNullException();

            StringBuilder sb = new StringBuilder();
            foreach (byte b in code)
                sb.Append(Convert.ToChar(b));
            string hex = sb.ToString();

            object result = string.Empty;
            if (type == typeof(Boolean))
                result = Convert.ToBoolean(Convert.ToByte(hex, BASE));
            else if (type == typeof(SByte))
                result = Convert.ToSByte(hex, BASE);
            else if (type == typeof(Int16))
                result = Convert.ToInt16(hex, BASE);
            else if (type == typeof(Int32))
                result = Convert.ToInt32(hex, BASE);
            else if (type == typeof(Int64))
                result = Convert.ToInt64(hex, BASE);
            else if (type == typeof(Byte))
                result = Convert.ToByte(hex, BASE);
            else if (type == typeof(UInt16))
                result = Convert.ToUInt16(hex, BASE);
            else if (type == typeof(UInt32))
                result = Convert.ToUInt32(hex, BASE);
            else if (type == typeof(UInt64))
                result = Convert.ToUInt64(hex, BASE);
            else
                throw new ArgumentException("Unsupported data types.");
            return result;
        }

        /// <summary>
        /// 정수를 10진수로 변환한 뒤 각 자리수를 ASCII 문자로 치환하여 반환
        /// 12(10) => { 0x31, 0x32 }
        /// boolean(bit) 타입은 byte(0x30 or 0x31)로 반환
        /// </summary>
        /// <param name="integer">99 이하 정수</param>
        /// <returns>ASCII</returns>
        public static byte[] IntegerToDecASCII(Type type, object integer)
        {
            if (integer == null)
                throw new ArgumentNullException();
            string dec;
            if (type == typeof(Byte) || type == typeof(SByte))
                dec = String.Format("{0:D2}", integer);
            else if (type == typeof(Int16) || type == typeof(UInt16))
                dec = String.Format("{0:D4}", integer);
            else if (type == typeof(Int32) || type == typeof(UInt32))
                dec = String.Format("{0:D8}", integer);
            else if (type == typeof(Int64) || type == typeof(UInt64))
                dec = String.Format("{0:D16}", integer);
            else
                throw new ArgumentException("Unsupported data types.");

            byte[] result = new byte[dec.Length];
            for (int i = 0; i < dec.Length; i++)
                result[i] = (byte)dec[i];
            return result;
        }

        public static object DecASCIIToInteger(Type type, byte[] code)
        {
            const int BASE = 10;
            if (code == null || type == null)
                throw new ArgumentNullException();

            StringBuilder sb = new StringBuilder();
            foreach (byte b in code)
                sb.Append(Convert.ToChar(b));

            string str = sb.ToString();
            object result = null;
            if (type == typeof(Boolean))
                result = Convert.ToBoolean(Convert.ToByte(str, BASE));
            else if (type == typeof(SByte))
                result = Convert.ToSByte(str, BASE);
            else if (type == typeof(Int16))
                result = Convert.ToInt16(str, BASE);
            else if (type == typeof(Int32))
                result = Convert.ToInt32(str, BASE);
            else if (type == typeof(Int64))
                result = Convert.ToInt64(str, BASE);
            else if (type == typeof(Byte))
                result = Convert.ToByte(str, BASE);
            else if (type == typeof(UInt16))
                result = Convert.ToUInt16(str, BASE);
            else if (type == typeof(UInt32))
                result = Convert.ToUInt32(str, BASE);
            else if (type == typeof(UInt64))
                result = Convert.ToUInt64(str, BASE);
            else
                throw new ArgumentException("Unsupported data types.");

            return result;
        }

    }
}
