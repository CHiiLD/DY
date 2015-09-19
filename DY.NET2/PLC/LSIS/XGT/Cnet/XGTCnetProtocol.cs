using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetProtocol : IProtocol
    {
        public XGTCnetControlChar Header { protected set; get; }   //헤더         1byte
        public ushort LocalPort { protected set; get; }            //국번         2byte
        public XGTCnetCommand Command { protected set; get; }      //명령어       1byte
        public XGTCnetCommandType CommandType { protected set; get; } //명령어 타입  2byte
        public XGTCnetControlChar Tail { protected set; get; }     //테일         1byte
        public byte BCC { protected set; get; }                    //프레임 체크  1byte or null
        public XGTCnetProtocolError Error { get; internal set; }   //에러

        public IList<IProtocolItem> Items { get; set; }

        public int GetErrorCode()
        {
            //Error 코드를 int로 변환해서 반환
            return -1;
        }
    }
}