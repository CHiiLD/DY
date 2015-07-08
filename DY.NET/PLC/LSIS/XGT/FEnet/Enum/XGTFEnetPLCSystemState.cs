﻿namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 프레임 구조 -> 헤더 구조 -> PLC Info -> Bit (08-12) 
    /// 시스템 상태
    /// </summary>
    public enum XGTFEnetPLCSystemState
    {
        RUN = 0x01,     //작동 
        STOP = 0x02,    //정지 
        PAUSE = 0x04,   //일시정지 
        DEBUG = 0x08    //디버그   
    }
}