/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜 프레임 클래스 구현
 * 날짜: 2015-03-25
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetExclusiveProtocolFrame : IProtocol
    {
        protected XGTCnetExclusiveProtocolFrame()
        {

        }

        public byte[] ASCData
        {
            get;
            set;
        }
        public XGTCnetExclusiveProtocolError Error = XGTCnetExclusiveProtocolError.UNKNOWN;

        public XGTCnetControlCodeType   Header;         //헤더         1byte
        public byte                     LocalPort;      //국번         2byte
        public XGTCnetMainCommandType   Command;        //명령어       1byte
        public XGTCnetCommandType       CommandType;    //명령어 타입  2byte

        public XGTCnetControlCodeType   Tail;           //테일         1byte
        public byte                     BCC;            //프레임 체크   1byte or null
    }
}