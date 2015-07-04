/*
 * 작성자: CHILD	
 * 기능: Cnet 전용 통신에 사용되는 프레임의 컨트롤 코드의 타입 열거 클래스
 * 날짜: 2015-03-25
 */

namespace DY.NET.LSIS.XGT
{
    public enum XGTCnetCCType : byte
    {
        ENQ = 0x05, //요청 시작 코드
        ACK = 0x06, //ACK 응답 프레임 시작 코드
        NAK = 0x15, //NAK 응답 프레임 시작 코드
        EOT = 0x04, //요청 마감 코드
        ETX = 0x03, //응답 프레임 마감 코드
    }

    public static class XGTCnetCCTypeExtension
    {
        public static byte ToByte(this XGTCnetCCType type)
        {
            return (byte)type;
        }
    }
}