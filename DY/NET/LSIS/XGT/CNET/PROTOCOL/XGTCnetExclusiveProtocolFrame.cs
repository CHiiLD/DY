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
        public const int PROTOCOL_HEAD_SIZE = 6;

        protected XGTCnetExclusiveProtocolFrame()
        {
        }

        public byte[] ASCData
        {
            get;
            set;
        }

        public event SocketDataReceivedEventHandler DataReceivedEvent = delegate { };
        public event SocketDataReceivedEventHandler ErrorEvent = delegate { };
        public event SocketDataReceivedEventHandler DataRequestedEvent = delegate { };

        public XGTCnetExclusiveProtocolError Error = XGTCnetExclusiveProtocolError.UNKNOWN;

        public XGTCnetControlCodeType Header;       //헤더         1byte
        public ushort LocalPort;                    //국번         2byte
        public XGTCnetCommand Command;              //명령어       1byte
        public XGTCnetCommandType CommandType;      //명령어 타입  2byte

        public XGTCnetControlCodeType Tail;         //테일         1byte
        public byte BCC;                            //프레임 체크   1byte or null

        protected void AddProtocolHead(List<byte> asc_list)
        {
            asc_list.Add((byte)this.Header);
            asc_list.AddRange(TransData.ToASC(this.LocalPort, 2));
            asc_list.Add((byte)this.Command);
            asc_list.AddRange(XGTCnetCommandTypeExtensions.ToByteArray(this.CommandType));
        }

        protected void AddProtocolTail(List<byte> asc_list)
        {
            asc_list.Add((byte)this.Tail);
            var cmd = this.Command;
            if (cmd == XGTCnetCommand.r || cmd == XGTCnetCommand.w || cmd == XGTCnetCommand.x || cmd == XGTCnetCommand.y)
            {
                ushort sum = 0;
                foreach (byte b in asc_list)
                    sum += b;
                sum = (ushort)(sum << 8);
                sum = (ushort)(sum >> 8);
                this.BCC = (byte)sum;
            }
        }

        protected void CatchProtocolHead()
        {
            if (ASCData.Length < PROTOCOL_HEAD_SIZE)
                throw new IndexOutOfRangeException("ASCData's array length under 6.");

            byte[] head = new byte[PROTOCOL_HEAD_SIZE];
            Buffer.BlockCopy(ASCData, 0, head, 0, head.Length);
            Header = (XGTCnetControlCodeType)head[0];

            byte[] localport = { head[1], head[2] };
            LocalPort = (ushort)TransData.ToHex(localport, typeof(ushort));

            Command = (XGTCnetCommand)head[3];
            byte[] cmd_type = { head[4], head[5] };
            CommandType = (XGTCnetCommandType)TransData.ToHex(cmd_type, typeof(ushort));
        }

        protected void CatchprotocolTail()
        {
            var cmd = this.Command;
            bool isBCC_Exist = IsExistBCCFromASCData();
            Tail = (XGTCnetControlCodeType)ASCData[ASCData.Length - 1 - (isBCC_Exist ? 1 : 0)];
            if (isBCC_Exist)
                BCC = ASCData.Last();
        }

        protected abstract void AttachProtocolFrame(List<byte> asc_list);
        protected abstract void DetachProtocolFrame();

        public void AssembleProtocol()
        {
            List<byte> asc_list = new List<byte>();
            AddProtocolHead(asc_list);
            AttachProtocolFrame(asc_list);
            AddProtocolTail(asc_list);
            ASCData = asc_list.ToArray();
        }

        public void AnalysisProtocol(byte[] binaryData)
        {
            if (binaryData == null)
                throw new ArgumentNullException("argument is null.");
            ASCData = binaryData;
            AnalysisProtocol();
        }

        public void AnalysisProtocol()
        {
            if (ASCData == null)
                throw new NullReferenceException("ASCData is null.");
            CatchProtocolHead();
            DetachProtocolFrame();
            CatchprotocolTail();
        }

        public bool IsExistBCCFromASCData()
        {
            if (ASCData == null)
                throw new NullReferenceException("ASCData is null.");
            
            if (Command == XGTCnetCommand.r || Command == XGTCnetCommand.w || Command == XGTCnetCommand.x || Command == XGTCnetCommand.y)
                return true;
            else
                return false;
        }
    }
}