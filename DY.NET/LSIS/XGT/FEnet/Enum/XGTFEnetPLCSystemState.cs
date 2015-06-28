namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 프레임 구조 -> 헤더 구조 -> PLC Info -> Bit (08-12) 
    /// 시스템 상태
    /// </summary>
    public enum XGTFEnetPLCSystemState
    {
        STOP = 0x10,    //정지 00010 000 
        RUN = 0x20,     //작동 00100 000 
        PAUSE = 0x40,   //일시정지 01000 000
        DEBUG = 0x80    //디버그   10000 000
    }
}