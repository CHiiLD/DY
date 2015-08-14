using System;
namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 데이터 타입
    /// 사용설명서_XGT FEnet_국문_V2.0(8.1.4 데이터 타입)
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

    public static class XGTFEnetDataTypeExtension
    {
        /// <summary>
        /// XGTFEnetDataType 변수를 byte[]로 반환한다.
        /// </summary>
        /// <param name="type">XGTFEnetDataType</param>
        /// <returns>2byte</returns>
        public static byte[] ToBytes(this XGTFEnetDataType type)
        {
            byte[] data_type = new byte[2];
            data_type[0] = 0x00;
            switch (type)
            {
                case XGTFEnetDataType.BIT:
                    data_type[1] = (byte)XGTFEnetDataType.BIT;
                    break;
                case XGTFEnetDataType.BYTE:
                    data_type[1] = (byte)XGTFEnetDataType.BYTE;
                    break;
                case XGTFEnetDataType.WORD:
                    data_type[1] = (byte)XGTFEnetDataType.WORD;
                    break;
                case XGTFEnetDataType.DWORD:
                    data_type[1] = (byte)XGTFEnetDataType.DWORD;
                    break;
                case XGTFEnetDataType.LWORD:
                    data_type[1] = (byte)XGTFEnetDataType.LWORD;
                    break;
                case XGTFEnetDataType.CONTINUATION:
                    break;
            }
            return data_type;
        }
    }

    /// <summary>
    /// Type 객체의 확장 클래스 
    /// </summary>
    public static class TypeExtension4XGTFEnetDataType
    {
        /// <summary>
        /// Type 객체를 XGTFEnetDataType로 변환한다.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>XGTFEnetDataType</returns>
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