namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// PLC INFO
    /// PLC 동작 정상 / 에러
    /// </summary>
    public enum XGTFEnetCpuState
    {
        CPU_NOR = 0x00, //정상 0000 0000
        CPU_ERR = 0x01  //에러 0000 0001 
    }
}
