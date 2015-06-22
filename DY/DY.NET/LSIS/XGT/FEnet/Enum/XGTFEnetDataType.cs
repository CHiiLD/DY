namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 데이터 타입
    /// - 직접 변수를 읽거나 쓰고자 할 경우 명령어 타입으로 데이터 타입을 지정합니다.
    /// </summary>
    enum XGTFEnetDataType
    {
        BIT = 0x00,
        BYTE = 0x01,
        WORD = 0x02,
        DWORD = 0x03,
        LWORD = 0x04,
        CONTINUATION = 0x14 //연속
    }
}