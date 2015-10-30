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
        public static object ToValue(Type type, params byte[] bytes)
        {
            if (type == typeof(Int16) || type == typeof(UInt16))
                Array.Reverse(bytes);
            object target = null;
            if (type != typeof(String))
                target = BinaryFormatTranslator.BinaryToInteger(type, bytes);
            else
                target = StringFormatTranslator.ByteArrayToString(bytes);
            return target;
        }

        public static TOutput ToValue<TOutput>(params byte[] bytes)
        {
            return (TOutput)ToValue(typeof(TOutput), bytes);
        }

        public static byte[] ToCode(Type type, object value)        
        {
            byte[] bytes = null;
            if (type != typeof(String))
                bytes = BinaryFormatTranslator.IntegerToBinary(type, value);
            else
                bytes = StringFormatTranslator.StringToByteArray(value as string);
            if (type == typeof(Int16) || type == typeof(UInt16))
                Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] ToASCII<TOutput>(object value)
        {
            return ToCode(typeof(TOutput), value);
        }
    }
}