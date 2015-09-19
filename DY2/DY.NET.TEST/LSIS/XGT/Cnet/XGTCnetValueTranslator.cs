using System;
using System.Text;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT CNET 전용, byte[] 데이터 <-> 정수, 문자열 데이터 ; 변환 클래스
    /// </summary>
    public static class XGTCnetASCIITranslator
    {
        public static byte[] UInt16ToTwoByte(ushort value)
        {
            byte[] ascii = null;
            string hex_str = string.Format("{0:X2}", value);
            ascii = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                ascii[i] = (byte)hex_str[i];
            return ascii;
        }

        /// <summary>
        /// 정수를 표현하는 byte[]를 10진수 ushort로 변환한다.
        /// 0x32 0x30 -> 20 
        /// </summary>
        /// <param name="bytes">반드시 배열의 길이는 2이어야 한다.</param>
        /// <returns></returns>
        public static ushort TwoByteToUInt16(byte[] bytes)
        {
            if (bytes.Length != 2)
                throw new ArgumentOutOfRangeException();

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(Convert.ToChar(b));
            string hex_str = sb.ToString();
            ushort target = Convert.ToUInt16(hex_str, 16);
            return target;
        }

        //public static byte[] UInt32ToFourByte(uint value)
        //{
        //    byte[] target = null;
        //    string hex_str = string.Format("{0:X2}", value);
        //    target = new byte[hex_str.Length];
        //    for (int i = 0; i < hex_str.Length; i++)
        //        target[i] = (byte)hex_str[i];
        //    return target;
        //}

        public static byte[] ValueToASCII(object value)
        {
            return ValueToASCII(value, value.GetType());
        }

        public static byte[] ValueToASCII(object value, Type type)
        {
            byte[] target = null;
            string hex_str = string.Empty;
            if (type == typeof(Boolean))
                hex_str = (bool)value == true ? "01" : "00";
            else if (type == typeof(Byte) || type == typeof(SByte))
                hex_str = string.Format("{0:X2}", value);
            else if (type == typeof(Int16) || type == typeof(UInt16))
                hex_str = string.Format("{0:X4}", value);
            else if (type == typeof(Int32) || type == typeof(UInt32))
                hex_str = string.Format("{0:X8}", value);
            else if (type == typeof(Int64) || type == typeof(UInt64))
                hex_str = string.Format("{0:X16}", value);
            else if (type == typeof(String))
                hex_str = (string)value;
            target = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                target[i] = (byte)hex_str[i];
            return target;
        }
        
        public static object ASCIIToValue(byte[] bytes, Type type)
        {
            object target = null;
            const int HEX = 16;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(Convert.ToChar(b));
            string hex_str = sb.ToString();

            if (type == typeof(Boolean))
                target = Convert.ToBoolean(Convert.ToByte(hex_str, HEX));
            else if (type == typeof(SByte))
                target = Convert.ToSByte(hex_str, HEX);
            else if (type == typeof(Int16))
                target = Convert.ToInt16(hex_str, HEX);
            else if (type == typeof(Int32))
                target = Convert.ToInt32(hex_str, HEX);
            else if (type == typeof(Int64))
                target = Convert.ToInt64(hex_str, HEX);
            else if (type == typeof(Byte))
                target = Convert.ToByte(hex_str, HEX);
            else if (type == typeof(UInt16))
                target = Convert.ToUInt16(hex_str, HEX);
            else if (type == typeof(UInt32))
                target = Convert.ToUInt32(hex_str, HEX);
            else if (type == typeof(UInt64))
                target = Convert.ToUInt64(hex_str, HEX);
            else if (type == typeof(string))
                target = hex_str;
            return target;
        }
    }
}