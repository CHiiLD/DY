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
        public byte[] BinaryData
        {
            get;
            set;
        }
        public XGTCnetExclusiveProtocolError Error = XGTCnetExclusiveProtocolError.UNKNOWN;

        public XGTCnetControlCodeType   Header;         //헤더
        public byte                     LocalPort;      //국번
        public XGTCnetMainCommandType   Command;        //명령어
        public XGTCnetCommandType       CommandType;    //명령어 타입

        public XGTCnetControlCodeType   Tail;           //테일
        public byte                     BCC;            //프레임 체크
    }
}