using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public class MC3EProtocol : IProtocol
    {
        /// <summary>
        /// 서브헤더
        ///            |          RESP             |          RESP
        /// ASCII      | 0x35, 0x30,  0x30,  0x30  | 0x44, 0x30, 0x30, 0x30
        /// BINARY     | 0x50, 0x00                | 0xD0, 0x00
        /// BINARY인 경우 HL순 LH가 아님에 주의
        /// </summary>
        public MC3ESubHeader SubHeader { get; set; }
        
        /// <summary>
        /// 네트워크번호
        ///                                                  | 네트워크번호 |  PLC번호
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// Q시리즈E71장착국                                  |    0x00     |   0xFF
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// MELSECNET/H,MELSECNET/10상의 관리국               |             |
        /// (Q시리즈E71장착국을 일반국에 장착시)               |             |
        ///                                                  |             |
        /// MELSECNET/H상의 리모트 마스터 국                  |     0x01~   |  0x7D:지정 관리국/마스터국
        /// (Q시리즈E71장착국을 리모트IO/국에 장착시)          |     0xEF    |  0x7E:현재 관리국/마스터국
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|             | ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// MELSECNET/H,MELSECNET/10상의                     |             |  
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ |  0x01 ~ 0x40
        /// '엑서스 시의 유효모듈'>설정의 네트워크 모듈 경유국  |    0xFE     | 
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// </summary>
        public byte NetworkNumber { get; set; }

        /// <summary>
        /// PLC번호
        ///                                                  | 네트워크번호 |  PLC번호
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// Q시리즈E71장착국                                  |    0x00     |   0xFF
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// MELSECNET/H,MELSECNET/10상의 관리국               |             |
        /// (Q시리즈E71장착국을 일반국에 장착시)               |             |
        ///                                                  |             |
        /// MELSECNET/H상의 리모트 마스터 국                  |     0x01~   |  0x7D:지정 관리국/마스터국
        /// (Q시리즈E71장착국을 리모트IO/국에 장착시)          |     0xEF    |  0x7E:현재 관리국/마스터국
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|             | ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// MELSECNET/H,MELSECNET/10상의                     |             |  
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ |  0x01 ~ 0x40
        /// '엑서스 시의 유효모듈'>설정의 네트워크 모듈 경유국  |    0xFE     | 
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// </summary>
        public byte PLCNumber { get; set; }
        
        /// <summary>
        /// 요구상대 모듈I/O번호
        /// 
        /// 멀티시피유 시스템의 PLC시피유일 경우 지정(Q시리즈C24 등에 의한 멀티드롭 접속상의 PLC CPU지정방법은 QnA호환 C4프레임을 사용한 것과 같음 3.1.6항 참조
        /// 하고 그외에는 아래와 같이 표기
        /// 요구상대 모듈I/O번호 -> 0x03FF, 요구상대모듈 국번호 -> 0x00
        /// </summary>
        public ushort TargetModuleIONumber { get; set; } 

        /// <summary>
        /// 요구상대모듈 국번호
        /// 
        /// 멀티시피유 시스템의 PLC시피유일 경우 지정(Q시리즈C24 등에 의한 멀티드롭 접속상의 PLC CPU지정방법은 QnA호환 C4프레임을 사용한 것과 같음 3.1.6항 참조
        /// 하고 그외에는 아래와 같이 표기
        /// 요구상대 모듈I/O번호 -> 0x03FF, 요구상대모듈 국번호 -> 0x00
        /// </summary>
        public byte TargetModuleLocalNumber { get; set; }
        
        /// <summary>
        /// 요구데이터길이
        /// 
        /// REQT: 시피유 감시 타이머 ~ 요구 캐릭터 부(끝)까지의 데이터 길이
        /// RESP: 종료코드 ~ 응답 데이터 부(끝) 혹은 에러정보부까지의 데이터 길이
        /// * 예시
        /// ASCII  24byte => 0x18(HEX) => 0x30, 0x30, 0x31, 0x38 (HL)
        /// BINARY 12byte => 0x0C(HEX) => 0x0C, 0x00 (LH)
        /// </summary>
        public ushort DataLength { get; set; } 

        /// <summary>
        /// 시피유 감시 타이머
        /// 무한대기: 0x000
        /// 대기시간: 0x001 ~ 0xFFFF(단위 250ms)
        /// 
        /// 설정 범위 | 교신상대
        /// 1~40     | 자국
        /// 2~240    | MELSECNET/H,MELSECNET/10경유의 타국 또는 라이터 중계에 의한 타국
        /// </summary>
        public ushort CPUMonitorTimer { get; set; }
        
        /* * * * * * * * * * * * * * * * * * * * * * 
         
         * * * * * * * * * * * * * * * * * * * * * */
        /// <summary>
        /// 커맨드
        /// 
        /// |     | 비트 | 워드 |
        /// |읽기 | 0401 | 0401 |
        /// |쓰기 | 1401 | 1401 |
        /// </summary>
        public MC3ECommand Command { get; set; }

        /// <summary>
        /// 서브커맨드
        /// 
        /// |     | 비트 | 워드 |
        /// |읽기 | 00?1 | 00?0 |
        /// |쓰기 | 00?1 | 00?0 |
        /// ASCII   0x000 또는 아래에 의한 수치를 ASCII코드 4자리(16진)로 변환하여 사용하고 상위자리부터 송신
        /// BINARY  0x000 또는 아래에 의한 2byte의 수치를 사용하여 송신
        /// BIT INDEX 15 14 13 12 11 10 09 08 07  06  05 04 03 02 01 00
        ///           0  0  0  0  0  0  0  0  1/0 1/0 0  0  0  0  0  1/0
        ///                                                          B0: 0 => 워드사용, 단위를 지정치 아니함 
        ///                                                              1 => 비트단위사용
        ///                                       B6: 0 => 랜덤읽기, 모니터데이터등록 이외의 기능 사용 
        ///                                           1 => 랜덤읽기, 모니터데이터등록 기능 사용
        ///                                   B07: 0 => 디바이스메모리 확장 지정 없음
        ///                                        1 => 디바이스 메모리 확장 지정 있음 (Q, QnACPU국만 지정 가능)
        /// </summary>
        ///public ushort SubCommand { get; set; }

        public MC3EDeviceMemoryExtension DeviceMemoryExtension { get; set; }
        public MC3ESpecialFunction SpecialFunction { get; set; }
        public MC3EDataType? DataType { get; set; }

        /// <summary>
        /// 종료코드 또는 에러코드
        /// 
        /// 응답 프로토콜 처리결과의 산출
        /// 정상종료 시에는 0이 산출되며 이상종료 시에는 에러코드가 산출된다. 사용자 매뉴얼(기본편) 11장 참조
        /// 에러정보부에선 에러응답을 한 네트워크, PLC번호와 에러발생시의 커맨드, 서브커맨드 등이 산출된다.
        /// </summary>
        public MCEFrameError Error { get; set; }
        public byte ErrorNetworkNumber { get; set; }
        public byte ErrorPLCNumber { get; set; }
        public ushort ErrorTargetModuleIONumber { get; set; }
        public byte ErrorTargetModuleLocalNumber { get; set; }
        public MC3ECommand ErrorCommand { get; set; }
        public MC3EDeviceMemoryExtension ErrorDeviceMemoryExtension { get; set; }
        public MC3ESpecialFunction ErrorSpecialFunction { get; set; }
        public MC3EDataType? ErrorDataType { get; set; }

        //DATA INFORMATION
        public IList<IProtocolData> Data { get; set; }
        public Type Type { get; set; }

        public void Initialize()
        {
            SubHeader = MC3ESubHeader.NONE;
            NetworkNumber = byte.MaxValue;
            PLCNumber = byte.MaxValue;
            TargetModuleIONumber = ushort.MaxValue;
            TargetModuleLocalNumber = byte.MaxValue;
            DataLength = ushort.MaxValue;
            CPUMonitorTimer = 0;
            Command = MC3ECommand.NONE;
            DeviceMemoryExtension = MC3EDeviceMemoryExtension.OFF;
            SpecialFunction = MC3ESpecialFunction.OFF;
            DataType = null;

            Error = MCEFrameError.OK;
            ErrorNetworkNumber = 0;
            ErrorPLCNumber = 0;
            ErrorTargetModuleIONumber = 0;
            ErrorTargetModuleLocalNumber = 0;

            ErrorDeviceMemoryExtension = MC3EDeviceMemoryExtension.OFF;
            ErrorSpecialFunction = MC3ESpecialFunction.OFF;
            ErrorDataType = null;

            Data = null;
            Type = null;
        }

        public int GetErrorCode()
        {
            return (int)Error;
        }
    }
}