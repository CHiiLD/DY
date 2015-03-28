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
    public abstract class XGTCnetExclusiveProtocolFrame : IProtocol
    {
        protected XGTCnetExclusiveProtocolFrame()
        {
        }

        public byte[] ASCData
        {
            get;
            set;
        }

        public event SocketDataReceivedEventHandler DataReceivedEvent;
        public event SocketDataReceivedEventHandler ErrorEvent;
        public event SocketDataReceivedEventHandler DataRequestedEvent;

        public XGTCnetExclusiveProtocolError Error = XGTCnetExclusiveProtocolError.UNKNOWN;

        public XGTCnetControlCodeType   Header;         //헤더         1byte
        public ushort                   LocalPort;      //국번         2byte
        public XGTCnetCommand   Command;        //명령어       1byte
        public XGTCnetCommandType       CommandType;    //명령어 타입  2byte

        public XGTCnetControlCodeType   Tail;           //테일         1byte
        public byte                     BCC;            //프레임 체크   1byte or null

        protected void AddProtocolHead(List<byte> asc_list)
        {
            asc_list.Add((byte)this.Header);
            asc_list.AddRange(TransASC.ToASC(this.LocalPort, DataType.WORD));
            asc_list.Add((byte)this.Command);
            asc_list.AddRange(XGTCnetCommandTypeExtensions.ToByteArray(this.CommandType));
        }

        protected void AddProtocolTail(List<byte> asc_list)
        {
            asc_list.Add((byte)this.Tail);
            var c = this.Command;
            if (c == XGTCnetCommand.r || c == XGTCnetCommand.w || c == XGTCnetCommand.x || c == XGTCnetCommand.y)
            {
                ushort sum = 0;
                foreach (byte b in asc_list)
                    sum += b;
                sum = (ushort)(sum << 8);
                sum = (ushort)(sum >> 8);
                this.BCC = (byte)sum;
            }
        }

        protected abstract void AddProtocolFrame(List<byte> asc_list);

        public void AssembleProtocol()
        {
            List<byte> asc_list = new List<byte>();
            AddProtocolHead(asc_list);
            AddProtocolFrame(asc_list);
            AddProtocolTail(asc_list);
            ASCData = asc_list.ToArray();
        }
    }
}