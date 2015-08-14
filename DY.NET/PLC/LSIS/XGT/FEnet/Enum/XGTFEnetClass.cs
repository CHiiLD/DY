namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 헤더 구조(Application Header Format) - PLC Info 
    /// XGT FEnet - 이중화 마스터/단독 or 이중화 스레이브를 표시한다. 
    /// 
    /// 사용설명서_XGT FEnet_국문_V2.0(8.1.2 프레임 구조)
    /// </summary>
    public enum XGTFEnetClass
    {
        MASTER = 0x00, // 00000000
        SLAVE = 0x02   // 00000010
    }
}