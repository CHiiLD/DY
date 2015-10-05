using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public static class BinaryFormatTranslator
    {
        public static byte[] IntegerToBinary(Type type, object integer)
        {
            if (integer == null || type == null)
                throw new ArgumentNullException();

            byte[] result = null;
            if (type == typeof(Boolean))
                result = BitConverter.GetBytes(Boolean.Parse(string.Format("{0}", integer)));
            else if (type == typeof(SByte))
                result = new byte[] { unchecked((byte)(sbyte)integer) };
            else if (type == typeof(UInt16))
                result = BitConverter.GetBytes(UInt16.Parse(string.Format("{0}", integer)));
            else if (type == typeof(UInt32))
                result = BitConverter.GetBytes(UInt32.Parse(string.Format("{0}", integer)));
            else if (type == typeof(UInt64))
                result = BitConverter.GetBytes(UInt64.Parse(string.Format("{0}", integer)));
            else if (type == typeof(Byte))
                result = new byte[] { (byte)integer };
            else if (type == typeof(Int16))
                result = BitConverter.GetBytes(Int16.Parse(string.Format("{0}", integer)));
            else if (type == typeof(Int32))
                result = BitConverter.GetBytes(Int32.Parse(string.Format("{0}", integer)));
            else if (type == typeof(Int64))
                result = BitConverter.GetBytes(Int64.Parse(string.Format("{0}", integer)));
            else
                throw new ArgumentException("Unsupported data type.");

            if (BitConverter.IsLittleEndian && result.Length != 1)
                Array.Reverse(result);

            return result;
        }

        public static object BinaryToInteger(Type type, byte[] code)
        {
            if (code == null || type == null)
                throw new ArgumentNullException();

            if (BitConverter.IsLittleEndian && code.Length != 1)
                Array.Reverse(code);

            object result = null;
            if (type == typeof(Boolean))
                result = BitConverter.ToBoolean(code, 0);
            else if (type == typeof(SByte))
                result = (sbyte)code[0];
            else if (type == typeof(Int16))
                result = BitConverter.ToInt16(code, 0);
            else if (type == typeof(Int32))
                result = BitConverter.ToInt32(code, 0);
            else if (type == typeof(Int64))
                result = BitConverter.ToInt64(code, 0);
            else if (type == typeof(Byte))
                result = (byte)code[0];
            else if (type == typeof(UInt16))
                result = BitConverter.ToUInt16(code, 0);
            else if (type == typeof(UInt32))
                result = BitConverter.ToUInt32(code, 0);
            else if (type == typeof(UInt64))
                result = BitConverter.ToUInt64(code, 0);
            else
                throw new ArgumentException("Unsupported data type.");

            return result;
        }
    }
}