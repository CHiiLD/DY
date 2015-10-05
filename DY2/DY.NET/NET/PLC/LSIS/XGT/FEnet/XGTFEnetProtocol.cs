using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet Protocol Structure Information
    /// </summary>
    public class XGTFEnetProtocol : IProtocol
    {
        private static readonly Random ms_Random = new Random();
        //HEADER INFORMATION
        public XGTFEnetCompanyID CompanyID { get; set; }            //PLC 제품

        public XGTFEnetCpuType CpuType { get; set; }                //bit00-05 시피유 타입
        public XGTFEnetClass Class { get; set; }                    //7bit
        public XGTFEnetCpuState CpuState { get; set; }              //8bit 

        public XGTFEnetPLCSystemState PLCState { get; set; }        //bit08-12 시스템 상태

        public XGTFEnetStreamDirection StreamDirection { get; set; }//클라 -> 서버 or 서버 -> 클라
        public XGTFEnetCpuInfo CpuInfo { get; set; }                //XGT시리즈의 시피유 종류
        public ushort InvokeID { get; set; }                        //프레임 간의  순서를 구별하기 위한 ID (응답 프레임에 이 번호를 붙여 보내줌)
        public ushort BodyLength { get; set; }                      //실질 데이터의 바이트 개수
        public byte SlotPosition { get; set; }                      // FEnet 모듈의 슬롯 넘버
        public byte BasePosition { get; set; }                      // FEnet 모듈의 베이스 넘버
        public byte BCC { get; set; }
        //FRAME INFORMATION
        public XGTFEnetError Error { get; set; } //에러코드 2byte
        public XGTFEnetCommand Command { get; set; } // 명령어
        public XGTFEnetDataType DataType { get; set; } // 데이터 타입
        //DATA INFORMATION
        public Type Type { get; set; }
        public IList<IProtocolData> Data { get; set; }

        public XGTFEnetProtocol()
        {
            Initialize();
        }

        public XGTFEnetProtocol(Type type, XGTFEnetCommand command)
            : this()
        {
            Command = command;
            DataType = type.ToXGTFEnetDataType();
            Type = type;
        }

        public virtual int GetErrorCode()
        {
            return (int)Error;
        }

        public virtual void Initialize()
        {
            CompanyID = XGTFEnetCompanyID.NONE;
            CpuType = XGTFEnetCpuType.NONE;
            Class = XGTFEnetClass.MASTER;
            CpuState = XGTFEnetCpuState.CPU_NOR;
            PLCState = XGTFEnetPLCSystemState.NONE;
            StreamDirection = XGTFEnetStreamDirection.NONE;
            CpuInfo = XGTFEnetCpuInfo.NONE;
            InvokeID = (ushort)ms_Random.Next((int)ushort.MaxValue);
            BCC = 0;
            BodyLength = 0;
            SlotPosition = 0;
            BasePosition = 0;
            Error = XGTFEnetError.OK;
            Command = XGTFEnetCommand.NONE;
            DataType = XGTFEnetDataType.NONE;

            Data = null;
            this.Type = null;
        }
    }
}