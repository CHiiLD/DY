namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 명령어 일람
    /// </summary>
    enum XGTFEnetCommand : ushort
    {
        READ_REQT = 0x0054,     // 읽기 요구 명령어 코드
        READ_RESP = 0x0055,     // 읽기 응답 명령어 코드
        WRITE_REQT = 0x0058,    // 쓰기 응답 명령어 코드
        WRITE_RESP = 0x0059     // 쓰기 응답 명령어 코드
    }
}