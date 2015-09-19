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

    public static class XGTCnetCommandTypeExtensions
    {
        /// <summary>
        /// byte 배열을 XGTCnetCmdType 변수로 반환한다.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static XGTCnetCommandType ToCommandType(byte[] data)
        {
            if (data.Length != 2)
                throw new ArgumentException("It's not XGTCnetCommandType's binary data");
            if (data[0] != 'S')
                throw new ArgumentException("Prefix is not 'S'");

            if (data[1] == 'S')
                return XGTCnetCommandType.SS;
            else if (data[1] == 'B')
                return XGTCnetCommandType.SB;
            else
                throw new Exception("It's not XGTCnetCommandType's binary data");
        }

        /// <summary>
        /// XGTCnetCmdType 변수를 byte 배열로 반환한다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this XGTCnetCommandType type)
        {
            byte[] b = new byte[2];
            switch (type)
            {
                case XGTCnetCommandType.SS:
                    b[0] = (byte)'S';
                    b[1] = (byte)'S';
                    break;
                case XGTCnetCommandType.SB:
                    b[0] = (byte)'S';
                    b[1] = (byte)'B';
                    break;
            }
            return b;
        }
    }
}
