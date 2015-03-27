/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜의 Error 응답 값들의 열거 클래스
 * 첨부: Cnet 전용 통신에 사용되는 컨트롤 코드의 타입 열거 클래스
 * 날짜: 2015-03-25
 */

namespace DY.NET.LSIS.XGT
{
    public enum XGTCnetCommandType : ushort
    {
        SS = 0x5353,    //개별 읽기/쓰기
        SB = 0x5342     //연속 읽기/쓰기
    }
}
