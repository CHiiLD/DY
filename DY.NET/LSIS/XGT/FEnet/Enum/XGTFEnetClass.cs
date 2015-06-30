namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// PLC INFO
    /// 이중화 마스터/단독 or 이중화 스레이브를 표시합니다. 
    /// </summary>
    public enum XGTFEnetClass
    {
        MASTER = 0x00, // 00000000
        SLAVE = 0x02   // 00000010
    }
}