/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜 RWSB RWSS XYSS XYSB 에 사용할 수 있는 범용 클래스
 * 날짜: 2015-03-25
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetExclusiveProtocol : XGTCnetExclusiveProtocolFrame
    {
        public ushort BlockCnt;         //2byte
        List<ENQDataFormat> ENQDatas = new List<ENQDataFormat>();   //?byte
        List<ACKDataFormat> ACKDatas = new List<ACKDataFormat>();   //?byte
        public ushort RegisterNum;      //2byte

        protected void AddProtocolRSS(List<byte> asc_list)
        {
            asc_list.AddRange(TransASC.ToASC(ENQDatas.Count, DataType.WORD)); // BLOCK COUNT ATTACH
            foreach(ENQDataFormat d in ENQDatas)    //변수길이, 변수이름이 필요
            {
                asc_list.AddRange(TransASC.ToASC(d.Var_Name.Length, DataType.WORD));
                asc_list.AddRange(TransASC.ToASC(d.Var_Name, 1));
            }
        }

        protected void AddProtocolWSS(List<byte> asc_list)
        {

        }

        protected void AddProtocolXSS(List<byte> asc_list)
        {

        }

        protected void AddProtocolRSB(List<byte> asc_list)
        {

        }

        protected void AddProtocolWSB(List<byte> asc_list)
        {

        }

        protected void AddProtocolXSB(List<byte> asc_list)
        {

        }

        protected override void AddProtocolFrame(List<byte> asc_list)
        {
            switch (CommandType)
            {
                case XGTCnetCommandType.SS:
                    break;
                case XGTCnetCommandType.SB:
                    break;
            }
        }
    }
}