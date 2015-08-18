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

        public static object ToValue(this DataType data_type, string str)
        {
            Type ret = null;
            object value = null;
            switch (data_type)
            {
                case DataType.BIT:
                case DataType.BOOL:
                    {
                        if (str == "0" || str == "False" || str == "FALSE" || str == "false")
                            value = false;
                        else if (str == "1" || str == "True" || str == "TRUE" || str == "true")
                            value = true;
                    }
                    break;
                case DataType.BYTE: ret = typeof(Byte);
                    {
                        byte out_v;
                        if (Byte.TryParse(string.Format("{0}", str), out out_v))
                            value = out_v;
                    }
                    break;
                case DataType.WORD:
                    {
                        ushort out_v;
                        if (UInt16.TryParse(string.Format("{0}", str), out out_v))
                            value = out_v;
                    }
                    break;
                case DataType.DWORD:
                    {
                        uint out_v;
                        if (UInt32.TryParse(string.Format("{0}", str), out out_v))
                            value = out_v;
                    }
                    break;
                case DataType.LWORD:
                    {
                        ulong out_v;
                        if (UInt64.TryParse(string.Format("{0}", str), out out_v))
                            value = out_v;
                    }
                    break;
                case DataType.SBYTE:
                    {
                        sbyte out_v;
                        if (SByte.TryParse(string.Format("{0}", str), out out_v))
                            value = out_v;
                    }
                    break;
                case DataType.SHORT:
                    {
                        short out_v;
                        if (Int16.TryParse(string.Format("{0}", str), out out_v))
                            value = out_v;
                    }
                    break;
                case DataType.INT:
                    {
                        int out_v;
                        if (Int32.TryParse(string.Format("{0}", str), out out_v))
                            value = out_v;
                    }
                    break;
                case DataType.LONG:
                    {
                        long out_v;
                        if (Int64.TryParse(string.Format("{0}", str), out out_v))
                            value = out_v;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return value;
        }
    }
}