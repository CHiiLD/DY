namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet - 헤더 구조(Application Header Format) - PLC Info 
    /// PLC 동작 정상 / 에러
    /// 
    /// 사용설명서_XGT FEnet_국문_V2.0(8.1.2 프레임 구조)
    /// </summary>
    public enum XGTFEnetCpuState
    {
        CPU_NOR = 0x00, //정상 0000 0000
        CPU_ERR = 0x01  //에러 0000 0001 
    }
}
