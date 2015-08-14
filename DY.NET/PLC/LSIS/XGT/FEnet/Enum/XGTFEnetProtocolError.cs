namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// FEnet 에러코드(2 byte) 
    /// 실제 프레임 상에서 값을 확인할 경우 16진수 워드 데이터를 표현할 때 바이트 스왑이 발생하여 
    /// 상, 하 바이트가 바뀌어 표현된다 0x0054 -> 0x5400
    /// 
    /// 사용설명서_XGT FEnet_국문_V2.0(10.4 통신 에러코드)
    /// </summary>
    public enum XGTFEnetProtocolError : ushort
    {
        //사용자 정의 
        OK = 0x0000,          //정상
        EXCEPTION = 0xFFFF,   //판별할 수 없는 에러 

        //메뉴얼 정의
        BLOCK_OVER = 0x0001, //블록 수 초과 에러(개별 읽기/쓰기 요청시 블록 수가 16보다 큼
        DATA_TYPE = 0x0002,  //데이터 타입 에러(X,B,W,D,L)이 아닌 데이터 타입을 수신했음
        NOT_SERVICED_DEVICE_REQUEST = 0x0003, //서비스 되지 않는 디바시으를 요구한 경우(XGK: P,M,L,K,R  XGL I,Q,M..)
        VAR_DIVECE_TERRITORY_OVER = 0x0004,   //변수 요구 영역 초과 에러(각 디바이스별 지원하는 영역을 초과해서 요구한 경우)
        SS_BLOCK_1440BYTE_OVER = 0x0005, //한번에 최대 1400byte까지 읽거나 쓸 수 있는데 초과해서 요청한 경우 (개별 블록 사이즈)
        SIZE_BLOCK_1440BYTE_OVER = 0x0006, //한번에 최대 1400byte까지 읽거나 쓸 수 있는데 초과해서 요청한 경우 (블록별 총 사이즈)
        HEADER_COMPANY_ID = 0x0075, //전용 서비스에서 프레임 헤더의 선두 부분이 잘못된 경우(LSIS-XGT)
        HEADER_LENGTH = 0x0076, //전용 서비스에서 프레임 헤더의 길이가 잘못된 경우
        HEADER_CHECKSUM = 0x0077, //전용 서비스에서 프레임 헤더의 Checksum이 잘못된 경우
        INSTRUCTION_COMMAND = 0x0078, //전용 서비스에서 명령어가 잘못된 경우

        //메뉴얼에 실려있지 않아 직접 문의해서 알아낸 에러들 
        DEVICE_TYPE = 0x0010,
        ADDRESS_FORMAT = 0x0011,
        DATA_TYPE2 = 0x0012,
    }
}