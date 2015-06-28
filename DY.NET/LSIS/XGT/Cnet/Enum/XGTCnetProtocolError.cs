/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜의 Error 응답 값들의 열거 클래스
 * 설명: Error Value는 아스키2코드로 4byte의 내용으로 에러의 종류를 표시합니다.
 * 날짜: 2015-03-25
 */

namespace DY.NET.LSIS.XGT
{
    public enum XGTCnetProtocolError : uint
    {
        //사용자 정의 
        OK = 0x00000000,                //정상
        CAT_NOT_READ_TAIL = 0xFFFFFFFE, //통신에서 TAIL까지 읽지 못했을 때      
        EXCEPTION = 0xFFFFFFFF,           //판별할 수 없는 에러 

        //메뉴얼 정의
        BLOCK_OVER = 0x00000003,        //블록 수 초과 에러(개별 읽기/쓰기 요청시 블록 수가 16보다 큼
        VAR_LENGTH = 0x00000004,        //변수 길이 에러(변수 길이가 최대 크기인 16보다 큼) 
        DATA_TYPE = 0x00000007,         //데이터 타입 에러(X,B,W,D,L)이 아닌 데이터 타입을 수신했음
        DATA = 0x00000011,              //데이터 에러(데이터 길이 영역 정보가 잘못된 경우, %로 시작하지 않은 경우, 변수의 영역 값이 잘못된 경우, Bit쓰기인 경우 반드시 00 or 01로 써야하는데 다른 값으로 쓴 경우)
        MONITOR_EXECUTE1 = 0x00000090,  //모니터 실행 에러(등록 안된 모니터 실행을 요구한 경우)
        MONITOR_EXECUTE2 = 0x00000190,  //모니터 실행 에러(등록 번호 범위를 초과한 경우)
        MONITOR_REGISTER = 0x00000290,  //모니터 등록 에러(등록 번호 범위를 초과한 경우)
        DEVICE_FRAME = 0x00001132, //사용하는 디바이스가 아닌 프레임 예를 입력할 경우
        DATA_SIZE_OVER = 0x00001232,    //데이터 크기 에러(한번에 최대 60Word까지 읽거나 쓸 수 있는데 초과해서 요청한 경우
        NO_NEED_FRAME = 0x00001234,     //여유 프레임 에러(필요 없는 내용이 추가로 존재하는 경우)
        DATA_TYPE_DISCARD = 0x00001332, //데이터 타입 불일치 에러(개별 읽기/쓰기인 경우, 모든 블록은 동일한 데이터 타입 대응하여야 함)
        CAN_NOT_CONVERT_TO_HEX = 0x00001432, //데이터 값 에러(데이터 값이 hex 변환 불가능한 경우) 
        VAR_DIVECE_TERRITORY_OVER = 0x00007132   //변수 요구 영역 초과 에러(각 디바이스별 지원하는 영역을 초과해서 요구한 경우)
    }
}