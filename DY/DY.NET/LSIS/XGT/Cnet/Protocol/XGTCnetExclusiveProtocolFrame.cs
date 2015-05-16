/*
 * 작성자: CHILD	
 * 목적: LS산전의 XGT Cnet 전용 프로토콜 프레임 클래스 구현
 * 날짜: 2015-03-25
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet 개방형 프로토콜 추상 클래스
    /// </summary>
    public abstract class XGTCnetExclusiveProtocolFrame : IProtocol
    {
        public const int PROTOCOL_HEAD_SIZE = 6;
        public const int PROTOCOL_ASC_SIZE_LIMIT = 256;
        public const int PROTOCOL_ASC_SIZE_ERROR = 4;
        public const int PROTOCOL_SB_DATACNT_LIMIT = 240;

        #region var_properties_event
        /// <summary>
        /// 원시 프로토콜 데이터
        /// </summary>
        internal byte[] ProtocolData { get; set; }

        /// <summary>
        /// PROTOCOL FRAME DATAS
        /// </summary>
        private XGTCnetExclusiveProtocolError _Error = XGTCnetExclusiveProtocolError.OK;
        public XGTCnetExclusiveProtocolError Error
        {
            get
            {
                return _Error;
            }
            internal set
            {
                _Error = value;
            }
        }
        public XGTCnetControlCodeType Header { protected set; get; }        //헤더         1byte
        public ushort LocalPort { protected set; get; }                     //국번         2byte
        public XGTCnetCommand Command { protected set; get; }               //명령어       1byte
        public XGTCnetCommandType CommandType { protected set; get; }       //명령어 타입  2byte
        public XGTCnetControlCodeType Tail { protected set; get; }          //테일         1byte
        public byte BCC { protected set; get; }                             //프레임 체크   1byte or null
        #endregion

        #region interface
        /// <summary>
        /// 통신 중 예외 또는 에러가 발생시 통지
        /// </summary>
        public event EventHandler<SocketDataReceivedEventArgs> ErrorEvent;
        /// <summary>
        /// 프로토콜 요청을 성공적으로 전달되었을 시 통지
        /// </summary>
        public event EventHandler<SocketDataReceivedEventArgs> RequestedEvent;
        /// <summary>
        /// 요청된 프로토콜에 따른 응답 프로토콜을 성공적으로 받았을 시 통지
        /// </summary>
        public event EventHandler<SocketDataReceivedEventArgs> ReceivedEvent;

        /// <summary>
        /// Requested 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        public void OnDataReceived(object obj, IProtocol protocol)
        {
            var pt = System.Threading.Volatile.Read(ref protocol);
            if (ReceivedEvent != null)
                ReceivedEvent(obj, new SocketDataReceivedEventArgs(pt));
        }
        /// <summary>
        /// Requested 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        public void OnDataRequested(object obj, IProtocol protocol)
        {
            var pt = System.Threading.Volatile.Read(ref protocol);
            if (RequestedEvent != null)
                RequestedEvent(obj, new SocketDataReceivedEventArgs(pt));
        }
        /// <summary>
        /// OnError 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        public void OnError(object obj, IProtocol protocol)
        {
            var pt = System.Threading.Volatile.Read(ref protocol);
            if (ErrorEvent != null)
                ErrorEvent(obj, new SocketDataReceivedEventArgs(pt));
        }
        #endregion

        #region protected

        /// <summary>
        /// 동적 생성을 방지
        /// </summary>
        protected XGTCnetExclusiveProtocolFrame()
        {
        }

        /// <summary>
        /// 복사생성자
        /// </summary>
        /// <param name="that"> 복사할 XGTCnetExclusiveProtocolFrame 상속 객체 </param>
        protected XGTCnetExclusiveProtocolFrame(XGTCnetExclusiveProtocolFrame that)
        {
            this.ProtocolData = that.ProtocolData;
            this.ReceivedEvent = that.ReceivedEvent;
            this.ErrorEvent = that.ErrorEvent;
            this.RequestedEvent = that.RequestedEvent;
            this.Error = that.Error;
            this.Header = that.Header;
            this.LocalPort = that.LocalPort;
            this.Command = that.Command;
            this.CommandType = that.CommandType;
            this.Tail = that.Tail;
            this.BCC = that.BCC;
        }

        protected XGTCnetExclusiveProtocolFrame(byte[] binaryDatas)
        {
            ProtocolData = binaryDatas;
        }

        protected XGTCnetExclusiveProtocolFrame(ushort localPort, XGTCnetCommand cmd, XGTCnetCommandType type)
        {
            this.LocalPort = localPort;
            this.Command = cmd;
            this.CommandType = type;
        }

        protected abstract void PrintBinaryMainInfo();

        protected void AddProtocolHead(List<byte> asc_list)
        {
            asc_list.Add((byte)this.Header);
            asc_list.AddRange(CA2C.ToASC(this.LocalPort));
            asc_list.Add((byte)this.Command);
            asc_list.AddRange(this.CommandType.ToByteArray());
        }

        /// <summary>
        /// 해더의 커맨드가 소문자일 경우 BCC를 계산한다.
        /// </summary>
        /// <param name="asc_list"></param>
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

        /// <summary>
        /// 헤더를 파싱합니다.
        /// </summary>
        protected void CatchProtocolHead()
        {
            if (ProtocolData.Length < PROTOCOL_HEAD_SIZE)
                throw new IndexOutOfRangeException("ASCData's array length under 6.");

            byte[] head = new byte[PROTOCOL_HEAD_SIZE];
            Buffer.BlockCopy(ProtocolData, 0, head, 0, head.Length);
            Header = (XGTCnetControlCodeType)head[0];

            byte[] localport = { head[1], head[2] };
            LocalPort = (ushort)CA2C.ToValue(localport, typeof(ushort));

            Command = (XGTCnetCommand)head[3];
            byte[] cmd_type = { head[4], head[5] };
            CommandType = XGTCnetCommandTypeExtensions.ToCmdType(cmd_type);
        }

        /// <summary>
        /// 테일을 파싱합니다.
        /// </summary>
        protected void CatchprotocolTail()
        {
            var cmd = this.Command;
            bool isBCC_Exist = IsExistBCCFromASCData();
            Tail = (XGTCnetControlCodeType)ProtocolData[ProtocolData.Length - 1 - (isBCC_Exist ? 1 : 0)];
            if (isBCC_Exist)
                BCC = ProtocolData.Last();
        }

        /// <summary>
        /// 헤더 국번 명령어 명령어타입 테일 프레임체크를 제외한 메인 데이터 부분만 추출해서 리턴합니다.
        /// </summary>
        /// <returns> 헤더 국번 명령어 명령어타입 테일 프레임체크를 제외한 메인 데이터 </returns>
        protected byte[] GetMainData()
        {
            int asc_data_cnt = ProtocolData.Length - PROTOCOL_HEAD_SIZE - (IsExistBCCFromASCData() ? 2 : 1);
            if (!(PROTOCOL_ASC_SIZE_ERROR <= asc_data_cnt && asc_data_cnt < PROTOCOL_ASC_SIZE_LIMIT))
                throw new Exception("impossibie byte asc sturected data count");

            byte[] asc_arr = new byte[asc_data_cnt];
            Buffer.BlockCopy(ProtocolData, PROTOCOL_HEAD_SIZE, asc_arr, 0, asc_data_cnt);
            return asc_arr;
        }

        /// <summary>
        /// 헤더가 NAK 일 경우, 에러를 파싱합니다.
        /// </summary>
        /// <returns></returns>
        protected bool CatchErrorCode()
        {
            bool ret = false;
            if (this.Header == XGTCnetControlCodeType.NAK)
            {
                byte[] main_data = this.GetMainData();
                if (main_data.Length == 4)
                    this.Error = (XGTCnetExclusiveProtocolError)CA2C.ToValue(main_data, typeof(uint));
                ret = true;
            }
            return ret;
        }

        protected abstract void AttachProtocolFrame(List<byte> asc_list);
        protected abstract void DetachProtocolFrame();

        #endregion

        #region internal
        /// <summary>
        /// 받은 ASC데이터들의 테일을 검사하여 EXT 값이 왔는지 검사합니다.
        /// </summary>
        /// <returns> 검사값, 프로토콜 로스 없이 전부 받았다면 ture 아니면 false </returns>
        internal bool IsComeInEXTTail()
        {
            if (ProtocolData.Length < PROTOCOL_HEAD_SIZE)
                return false;

            bool isBCC_Exist = IsExistBCCFromASCData();
            byte value = ProtocolData[ProtocolData.Length - 1 - (isBCC_Exist ? 1 : 0)];
            return value == (byte)XGTCnetControlCodeType.ETX;
        }

        /// <summary>
        /// 맴머 변수의 정보를 토대로 원시 프로토콜 데이터를 구합니다.
        /// </summary>
        internal void AssembleProtocol()
        {
            List<byte> asc_list = new List<byte>();
            AddProtocolHead(asc_list);
            AttachProtocolFrame(asc_list);
            AddProtocolTail(asc_list);
            ProtocolData = asc_list.ToArray();

            if (ProtocolData.Length > PROTOCOL_ASC_SIZE_LIMIT)
                throw new Exception("binary data's length over PROTOCOL_ASC_SIZE_LIMIT(256byte)");
        }

        /// <summary>
        /// 받은 원시 프로토콜 데이터를 바탕으로 프로토콜 구조와 데이터를 파악합니다.
        /// </summary>
        internal void AnalysisProtocol()
        {
            if (ProtocolData == null)
                throw new NullReferenceException("BinaryData is null.");
            if (ProtocolData.Length < PROTOCOL_HEAD_SIZE)
                throw new ArgumentOutOfRangeException("BinaryData is not understandable data's length");

            CatchProtocolHead();
            if (!CatchErrorCode())
                DetachProtocolFrame();
            CatchprotocolTail();
        }

        /// <summary>
        /// BCC데이터가 추가 될지 확인하는 메서드
        /// </summary>
        /// <returns></returns>
        internal bool IsExistBCCFromASCData()
        {
            if (ProtocolData == null)
                throw new NullReferenceException("BinaryData is null.");

            if (Command == XGTCnetCommand.r || Command == XGTCnetCommand.w || Command == XGTCnetCommand.x || Command == XGTCnetCommand.y)
                return true;
            else
                return false;
        }

        #endregion
       
        public void PrintBinaryFrameInfo()
        {
            Console.WriteLine("XGT PROTOCOL INFORMATION");
            Console.WriteLine("ASC Code: " + B2HS.Change(ProtocolData));
            Console.WriteLine("Local Port {0}", LocalPort);
            Console.WriteLine(string.Format("Header: {0}", Header == XGTCnetControlCodeType.ENQ ? "ENQ" : Header == XGTCnetControlCodeType.ACK ? "ACK" : "NAK"));
            Console.WriteLine(string.Format("Command: {0}", (char)Command));
            Console.WriteLine("CommandType: " + CommandType.ToString());
            if (Error == XGTCnetExclusiveProtocolError.OK)
                PrintBinaryMainInfo();
            else
                Console.WriteLine("Error : " + Error.ToString());
            Console.WriteLine(string.Format("Tail: {0}", Tail == XGTCnetControlCodeType.EOT ? "EOT" : "EXT"));
            Console.WriteLine(string.Format("BCC: {0}", BCC));
            Console.Write("--------------------------------------------------------------------------------");
        }
    }
}