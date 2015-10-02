namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet - 주 명령어 프레임
    /// 사용설명서_XGT_Cnet_국문_V2.8(7.2.2 명령어 일람)
    /// </summary>
    public enum XGTCnetCommand : byte
    {
        NONE = 0x00,
        R = 0x52,
        W = 0x57,
    }

    public static class XGTCnetCommandExtension
    {
        /// <summary>
        /// XGTCnetCommand를 byte로 변환한다.
        /// </summary>
        public static byte ToByte(this XGTCnetCommand cmd)
        {
            return (byte)cmd;
        }
    }
}
