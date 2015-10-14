using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public interface IQHeader
    {
        /// <summary>
        /// MELSECNET/H, MELSECNET/10 네트워크 시스템의 네트워크 번호
        ///                                                  | 네트워크번호 |
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// Q시리즈E71장착국                                  |    0x00     |
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// MELSECNET/H,MELSECNET/10상의 관리국               |             |
        /// (Q시리즈E71장착국을 일반국에 장착시)               |             |
        ///                                                  |             |
        /// MELSECNET/H상의 리모트 마스터 국                  |     0x01~   |
        /// (Q시리즈E71장착국을 리모트IO/국에 장착시)          |     0xEF    |
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|             |
        /// MELSECNET/H,MELSECNET/10상의                     |             |  
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ |
        /// '엑서스 시의 유효모듈'설정의 네트워크 모듈 경유국  |    0xFE     | 
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
        /// </summary>
         byte NetworkNumber { get; set; }

        /// <summary>
        /// MELSECNET/H, MELSECNET/10 네트워크 시스템의 국번호
        ///                                                  |          PLC번호            |
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|
        /// Q시리즈E71장착국                                  |    0xFF                     |
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|
        /// MELSECNET/H,MELSECNET/10상의 관리국               |                            |
        /// (Q시리즈E71장착국을 일반국에 장착시)               |   0x7D:지정 관리국/마스터국 | 
        ///                                                  |   0x7E:현재 관리국/마스터국  |
        /// MELSECNET/H상의 리모트 마스터 국                  |                             |
        /// (Q시리즈E71장착국을 리모트IO/국에 장착시)          |                             |
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|  ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ |
        /// MELSECNET/H,MELSECNET/10상의                     |                             |
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|  0x01 ~ 0x40                |
        /// '엑서스 시의 유효모듈'설정의 네트워크 모듈 경유국  |                            |
        /// ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|
        /// </summary>
         byte PLCNumber { get; set; }

        /// <summary>
        /// 요구상대 모듈I/O번호
        /// 
        /// 멀티시피유 시스템의 PLC시피유일 경우 지정(Q시리즈C24 등에 의한 멀티드롭 접속상의 PLC CPU지정방법은 QnA호환 C4프레임을 사용한 것과 같음 3.1.6항 참조
        /// 하고 그외에는 아래와 같이 표기
        /// 요구상대 모듈I/O번호 -> 0x03FF, 요구상대모듈 국번호 -> 0x00
        /// </summary>
         ushort ModuleIONumber { get; set; }

        /// <summary>
        /// 요구상대모듈 국번호
        /// 
        /// 멀티시피유 시스템의 PLC시피유일 경우 지정(Q시리즈C24 등에 의한 멀티드롭 접속상의 PLC CPU지정방법은 QnA호환 C4프레임을 사용한 것과 같음 3.1.6항 참조
        /// 하고 그외에는 아래와 같이 표기
        /// 요구상대 모듈I/O번호 -> 0x03FF, 요구상대모듈 국번호 -> 0x00
        /// </summary>
         byte ModuleLocalNumber { get; set; }

        /// <summary>
        /// 요구데이터길이
        /// 
        /// REQT: 시피유 감시 타이머 ~ 요구 캐릭터 부(끝)까지의 데이터 길이
        /// RESP: 종료코드 ~ 응답 데이터 부(끝) 혹은 에러정보부까지의 데이터 길이
        /// * 예시
        /// ASCII  24byte => 0x18(HEX) => 0x30, 0x30, 0x31, 0x38 (HL)
        /// BINARY 12byte => 0x0C(HEX) => 0x0C, 0x00 (LH)
        /// </summary>
         ushort DataLength { get; set; }

        /// <summary>
        /// 시피유 감시 타이머
        /// 무한대기: 0x000
        /// 대기시간: 0x001 ~ 0xFFFF(단위 250ms)
        /// 
        /// 설정 범위 | 교신상대
        /// 1~40     | 자국
        /// 2~240    | MELSECNET/H,MELSECNET/10경유의 타국 또는 라이터 중계에 의한 타국
        /// </summary>
        ushort CPUMonitorTimer { get; set; }

        /// <summary>
        /// 종료코드 또는 에러코드
        /// 
        /// 응답 프로토콜 처리결과의 산출
        /// 정상종료 시에는 0이 산출되며 이상종료 시에는 에러코드가 산출된다. 사용자 매뉴얼(기본편) 11장 참조
        /// 에러정보부에선 에러응답을 한 네트워크, PLC번호와 에러발생시의 커맨드, 서브커맨드 등이 산출된다.
        /// </summary>
        MCEthernetError Error { get; set; }
    }
}