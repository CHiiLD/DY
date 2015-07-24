using System;

namespace DY.NET
{
    /// <summary>
    /// 데이터 표현 방식
    /// </summary>
    public enum DataType
    {
        BIT,
        BYTE,
        WORD,
        DWORD,
        LWORD,

        BOOL,
        SBYTE,
        SHORT,
        INT,
        LONG
    }

    /// <summary>
    /// DataType 확장 클래스
    /// </summary>
    public static class DataTypeExtension
    {
        public static Type ToType(this DataType data_type)
        {
            Type ret = null;
            switch (data_type)
            {
                case DataType.BIT: ret = typeof(Boolean);
                    break;
                case DataType.BYTE: ret = typeof(Byte);
                    break;
                case DataType.WORD: ret = typeof(UInt16);
                    break;
                case DataType.DWORD: ret = typeof(UInt32);
                    break;
                case DataType.LWORD: ret = typeof(UInt64);
                    break;
                case DataType.BOOL: ret = typeof(Boolean);
                    break;
                case DataType.SBYTE: ret = typeof(SByte);
                    break;
                case DataType.SHORT: ret = typeof(Int16);
                    break;
                case DataType.INT: ret = typeof(Int32);
                    break;
                case DataType.LONG: ret = typeof(Int64);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return ret;
        }
    }
}