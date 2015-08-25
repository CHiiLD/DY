using System;
using System.Text;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT CNET 전용, byte[] 데이터 <-> 정수, 문자열 데이터 ; 변환 클래스
    /// </summary>
    public static class CA2C
    {
        /// <summary>
        /// ushort 정수를 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Data2ASCII(ushort value)
        {
            byte[] target = null;
            string hex_str = string.Format("{0:X2}", value);
            target = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                target[i] = (byte)hex_str[i];
            return target;
        }

        public static byte[] Data2ASCII(int value)
        {
#if DEBUG
            if (value > ushort.MaxValue)
                System.Diagnostics.Debug.Assert(false);
#endif
            byte[] target = null;
            string hex_str = string.Format("{0:X2}", value);
            target = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                target[i] = (byte)hex_str[i];
            return target;
        }

        public static byte[] String2ASCII(string value)
        {
            byte[] target = null;
            target = new byte[value.Length];
            for (int i = 0; i < value.Length; i++)
                target[i] = (byte)value[i];
            return target;
        }

        public static byte[] Value2ASCII(object value, Type type)
        {
            //0x1234 -> 0x31, 32, 33, 34
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
#if DEBUG
            else

                System.Diagnostics.Debug.Assert(false);
#endif
            target = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                target[i] = (byte)hex_str[i];

            return target;
        }

        /// <summary>
        /// 정수를 표현하는 byte[]를 10진수 ushort로 변환한다.
        /// 0x32 0x30 -> 20 
        /// </summary>
        /// <param name="bytes">반드시 배열의 길이는 2이어야 한다.</param>
        /// <returns></returns>
        public static ushort ToUInt16Value(byte[] bytes)
        {
#if DEBUG
            if (bytes.Length != 2)
                System.Diagnostics.Debug.Assert(false);
#endif
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(Convert.ToChar(b));
            string hex_str = sb.ToString();
            ushort target = Convert.ToUInt16(hex_str, 16);
            return target;
        }

        public static object ToValue(byte[] bytes, Type type)
        {
            object target = null;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(Convert.ToChar(b));
            string hex_str = sb.ToString();

            if (type == typeof(Boolean))
                target = Convert.ToBoolean(Convert.ToByte(hex_str, 16));
            else if (type == typeof(SByte))
                target = Convert.ToSByte(hex_str, 16);
            else if (type == typeof(Int16))
                target = Convert.ToInt16(hex_str, 16);
            else if (type == typeof(Int32))
                target = Convert.ToInt32(hex_str, 16);
            else if (type == typeof(Int64))
                target = Convert.ToInt64(hex_str, 16);
            else if (type == typeof(Byte))
                target = Convert.ToByte(hex_str, 16);
            else if (type == typeof(UInt16))
                target = Convert.ToUInt16(hex_str, 16);
            else if (type == typeof(UInt32))
                target = Convert.ToUInt32(hex_str, 16);
            else if (type == typeof(UInt64))
                target = Convert.ToUInt64(hex_str, 16);
            else if (type == typeof(string))
                target = hex_str;
#if DEBUG
            else
                System.Diagnostics.Debug.Assert(false);
#endif
            return target;
        }
    }
}