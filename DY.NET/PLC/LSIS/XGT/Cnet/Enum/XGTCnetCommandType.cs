using System;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet - 명령어 타입 프레임
    /// 사용설명서_XGT_Cnet_국문_V2.8(7.2.2 명령어 일람)
    /// </summary>
    public enum XGTCnetCmdType : ushort
    {
        /// <summary>
        /// 개별
        /// </summary>
        SS = 0x5353,  
        /// <summary>
        /// 연속
        /// </summary>
        SB = 0x5342   
    }

    public static class XGTCnetCommandTypeExtensions
    {
        /// <summary>
        /// XGTCnetCmdType 변수의 이름을 반환한다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToString(this XGTCnetCmdType type)
        {
            string s = null;
            switch(type)
            {
                case XGTCnetCmdType.SS:
                    s = "SS";
                    break;
                case XGTCnetCmdType.SB:
                    s = "SB";
                    break;
            }
            return s;
        }

        /// <summary>
        /// byte 배열을 XGTCnetCmdType 변수로 반환한다.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static XGTCnetCmdType ToCmdType(byte[] data)
        {
            if (data.Length != 2)
                throw new ArgumentException("It's not XGTCnetCommandType's binary data");
            if (data[0] != 'S')
                throw new ArgumentException("Prefix is not 'S'");
            if (data[1] == 'S')
                return XGTCnetCmdType.SS;
            else if (data[1] == 'B')
                return XGTCnetCmdType.SB;
            else
                throw new Exception("It's not XGTCnetCommandType's binary data");
        }

        /// <summary>
        /// XGTCnetCmdType 변수를 byte 배열로 반환한다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this XGTCnetCmdType type)
        {
            byte[] b = new byte[2];
            switch (type)
            {
                case XGTCnetCmdType.SS:
                    b[0] = (byte)'S';
                    b[1] = (byte)'S';
                    break;
                case XGTCnetCmdType.SB:
                    b[0] = (byte)'S';
                    b[1] = (byte)'B';
                    break;
            }
            return b;
        }
    }
}
