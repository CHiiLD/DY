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
        public const int PROTOCOL_ASC_SIZE_LIMIT = 256;
        public const int PROTOCOL_ASC_SIZE_ERROR = 4;

        protected XGTCnetExclusiveProtocolFrame()
        {

        }

        protected XGTCnetExclusiveProtocolFrame(ushort localPort, XGTCnetCommand cmd, XGTCnetCommandType type)
        {
            this.LocalPort = localPort;
            this.Command = cmd;
            this.CommandType = type;
        }

        public byte[] BinaryData
        {
            get;
            set;
        }

        public event SocketDataReceivedEventHandler DataReceivedEvent;
        public event SocketDataReceivedEventHandler ErrorEvent;
        public event SocketDataReceivedEventHandler DataRequestedEvent;

        public XGTCnetExclusiveProtocolError Error = XGTCnetExclusiveProtocolError.OK;

        public XGTCnetControlCodeType Header;       //헤더         1byte
        public ushort LocalPort;                    //국번         2byte
        public XGTCnetCommand Command;              //명령어       1byte
        public XGTCnetCommandType CommandType;      //명령어 타입  2byte

        public XGTCnetControlCodeType Tail;         //테일         1byte
        public byte BCC;                            //프레임 체크   1byte or null

        protected abstract void PrintBinaryDataInfo();

        public static bool CatchErrorCode(XGTCnetExclusiveProtocolFrame frame)
        {
            bool ret = false;
            frame.CatchProtocolHead();
            if (frame.Header == XGTCnetControlCodeType.NAK)
            {
                byte[] main_data = frame.GetMainData();
                if (main_data.Length == 4)
                    frame.Error = (XGTCnetExclusiveProtocolError)CA2C.ToValue(main_data, typeof(uint));
                ret = true;
            }
            return ret;
        }

        protected void AddProtocolHead(List<byte> asc_list)
        {
            asc_list.Add((byte)this.Header);
            asc_list.AddRange(CA2C.ToASC(this.LocalPort));
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
                asc_list.Add(BCC);
            }
        }

        protected void CatchProtocolHead()
        {
            if (BinaryData.Length < PROTOCOL_HEAD_SIZE)
                throw new IndexOutOfRangeException("ASCData's array length under 6.");

            byte[] head = new byte[PROTOCOL_HEAD_SIZE];
            Buffer.BlockCopy(BinaryData, 0, head, 0, head.Length);
            Header = (XGTCnetControlCodeType)head[0];

            byte[] localport = { head[1], head[2] };
            LocalPort = (ushort)CA2C.ToValue(localport, typeof(ushort));

            Command = (XGTCnetCommand)head[3];
            byte[] cmd_type = { head[4], head[5] };
            CommandType = XGTCnetCommandTypeExtensions.ToCmdType(cmd_type);
        }

        protected void CatchprotocolTail()
        {
            var cmd = this.Command;
            bool isBCC_Exist = IsExistBCCFromASCData();
            Tail = (XGTCnetControlCodeType)BinaryData[BinaryData.Length - 1 - (isBCC_Exist ? 1 : 0)];
            if (isBCC_Exist)
                BCC = BinaryData.Last();
        }

        //헤더 국번 명령어 명령어타입 테일 프레임체크를 제외한 메인 데이터 부분만 추출해서 리턴합니다.
        protected byte[] GetMainData()
        {
            int asc_data_cnt = BinaryData.Length - PROTOCOL_HEAD_SIZE - (IsExistBCCFromASCData() ? 2 : 1);
            if (!(PROTOCOL_ASC_SIZE_ERROR <= asc_data_cnt && asc_data_cnt < PROTOCOL_ASC_SIZE_LIMIT))
                throw new Exception("impossibie byte asc sturected data count");

            byte[] asc_arr = new byte[asc_data_cnt];
            Buffer.BlockCopy(BinaryData, PROTOCOL_HEAD_SIZE, asc_arr, 0, asc_data_cnt);
            return asc_arr;
        }

        protected abstract void AttachProtocolFrame(List<byte> asc_list);
        protected abstract void DetachProtocolFrame();

        public void AssembleProtocol()
        {
            List<byte> asc_list = new List<byte>();
            AddProtocolHead(asc_list);
            AttachProtocolFrame(asc_list);
            AddProtocolTail(asc_list);
            BinaryData = asc_list.ToArray();
        }

        public void AnalysisProtocol(byte[] binaryData)
        {
            if (binaryData == null)
                throw new ArgumentNullException("argument is null.");
            BinaryData = binaryData;
            AnalysisProtocol();
        }

        public void AnalysisProtocol()
        {
            if (BinaryData == null)
                throw new NullReferenceException("ASCData is null.");
            if (CatchErrorCode(this) == false)
                DetachProtocolFrame();
            CatchprotocolTail();
        }

        public bool IsExistBCCFromASCData()
        {
            if (BinaryData == null)
                throw new NullReferenceException("ASCData is null.");

            if (Command == XGTCnetCommand.r || Command == XGTCnetCommand.w || Command == XGTCnetCommand.x || Command == XGTCnetCommand.y)
                return true;
            else
                return false;
        }

        public void OnDataReceivedEvent(object obj, SocketDataReceivedEventArgs e)
        {
            if (DataReceivedEvent != null)
                DataReceivedEvent(obj, e);
        }

        public void OnDataRequestedEvent(object obj, SocketDataReceivedEventArgs e)
        {
            if (DataRequestedEvent != null)
                DataRequestedEvent(obj, e);
        }

        public void OnErrorEvent(object obj, SocketDataReceivedEventArgs e)
        {
            if (ErrorEvent != null)
                ErrorEvent(obj, e);
        }

        public void PrintInfo()
        {
            Console.WriteLine("XGT Protocol Information");
            Console.WriteLine("ASC Code: " + Bytes2HexString.Change(BinaryData));
            Console.WriteLine(string.Format("Header: {0}", Header == XGTCnetControlCodeType.ENQ ? "ENQ" : Header == XGTCnetControlCodeType.ACK ? "ACK" : "NAK"));
            Console.WriteLine(string.Format("Command: {0}", (char)Command));
            Console.WriteLine("CommandType: " + XGTCnetCommandTypeExtensions.ToString(CommandType));
            if (Error == XGTCnetExclusiveProtocolError.OK)
                PrintBinaryDataInfo();
            else
                Console.WriteLine("Error : " + Error.ToString());
            Console.WriteLine(string.Format("Tail: {0}", Tail == XGTCnetControlCodeType.EOT ? "EOT" : "EXT"));
            Console.WriteLine(string.Format("BCC: {0}", BCC));
        }
    }
}