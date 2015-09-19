using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTFEnetProtocol : IProtocol
    {
        public XGTFEnetCompanyID CompanyID { get; protected set; }            //PLC 제품

        public XGTFEnetCpuType CpuType { get; protected set; } //bit00-05 시피유 타입
        public XGTFEnetClass Class { get; protected set; } // 7bit
        public XGTFEnetCpuState CpuState { get; protected set; } //8bit 
        public XGTFEnetPLCSystemState PLCState { get; protected set; } //bit08-12 시스템 상태

        public XGTFEnetSourceOfFrame SourceOfFrame { get; protected set; }    //클라 -> 서버 or 서버 -> 클라
        public XGTFEnetCpuInfo CpuInfo { get; protected set; }                //XGT시리즈의 시피유 종류
        public ushort InvokeID { get; protected set; }                        //프레임 간의  순서를 구별하기 위한 ID (응답 프레임에 이 번호를 붙여 보내줌)
        public ushort AppInstructionDataLength { get; protected set; }        //실질 데이터의 바이트 길이
        public byte SlotPosition { get; protected set; }                      // FEnet 모듈의 슬롯 넘버
        public byte BasePosition { get; protected set; }                      // FEnet 모듈의 베이스 넘버

        public XGTFEnetProtocolError Error { get; internal set; } //에러코드 2byte
        public XGTFEnetCommand Command { get; private set; } // 명령어
        public XGTFEnetDataType DataType { get; private set; } // 데이터 타입

        public IList<IProtocolItem> Items { get; set; }

        public virtual int GetErrorCode()
        {
            return -1;
        }
    }
}
