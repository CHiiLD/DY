namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet - 명령어
    /// 사용설명서_XGT FEnet_국문_V2.0(8.1.3 명령어 일람)
    /// </summary>
    public enum XGTFEnetCommand : ushort
    {
        NONE = 0x0000,
        READ_REQT = 0x0054,     // 읽기 요구 명령어 코드
        READ_RESP = 0x0055,     // 읽기 응답 명령어 코드
        WRITE_REQT = 0x0058,    // 쓰기 응답 명령어 코드
        WRITE_RESP = 0x0059     // 쓰기 응답 명령어 코드
    }

    public static class XGTFEnetCommandExtension
    {
        /// <summary>
        /// XGTFEnetCommand 변수를 byte[]로 변환한다.
        /// </summary>
        /// <param name="cmd">XGTFEnetCommand</param>
        /// <returns>2byte</returns>
        public static byte[] ToBytes(this XGTFEnetCommand cmd)
        {
            byte[] ret = new byte[2] { 0x00, 0x00 };
            switch (cmd)
            {
                case XGTFEnetCommand.READ_REQT:
                    ret[1] = 0x54;
                    break;
                case XGTFEnetCommand.READ_RESP:
                    ret[1] = 0x55;
                    break;
                case XGTFEnetCommand.WRITE_REQT:
                    ret[1] = 0x58;
                    break;
                case XGTFEnetCommand.WRITE_RESP:
                    ret[1] = 0x59;
                    break;
            }
            return ret;
        }
    }
}