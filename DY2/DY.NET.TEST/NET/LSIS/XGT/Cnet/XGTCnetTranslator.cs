using System;
using System.Text;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT CNET 전용, byte[] 데이터 <-> 정수, 문자열 데이터 ; 변환 클래스
    /// </summary>
    public static class XGTCnetTranslator
    {
        public static byte[] LocalPortToInfoData(ushort localport)
        {
            const int LOCALPORT_LIMIT = 99;
            if (localport > LOCALPORT_LIMIT)
                throw new ArgumentOutOfRangeException();

            byte[] result = null;
            string localport_str = string.Format("{0}", localport);
            result = new byte[localport_str.Length];
            for (int i = 0; i < localport_str.Length; i++)
                result[i] = (byte)localport_str[i];
            return result;
        }

        public static ushort InfoDataToLocalPort(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(Convert.ToChar(b));
            return Convert.ToUInt16(sb.ToString());
        }

        public static byte[] UInt16ToInfoData(ushort value)
        {
            string hex_str = string.Format("{0:X2}", value);
            byte[] result = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                result[i] = (byte)hex_str[i];
            return result;
        }

        public static byte[] Int32ToInfoData(int value)
        {
            string hex_str = string.Format("{0:X2}", value);
            byte[] result = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                result[i] = (byte)hex_str[i];
            return result;
        }

        public static ushort InfoDataToUInt16(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(Convert.ToChar(b));
            string hex_str = sb.ToString();
            ushort target = Convert.ToUInt16(hex_str, 16);
            return target;
        }

        public static byte[] AddressDataToASCII(string address)
        {
            var target = new byte[address.Length];
            for (int i = 0; i < address.Length; i++)
                target[i] = (byte)address[i];
            return target;
        }

        public static string ASCIIToAddressData(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(Convert.ToChar(b));
            return sb.ToString();
        }

        public static ushort ErrorCodeToInteger(byte[] code)
        {
            return InfoDataToLocalPort(code);
        }

        public static byte[] ValueDataToASCII(object value)
        {
            return ValueDataToASCII(value, value.GetType());
        }

        public static byte[] ValueDataToASCII(object value, Type type)
        {
            if (value == null)
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
                throw new ArgumentException();
            
            target = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                target[i] = (byte)hex_str[i];
            return target;
        }
        
        public static object ASCIIToValueData(byte[] bytes, Type type)
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
            else
                throw new ArgumentException();
            return target;
        }
    }
}