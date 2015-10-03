using System;
using System.Text;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet Protocol의 ASCII 데이터를 분석, 해석한다.
    /// </summary>
    public static class XGTCnetTranslator
    {
        /// <summary>
        /// 국번을 ASCII데이터로 변환하여 반환한다.
        /// </summary>
        public static byte[] LocalPortToASCII(ushort localport)
        {
            const int LOCALPORT_LIMIT = 99;
            if (localport > LOCALPORT_LIMIT)
                throw new ArgumentOutOfRangeException();

            byte[] result = null;
            string localport_str = string.Format("{0:D2}", localport);
            result = new byte[localport_str.Length];
            for (int i = 0; i < localport_str.Length; i++)
                result[i] = (byte)localport_str[i];
            return result;
        }

        /// <summary>
        /// ASCII데이터를 국번으로 해석하여 반환한다.
        /// </summary>
        public static ushort ASCIIToLocalPort(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(Convert.ToChar(b));
            return Convert.ToUInt16(sb.ToString());
        }

        /// <summary>
        /// 블록(개수) 데이터를 ASCII로 변환하여 반환한다.
        /// </summary>
        public static byte[] BlockDataToASCII(ushort value)
        {
            string hex_str = string.Format("{0:X2}", value);
            byte[] result = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                result[i] = (byte)hex_str[i];
            return result;
        }

        /// <summary>
        /// 블록(개수) 데이터를 ASCII로 변환하여 반환한다.
        /// </summary>
        public static byte[] BlockDataToASCII(int value)
        {
            string hex_str = string.Format("{0:X2}", value);
            byte[] result = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                result[i] = (byte)hex_str[i];
            return result;
        }

        /// <summary>
        /// ASCII데이터를 블록 데이터로 해석하여 반환환다.
        /// </summary>
        public static ushort ASCIIToBlockData(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(Convert.ToChar(b));
            string hex_str = sb.ToString();
            ushort target = Convert.ToUInt16(hex_str, 16);
            return target;
        }

        /// <summary>
        /// 메모리의 주소 이름을 ASCII데이터로 변환하여 반환한다.
        /// </summary>
        public static byte[] AddressDataToASCII(string address)
        {
            return Encoding.ASCII.GetBytes(address);
        }

        /// <summary>
        /// ASCII데이터를 메모리의 주소 이름으로 해석하여 반환한다.
        /// </summary>
        public static string ASCIIToAddressData(byte[] bytes)
        {
            return BitConverter.ToString(bytes, 0);
        }

        /// <summary>
        /// ASCII데이터를 에러코드로 해석하여 반환한다.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ushort ErrorCodeToInteger(byte[] code)
        {
            return ASCIIToLocalPort(code);
        }

        /// <summary>
        /// 정수 또는 이진 데이터를 ASCII데이터로 변환하여 반환한다.
        /// </summary>
        public static byte[] ValueDataToASCII(object value, Type type)
        {
            if (value == null || type == null)
                throw new ArgumentNullException();
            
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
            else
                throw new ArgumentException("Unsupported data types.");
            
            target = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                target[i] = (byte)hex_str[i];
            return target;
        }
        
        /// <summary>
        /// ASCII데이터를 정수 또는 이진 값으로 해석하여 반환한다.
        /// </summary>
        /// <returns></returns>
        public static object ASCIIToValueData(byte[] bytes, Type type)
        {
            if (bytes == null || type == null)
                throw new ArgumentNullException();

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
            else
                throw new ArgumentException("Unsupported data types.");
            return target;
        }
    }
}