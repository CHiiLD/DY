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
        /// 정수, 문자열 데이터를 바이트 크기에 맞추어 byte[]로 변환한다.
        /// </summary>
        /// <param name="value">정수, 문자열 데이터</param>
        /// <param name="type">타겟 타입</param>
        /// <returns></returns>
        public static byte[] ToASC(object value, Type type)
        {
            byte[] target = null;
            string hex_str = string.Empty;
            if (type == typeof(Boolean))
                hex_str = (bool)value == true ? "01" : "00";
            else if (type == typeof(Byte) || type == typeof(SByte))
                hex_str = string.Format("{0:X2}", value);
            else if (type == typeof(Int16) || type == typeof(UInt16))
                hex_str = string.Format("{0:X2}", value);
            else if (type == typeof(Int32) || type == typeof(UInt32))
                hex_str = string.Format("{0:X4}", value);
            else if (type == typeof(Int64) || type == typeof(UInt64))
                hex_str = string.Format("{0:X8}", value);
            else if (type == typeof(string))
                hex_str = (string)value;
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
        /// 정수, 문자열 데이터를 바이트 크기에 맞추어 byte[]로 변환한다.
        /// </summary>
        /// <param name="value">정수, 문자열 데이터</param>
        /// <returns></returns>
        public static byte[] ToASC(object value)
        {
            return ToASC(value, value.GetType());
        }

        /// <summary>
        /// byte[] 데이터를 정수 또는 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes">byte[]데이터</param>
        /// <param name="type">타겟 타입</param>
        /// <returns>정수 또는 문자열</returns>
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