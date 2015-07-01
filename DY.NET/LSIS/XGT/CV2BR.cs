using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public static class CV2BR
    {
        public static ushort ToValue(byte[] bytes)
        {
            return (ushort)ToValue(bytes, typeof(ushort));
        }

        public static object ToValue(byte[] bytes, Type type)
        {
            var r_bytes = bytes.Reverse().ToArray();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(r_bytes);
            object target = null;
            if (type == typeof(Boolean))
                target = BitConverter.ToBoolean(r_bytes, 0);
            else if (type == typeof(SByte))
                target = (sbyte)r_bytes[0];
            else if (type == typeof(Int16))
                target = BitConverter.ToInt16(r_bytes, 0);
            else if (type == typeof(Int32))
                target = BitConverter.ToInt32(r_bytes, 0);
            else if (type == typeof(Int64))
                target = BitConverter.ToInt64(r_bytes, 0);
            else if (type == typeof(Byte))
                target = (byte)r_bytes[0];
            else if (type == typeof(UInt16))
                target = BitConverter.ToUInt16(r_bytes, 0);
            else if (type == typeof(UInt32))
                target = BitConverter.ToUInt32(r_bytes, 0);
            else if (type == typeof(UInt64))
                target = BitConverter.ToUInt64(r_bytes, 0);
            else if (type == typeof(string))
                target = BitConverter.ToString(r_bytes, 0);
            else
#if DEBUG
                System.Diagnostics.Debug.Assert(false);
#endif
            return target;
        }

        public static byte[] ToBytes(bool value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] ToBytes(byte value)
        {
            byte[] bytes = new byte[] { value };
            return bytes;
        }

        public static byte[] ToBytes(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            Array.Reverse(bytes); //16비트로 프레임에 값을 표기할 경우 스왑을 해야한다.
            return bytes;
        }

        public static byte[] ToBytes(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] ToBytes(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] ToBytes(sbyte value)
        {
            byte[] bytes = new byte[] { (byte)value };
            return bytes;
        }

        public static byte[] ToBytes(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            Array.Reverse(bytes); //16비트로 프레임에 값을 표기할 경우 스왑을 해야한다.
            return bytes;
        }

        public static byte[] ToBytes(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] ToBytes(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] ToBytes(string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }

        public static byte[] ToBytes(object value)
        {
            return ToBytes(value, value.GetType());
        }

        public static byte[] ToBytes(object value, Type type)
        {
            if (value == null)
                return null;
            byte[] bytes = null;
            if (type == typeof(Boolean))
            {
                bool out_v;
                if (Boolean.TryParse(string.Format("{0}", value), out out_v))
                    bytes = ToBytes(out_v);
            }
            else if (type == typeof(SByte))
            {
                sbyte out_v;
                if (SByte.TryParse(string.Format("{0}", value), out out_v))
                    bytes = ToBytes(out_v);
            }
            else if (type == typeof(UInt16))
            {
                ushort out_v;
                if (UInt16.TryParse(string.Format("{0}", value), out out_v))
                    bytes = ToBytes(out_v);
            }
            else if (type == typeof(UInt32))
            {
                uint out_v;
                if (UInt32.TryParse(string.Format("{0}", value), out out_v))
                    bytes = ToBytes(out_v);
            }
            else if (type == typeof(UInt64))
            {
                ulong out_v;
                if (UInt64.TryParse(string.Format("{0}", value), out out_v))
                    bytes = ToBytes(out_v);
            }
            else if (type == typeof(Byte))
            {
                byte out_v;
                if (Byte.TryParse(string.Format("{0}", value), out out_v))
                    bytes = ToBytes(out_v);
            }
            else if (type == typeof(Int16))
            {
                short out_v;
                if (Int16.TryParse(string.Format("{0}", value), out out_v))
                    bytes = ToBytes(out_v);
            }
            else if (type == typeof(Int32))
            {
                int out_v;
                if (Int32.TryParse(string.Format("{0}", value), out out_v))
                    bytes = ToBytes(out_v);
            }
            else if (type == typeof(Int64))
            {
                long out_v;
                if (Int64.TryParse(string.Format("{0}", value), out out_v))
                    bytes = ToBytes(out_v);
            }
            else if (type == typeof(String) && typeof(String) == value.GetType())
            {
                bytes = Encoding.ASCII.GetBytes((string)value);
            }
            return bytes;
        }
    }
}