using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public enum MCEFrameError : ushort
    {
        /* 축약어
         * GXDeveloper -> GXD 
         */
        OK = 0x0000, //정상
        RW_DEVICE_TERRITORY = 0x0002, //읽기,쓰기 디바이스 범위의 지정에 잘못이 있는 경우
        SUBHEADER = 0x0050, //서브헤더의 커맨드/응답 종류가 규정 의외의 코드로 된 경우
        /// <summary>
        /// 랜덤 엑서스용 버퍼의 의한 교신에서 상대 기기로부터 지정 선두 어드레스 + 데이터 워드수가 0 ~ 6143의 범위를 넘을 경우
        /// 지정된 워드수만큼의 데이터(텍스트)를 1프레임으로 송신할 수 없는 경우
        /// </summary>
        DATA_FRAME = 0x0052,
        CAN_NOT_READ_ASCII = 0x0054, //GXD의 설정이 바이너리 읽기로 설정되었는데 ASCII코드로 송신되었을 경우
        /// <summary>
        /// GXD의 Enable Write at Runtime이 미허가일 때 write protocol을 송신하여 거부할 경우
        /// PLC가 RUN상태 중에 파라미터, 시퀸스프로그램, 마이컴 프로그램의 쓰기를 요구한 경우
        /// </summary>
        CAT_NOT_ACCESS = 0x0055, 
        INVALID_DEVICE = 0x0056, //PLC로부터 디바이스 지정에 잘못이 있는 경우
        /// <summary>
        /// 커맨드 점수 지정이 최대 점수 처리를 넘고 있는 경우 
        /// 커맨드 바이트 길이가 규정 외인 경우 
        /// 데이터 쓰기 시에 설정한 쓰기 데이터 점수가 점수 지정값과 다를 경우
        /// 모니터 데이터 등록을 실행치 아니하였는데 모니터 요구를 하는 경우
        /// 마이컴 프로그램 읽기/쓰기에 대해 파리미터 설정 범위의 최종 어드레스 이후를 지정하였을 경우
        /// 확장 파일 레지스터의 블록 넘버 지정에 대해 해당 메모리 카세트 용량을 넘는 범위의 블록 넘버를 지정하였을 경우
        /// </summary>
        COMPLREX0 = 0x0057,
        /// <summary>
        /// 상대 기기로부터 커맨드 선두 어드레스 지정이, 지정 가능한 범위를 초과한 경우
        /// 마이컴 프로그램, 파일 레지스터의 읽기/쓰기에 대해 PLC CPU 파라미터 설정 범위를 초과한 경우
        /// 확장 파일 레지스터의 블록 넘버 지정이 존재하지 않는 블록으로 한 경우
        /// 비트 디바이스용 커맨드에 대해 워드 디바이스로 지정한 경우
        /// 워드 디바이스용 커맨드에 대해 비트 디바이스의 선두 번호를 16배수 이외의 값으로 지정한 경우
        /// </summary>
        COMPLREX1 = 0x0058,
        CAN_NOT_DESIGNATE_EXTENSION_FILE_REGISTER = 0x0059,  //확장 파일 레지스터를 지정할 수 없는 경우
        /// <summary>
        /// 시피유와 이더넷 모듈이 서로 교신할 수 없는 경우
        /// 상대 기기로부터의 요구에 PLC CPU가 처리할 수 없는 경우
        /// </summary>
        CAT_NOT_COMMUNICATE_EACH_OTHER = 0x005B, 
        TIMEOVER = 0x0060, //이더넷 모듈과 PLC와의 교신 시간이 CPU감시 타이머값을 초과한 경우
        LOCK_ETHERNET_MODULE_PORT = 0x0063, //고정 버퍼 교신 시에 교신 상대 이더넷 모듈의 포트가 리모트 패스워드의 잠금 상태가 되어있다.
    }
}
