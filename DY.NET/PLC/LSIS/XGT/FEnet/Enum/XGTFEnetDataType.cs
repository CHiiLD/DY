using System;
namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 데이터 타입
    /// - 직접 변수를 읽거나 쓰고자 할 경우 명령어 타입으로 데이터 타입을 지정합니다.
    /// </summary>
    public enum XGTFEnetDataType : ushort
    {
        BIT = 0x00,
        BYTE = 0x01,
        WORD = 0x02,
        DWORD = 0x03,
        LWORD = 0x04,
        CONTINUATION = 0x14 //연속
    }

    /// <summary>
    /// XGTFEnetDataType 변수를 byte array로 변환
    /// </summary>
    public static class XGTFEnetDataTypeExtension
    {
        public static byte[] ToBytes(this XGTFEnetDataType type)
        {
            byte[] ret = new byte[2];
            ret[0] = 0x00;
            switch (type)
            {
                case XGTFEnetDataType.BIT:
                    ret[1] = (byte)XGTFEnetDataType.BIT;
                    break;
                case XGTFEnetDataType.BYTE:
                    ret[1] = (byte)XGTFEnetDataType.BYTE;
                    break;
                case XGTFEnetDataType.WORD:
                    ret[1] = (byte)XGTFEnetDataType.WORD;
                    break;
                case XGTFEnetDataType.DWORD:
                    ret[1] = (byte)XGTFEnetDataType.DWORD;
                    break;
                case XGTFEnetDataType.LWORD:
                    ret[1] = (byte)XGTFEnetDataType.LWORD;
                    break;
                case XGTFEnetDataType.CONTINUATION:
                    break;
            }
            return ret;
        }
    }

    /// <summary>
    /// Type 객체의 확장 클래스 
    /// 해당 타입을 XGTFEnetDataType 열거형으로 변환
    /// </summary>
    public static class TypeExtension4XGTFEnetDataType
    {
        public static XGTFEnetDataType ToXGTFEnetDataType(this Type type)
        {
            XGTFEnetDataType data_type = XGTFEnetDataType.BIT;
            if (type == typeof(Boolean))
                data_type = XGTFEnetDataType.BIT;
            else if (type == typeof(SByte) || type == typeof(Byte))
                data_type = XGTFEnetDataType.BYTE;
            else if (type == typeof(Int16) || type == typeof(UInt16))
                data_type = XGTFEnetDataType.WORD;
            else if (type == typeof(Int32) || type == typeof(UInt32))
                data_type = XGTFEnetDataType.DWORD;
            else if (type == typeof(Int64) || type == typeof(UInt64))
                data_type = XGTFEnetDataType.LWORD;
            return data_type;
        }
    }
}