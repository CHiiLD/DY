using System;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet - 명령어 타입 프레임
    /// 사용설명서_XGT_Cnet_국문_V2.8(7.2.2 명령어 일람)
    /// </summary>
    public enum XGTCnetCommandType : ushort
    {
        NONE = 0x0000,
        SS = 0x5353,
        SB = 0x5342
    }

    public static class XGTCnetCommandTypeExtension
    {
        public static byte[] ToBytes(this XGTCnetCommandType type)
        {
            byte[] result = null;
            switch (type)
            {
                case XGTCnetCommandType.SS:
                    result = new byte[] { 0x53, 0x53 }; break;
                case XGTCnetCommandType.SB:
                    result = new byte[] { 0x53, 0x42 }; break;
            }
            return result;
        }
    }
}
