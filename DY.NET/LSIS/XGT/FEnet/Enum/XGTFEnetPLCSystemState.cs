namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 프레임 구조 -> 헤더 구조 -> PLC Info -> Bit (08-12) 
    /// 시스템 상태
    /// </summary>
    public enum XGTFEnetPLCSystemState
    {
        STOP = 0x02,    //정지
        RUN = 0x02,     //작동
        PAUSE = 0x02,   //일시정지
        DEBUG = 0x02    //디버그
    }
}