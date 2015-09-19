namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet - 프로토콜 프레임 헤더
    /// 사용설명서_XGT_Cnet_국문_V2.8(7.2.3 직접변수 개별쓰기(W(w)SS))
    /// </summary>
    public enum XGTCnetControlChar : byte
    {
        NONE = 0x00,
        ENQ = 0x05, //요청 시작 코드
        ACK = 0x06, //ACK 응답 프레임 시작 코드
        NAK = 0x15, //NAK 응답 프레임 시작 코드
        EOT = 0x04, //요청 마감 코드
        ETX = 0x03, //응답 프레임 마감 코드
    }

    public static class XGTCnetASCIISignalExtension
    {
        /// <summary>
        /// XGTCnetCCType 변수를 바이트로 변환한다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte ToByte(this XGTCnetControlChar type)
        {
            return (byte)type;
        }
    }
}