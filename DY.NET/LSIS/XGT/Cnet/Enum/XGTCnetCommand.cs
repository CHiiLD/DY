/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜의 Error 응답 값들의 열거 클래스
 * 첨부: Cnet 전용 통신에 사용되는 메인 컨트롤 코드의 타입 열거 클래스
 * 날짜: 2015-03-25
 */

namespace DY.NET.LSIS.XGT
{
    public enum XGTCnetCommand : byte
    {
        //디바이스 개별/연속 읽기
        r = 0x72,
        w = 0x77,
        R = 0x52,
        W = 0x57,
        //모니터 변수등록/실행
        x = 0x78,
        y = 0x79,
        X = 0x58,
        Y = 0x59
    }

    public static class XGTCnetCommandExtension
    {
        public static byte ToByte(this XGTCnetCommand cmd)
        {
            return (byte)cmd;
        }
    }
}
