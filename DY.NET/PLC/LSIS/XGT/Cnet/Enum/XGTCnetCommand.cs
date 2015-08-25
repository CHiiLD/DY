namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet - 주 명령어 프레임
    /// 사용설명서_XGT_Cnet_국문_V2.8(7.2.2 명령어 일람)
    /// </summary>
    public enum XGTCnetCommand : byte
    {
        /// <summary>
        /// 읽기
        /// </summary>
        R = 0x52,
        /// <summary>
        /// 쓰기
        /// </summary>
        W = 0x57,
        /// <summary>
        /// 모니터링변수등록(사용안함)
        /// </summary>
        ///X = 0x58,
        /// <summary>
        /// 모니터 실행(사용안함)
        /// </summary>
        ///Y = 0x59,

        /// <summary>
        /// 읽기(사용안함)
        /// </summary>
        r = 0x72,
        /// <summary>
        /// 쓰기(사용안함)
        /// </summary>
        w = 0x77,
        /// <summary>
        /// 모니터링변수등록(사용안함)
        /// </summary>
        ///x = 0x78,
        /// <summary>
        /// 모니터 실행(사용안함)
        /// </summary>
        ///y = 0x79,
    }

    public static class XGTCnetCommandExtension
    {
        /// <summary>
        /// XGTCnetCommand 변수를 byte로 변환한다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static byte ToByte(this XGTCnetCommand cmd)
        {
            return (byte)cmd;
        }
    }
}
