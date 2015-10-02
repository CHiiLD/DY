namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet - 헤더 구조(Application Header Format) - PLC Info 
    /// LSIS XGT 시피유 종류
    /// 
    /// 사용설명서_XGT FEnet_국문_V2.0(8.1.2 프레임 구조)
    /// </summary>
    public enum XGTFEnetCpuType : byte
    {
        NONE = 0x00,
        XGK_CPUH = 0x04, // 00000100
        XGK_CPUS = 0x08, // 00001000
        XGL_CPUU = 0x14, // 00010100
    }

    public static class XGTFEnetCpuTypeExtension
    {
        public static byte ToByte(this XGTFEnetCpuType type)
        {
            return (byte)type;
        }
    }
}