using System;

namespace DY.NET
{
    /// <summary>
    /// 정수의 값을 가진 객체인지 검사
    /// </summary>
    public class NumericType
    {
        public const string ERROR_NOT_NEMERIC_TYPE = "DATA IS NOT NUMERIC TYPE";
        public static bool IsNumeric(object obj)
        {
            return IsNumeric(obj.GetType());
        }
        public static bool IsNumeric(Type type)
        {
            return     type == typeof(Boolean)
                    || type == typeof(Byte) || type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64)
                    || type == typeof(SByte) || type == typeof(UInt16) || type == typeof(UInt32) || type == typeof(UInt64);
        }
    }
}
