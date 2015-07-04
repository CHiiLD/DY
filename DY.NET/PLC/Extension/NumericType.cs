using System;

namespace DY.NET
{
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
