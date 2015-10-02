using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// FENET 전용, 프로토콜 데이터 변환 클래스
    /// </summary>
    public static class XGTFEnetTranslator
    {
        public static object ToValue(byte[] bytes, Type type)
        {
            if (bytes == null || type == null)
                throw new ArgumentNullException();
            if (BitConverter.IsLittleEndian && bytes.Length != 1 && type != typeof(string))
                Array.Reverse(bytes);
            object target = null;
            if (type == typeof(Boolean))
                target = BitConverter.ToBoolean(bytes, 0);
            else if (type == typeof(SByte))
                target = (sbyte)bytes[0];
            else if (type == typeof(Int16))
                target = BitConverter.ToInt16(bytes.Reverse().ToArray(), 0);
            else if (type == typeof(Int32))
                target = BitConverter.ToInt32(bytes, 0);
            else if (type == typeof(Int64))
                target = BitConverter.ToInt64(bytes, 0);
            else if (type == typeof(Byte))
                target = (byte)bytes[0];
            else if (type == typeof(UInt16))
                target = BitConverter.ToUInt16(bytes.Reverse().ToArray(), 0);
            else if (type == typeof(UInt32))
                target = BitConverter.ToUInt32(bytes, 0);
            else if (type == typeof(UInt64))
                target = BitConverter.ToUInt64(bytes, 0);
            else if (type == typeof(String))
                target = BitConverter.ToString(bytes, 0);
            else
                throw new ArgumentException("Unsupported data type.");
            return target;
        }

#if false
        public static byte[] ToASCII(object value)
        {
            if (value == null)
                throw new ArgumentNullException();
            return ToASCII(value, value.GetType());
        }
#endif

        public static byte[] ToASCII(object value, Type type)
        {
            if (value == null || type == null)
                throw new ArgumentNullException();
            byte[] bytes = null;
            if (type == typeof(Boolean))
                bytes = BitConverter.GetBytes(Boolean.Parse(string.Format("{0}", value)));
            else if (type == typeof(SByte))
                bytes = new byte[] { unchecked((byte)(sbyte)value) };
            else if (type == typeof(UInt16))
                bytes = BitConverter.GetBytes(UInt16.Parse(string.Format("{0}", value))).Reverse().ToArray();
            else if (type == typeof(UInt32))
                bytes = BitConverter.GetBytes(UInt32.Parse(string.Format("{0}", value)));
            else if (type == typeof(UInt64))
                bytes = BitConverter.GetBytes(UInt64.Parse(string.Format("{0}", value)));
            else if (type == typeof(Byte))
                bytes = new byte[] { (byte)value };
            else if (type == typeof(Int16))
                bytes = BitConverter.GetBytes(Int16.Parse(string.Format("{0}", value))).Reverse().ToArray();
            else if (type == typeof(Int32))
                bytes = BitConverter.GetBytes(Int32.Parse(string.Format("{0}", value)));
            else if (type == typeof(Int64))
                bytes = BitConverter.GetBytes(Int64.Parse(string.Format("{0}", value)));
            else if (type == typeof(String))
                bytes = Encoding.ASCII.GetBytes((string)value);
            else
                throw new ArgumentException("Unsupported data type.");
            if (BitConverter.IsLittleEndian && type != typeof(String) && bytes.Length != 1)
                Array.Reverse(bytes);
            return bytes;
        }
    }
}