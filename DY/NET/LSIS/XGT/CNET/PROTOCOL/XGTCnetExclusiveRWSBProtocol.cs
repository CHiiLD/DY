/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜 프레임에서 구조화된 데이타까지 포함하여 클래스 구현 (직접 변수 연속 읽기/쓰기)
 * * 주석: WSB에선 ACK응답에 구조화된 데이터를 보내지 않습니다. Write이니 보낼 필요가 없는 것입니다. 따라서 WSB일 땐, RecvData 변수를 사용하지 않습니다.
 * 날짜: 2015-03-25
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    //주의! Bit디바이스는 연속 읽기가 지원되지 않습니다.
    public class XGTCnetExclusiveRWSBProtocol : XGTCnetExclusiveProtocolFrame
    {
        public XGTCnetENQDataFormat ReqtData;
        public XGTCnetACKDataFormat RecvData;
    }
}
