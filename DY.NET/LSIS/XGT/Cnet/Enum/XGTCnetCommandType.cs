/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜의 Error 응답 값들의 열거 클래스
 * 설명: Cnet 전용 통신에 사용되는 컨트롤 코드의 타입 열거 클래스
 * 날짜: 2015-03-25
 */
using System;

namespace DY.NET.LSIS.XGT
{
    public enum XGTCnetCmdType : ushort
    {
        SS = 0x5353,    //개별 읽기/쓰기
        SB = 0x5342     //연속 읽기/쓰기
    }

    /// <summary>
    /// XGTCnetCommandType 열거형의 확장메서드를 위한 클래스
    /// </summary>
    public static class XGTCnetCommandTypeExtensions
    {
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

        public static XGTCnetCmdType ToCmdType(byte[] data)
        {
            if(data.Length != 2)
                throw new ArgumentException("it's not XGTCnetCommandType's binary data");
            if(data[0] != 'S')
                throw new ArgumentException("prefix is not 'S'");
            if (data[1] == 'S')
                return XGTCnetCmdType.SS;
            else
                return XGTCnetCmdType.SB;
        }

        public static byte[] ToByteArray(this XGTCnetCmdType type)
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
