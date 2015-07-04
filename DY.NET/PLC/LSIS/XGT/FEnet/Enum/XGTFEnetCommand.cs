namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 명령어 일람
    /// </summary>
    public enum XGTFEnetCommand : ushort
    {
        READ_REQT = 0x0054,     // 읽기 요구 명령어 코드
        READ_RESP = 0x0055,     // 읽기 응답 명령어 코드
        WRITE_REQT = 0x0058,    // 쓰기 응답 명령어 코드
        WRITE_RESP = 0x0059     // 쓰기 응답 명령어 코드
    }

    public static class XGTFEnetCommandExtension
    {
        public static byte[] ToBytes(this XGTFEnetCommand cmd)
        {
            byte[] ret = new byte[2];
            ret[0] = 0x00;
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