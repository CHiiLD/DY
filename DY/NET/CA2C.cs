/*
 * 작성자: CHILD	
 * 설명: ASC2 데이터를 Type형 변수로, Type형 변수를 ASC2데이터로 서로 변환하는 클래스
 * 날짜: 2015-03-31
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public class CA2C //Convert Asc2 code
    {
        protected CA2C()
        {

        }

        public static byte[] ToASC(object value, Type type)
        {
            byte[] target = null;
            string hex_str;

            if (type == typeof(Byte) || type == typeof(SByte))
                hex_str = string.Format("{0:X}", value);
            else if (type == typeof(Int16) || type == typeof(UInt16))
                hex_str = string.Format("{0:X2}", value);
            else if (type == typeof(Int32) || type == typeof(UInt32))
                hex_str = string.Format("{0:X4}", value);
            else if (type == typeof(Int64) || type == typeof(UInt64))
                hex_str = string.Format("{0:X8}", value);
            else if (type == typeof(string))
                hex_str = (string)value;
            else
                throw new ArgumentException("argument is not integer or string type");

            target = new byte[hex_str.Length];
            for (int i = 0; i < hex_str.Length; i++)
                target[i] = (byte)hex_str[i];

            return target;
        }

        public static byte[] ToASC(object value)
        {
            return ToASC(value, value.GetType());
        }

        public static object ToValue(byte[] value, Type type)
        {
            object target = null;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in value)
                sb.Append(Convert.ToChar(b));
            string hex_str = sb.ToString();

            if (type == typeof(Byte))
                target = Convert.ToByte(hex_str, 16);
            else if (type == typeof(Int16))
                target = Convert.ToInt16(hex_str, 16);
            else if (type == typeof(Int32))
                target = Convert.ToInt32(hex_str, 16);
            else if (type == typeof(Int64))
                target = Convert.ToInt64(hex_str, 16);
            else if (type == typeof(SByte))
                target = Convert.ToSByte(hex_str, 16);
            else if (type == typeof(UInt16))
                target = Convert.ToUInt16(hex_str, 16);
            else if (type == typeof(UInt32))
                target = Convert.ToUInt32(hex_str, 16);
            else if (type == typeof(UInt64))
                target = Convert.ToUInt64(hex_str, 16);
            else if (type == typeof(string))
                target = hex_str;
            else
                throw new ArgumentException("argument is not integer or string type");
            return target;
        }
    }
}