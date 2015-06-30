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
    public abstract class AXGTCnetProtocol : AProtocol
    {
        #region PUBLIC PROPERTIES
        /// <summary>
        /// PROTOCOL FRAME DATAS
        /// </summary>
        public XGTCnetCCType Header { protected set; get; }        //헤더         1byte
        public ushort LocalPort { protected set; get; }                     //국번         2byte
        public XGTCnetCommand Command { protected set; get; }               //명령어       1byte
        public XGTCnetCmdType CommandType { protected set; get; }       //명령어 타입  2byte
        public XGTCnetCCType Tail { protected set; get; }          //테일         1byte
        public byte BCC { protected set; get; }                             //프레임 체크  1byte or null
        public XGTCnetProtocolError Error { get; internal set; } //에러
        #endregion

        #region CONST VARIABLE
        protected const string ERROR_PROTOCOL_HEAD_SIZE = "ASCDATA'S ARRAY LENGTH UNDER 6";
        protected const string ERROR_PROTOCOL_ASC_SIZE_MAX_256BYTE = "PROTOCOLDATA DATA'S LENGTH OVER PROTOCOL_ASC_SIZE_LIMIT(256BYTE)";
        protected const string ERROR_PROTOCOL_SB_SIZE_MAX_240BYTE = "DATA COUNT(ASC BYTES) LIMITED 240BYTE";

        public const int XY_PROTOCOL_HEAD_SIZE = 4;
        public const int RW_PROTOCOL_HEAD_SIZE = 6;
        public const int PROTOCOL_ASC_SIZE_MAX_256BYTE = 256;
        public const int PROTOCOL_MIN_MAIN_DATA_SIZE = 2;
        public const int PROTOCOL_SB_SIZE_MAX_240BYTE = 240;
        #endregion

        #region PROTECTED METHOD
        /// <summary>
        /// 동적 생성을 방지
        /// </summary>
        protected AXGTCnetProtocol() { }

        /// <summary>
        /// 복사생성자
        /// </summary>
        /// <param name="that"> 복사할 XGTCnetExclusiveProtocolFrame 상속 객체 </param>
        protected AXGTCnetProtocol(AXGTCnetProtocol that)
            : base(that)
        {
            this.Error = that.Error;
            this.Header = that.Header;
            this.LocalPort = that.LocalPort;
            this.Command = that.Command;
            this.CommandType = that.CommandType;
            this.Tail = that.Tail;
            this.BCC = that.BCC;
            
        }

        protected AXGTCnetProtocol(byte[] binaryDatas)
            : base()
        {
            if (binaryDatas != null)
                ASC2Protocol = binaryDatas;
        }

        protected AXGTCnetProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCmdType type)
            : base()
        {
            LocalPort = localPort;
            Command = cmd;
            CommandType = type;
        }

        /// <summary>
        /// 프로토콜에 헤더 정보를 입력합니다
        /// </summary>
        /// <param name="asc_list">입력할 byte List</param>
        protected void AddProtocolHead(List<byte> asc_list)
        {
            asc_list.Add(Header.ToByte());
            asc_list.AddRange(CA2C.ToASC(LocalPort));
            asc_list.Add(Command.ToByte());
            if (Command == XGTCnetCommand.r || Command == XGTCnetCommand.w || Command == XGTCnetCommand.R || Command == XGTCnetCommand.W)
                asc_list.AddRange(CommandType.ToBytes());
        }

        /// <summary>
        /// 해더의 커맨드가 소문자일 경우 BCC를 계산한다.
        /// </summary>
        /// <param name="asc_list">byte List</param>
        protected void AddProtocolTail(List<byte> asc_list)
        {
            asc_list.Add(Tail.ToByte());
            var cmd = Command;
            if (cmd == XGTCnetCommand.r || cmd == XGTCnetCommand.w || cmd == XGTCnetCommand.x || cmd == XGTCnetCommand.y)
            {
                ushort sum = 0;
                foreach (byte b in asc_list)
                    sum += b;
                sum = (ushort)((ushort)0xFF & sum);
                BCC = (byte)sum;
                asc_list.Add(BCC);
            }
        }

        /// <summary>
        /// 헤더를 파싱합니다.
        /// </summary>
        protected void CatchProtocolHead()
        {
            if (ASC2Protocol.Length < RW_PROTOCOL_HEAD_SIZE)
                throw new IndexOutOfRangeException(ERROR_PROTOCOL_HEAD_SIZE);

            byte[] head = new byte[RW_PROTOCOL_HEAD_SIZE];
            Buffer.BlockCopy(ASC2Protocol, 0, head, 0, head.Length);
            Header = (XGTCnetCCType)head[0];
            LocalPort = (ushort)CA2C.ToValue(new byte[] { head[1], head[2] }, typeof(ushort));
            Command = (XGTCnetCommand)head[3];
            if (Command == XGTCnetCommand.r || Command == XGTCnetCommand.w || Command == XGTCnetCommand.R || Command == XGTCnetCommand.W)
                CommandType = XGTCnetCommandTypeExtensions.ToCmdType(new byte[] { head[4], head[5] });
            else
                // XY 응답 프로토콜은 SS, SB의 구분을 알려주는 값을 주지 않습니다.
                // 따라서 요청프로토콜을 사용하여 SS, SB의 여부를 가져옵니다. (뭔가 좀 이상한 LS산전 프로토콜)
                CommandType = ((AXGTCnetProtocol)OtherParty).CommandType;
        }

        /// <summary>
        /// 테일을 파싱합니다.
        /// </summary>
        protected void CatchprotocolTail()
        {
            bool isBCC_Exist = IsExistBCCFromASCData();
            if (isBCC_Exist)
                BCC = ASC2Protocol.Last();
            Tail = (XGTCnetCCType)ASC2Protocol[ASC2Protocol.Length - 1 - (isBCC_Exist ? 1 : 0)];
        }

        /// <summary>
        /// 헤더 국번 명령어 명령어타입 테일 프레임체크를 제외한 메인 데이터 부분만 추출해서 리턴합니다.
        /// </summary>
        /// <returns> 헤더 국번 명령어 명령어타입 테일 프레임체크를 제외한 메인 데이터 </returns>
        protected byte[] GetMainData()
        {
            int head_size = (Command == XGTCnetCommand.r || Command == XGTCnetCommand.R || Command == XGTCnetCommand.w || Command == XGTCnetCommand.W) ? RW_PROTOCOL_HEAD_SIZE : XY_PROTOCOL_HEAD_SIZE;
            int asc_data_cnt = ASC2Protocol.Length - head_size - (IsExistBCCFromASCData() ? 2 : 1);
            if (!(PROTOCOL_MIN_MAIN_DATA_SIZE <= asc_data_cnt))
                throw new Exception("IMPOSSIBIE BYTE ASC STURECTED DATA COUNT");

            byte[] asc_arr = new byte[asc_data_cnt];
            Buffer.BlockCopy(ASC2Protocol, head_size, asc_arr, 0, asc_data_cnt);
            return asc_arr;
        }

        /// <summary>
        /// 헤더가 NAK 일 경우, 에러를 파싱합니다.
        /// </summary>
        /// <returns></returns>
        protected bool CatchErrorCode()
        {
            bool ret = false;
            if (this.Header == XGTCnetCCType.NAK)
            {
                byte[] main_data = GetMainData();
                if (main_data.Length == 4)
                {
                    byte[] swap = new byte[4];
                    Buffer.BlockCopy(main_data, 0, swap, 2, 2);
                    Buffer.BlockCopy(main_data, 2, swap, 0, 2);
                    this.Error = (XGTCnetProtocolError)CA2C.ToValue(swap, typeof(uint));
                }
                ret = true;
            }
            return ret;
        }


        #endregion

        #region internal
        /// <summary>
        /// 받은 ASC데이터들의 테일을 검사하여 EXT 값이 왔는지 검사합니다.
        /// </summary>
        /// <returns> 검사값, 프로토콜 로스 없이 전부 받았다면 ture 아니면 false </returns>
        internal bool IsComeInEXTTail()
        {
            if (ASC2Protocol.Length < RW_PROTOCOL_HEAD_SIZE)
                return false;
            return ASC2Protocol[ASC2Protocol.Length - 1 - (IsExistBCCFromASCData() ? 1 : 0)] == XGTCnetCCType.ETX.ToByte();
        }

        internal bool IsComeInEXTTail(byte[] asc_data)
        {
            if (asc_data.Length < RW_PROTOCOL_HEAD_SIZE)
                return false;
            return asc_data[asc_data.Length - 1 - (IsExistBCCFromASCData() ? 1 : 0)] == XGTCnetCCType.ETX.ToByte();
        }

        /// <summary>
        /// 맴머 변수의 정보를 토대로 원시 프로토콜 데이터를 구합니다.
        /// </summary>
        internal override void AssembleProtocol()
        {
            List<byte> asc_list = new List<byte>();
            AddProtocolHead(asc_list);
            AttachProtocolFrame(asc_list);
            AddProtocolTail(asc_list);
            ASC2Protocol = asc_list.ToArray();
            if (ASC2Protocol.Length > PROTOCOL_ASC_SIZE_MAX_256BYTE)
                throw new Exception(ERROR_PROTOCOL_ASC_SIZE_MAX_256BYTE);
        }

        /// <summary>
        /// 받은 원시 프로토콜 데이터를 바탕으로 프로토콜 구조와 데이터를 파악합니다.
        /// </summary>
        internal override void AnalysisProtocol()
        {
            if (ASC2Protocol == null)
                throw new NullReferenceException("PROTOCOLDATA IS NULL.");
            if (ASC2Protocol.Length < RW_PROTOCOL_HEAD_SIZE)
                throw new ArgumentOutOfRangeException(ERROR_PROTOCOL_HEAD_SIZE);
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
            if (ASC2Protocol == null)
                throw new NullReferenceException("PROTOCOLDATA IS NULL.");
            if (Command == XGTCnetCommand.r || Command == XGTCnetCommand.w || Command == XGTCnetCommand.x || Command == XGTCnetCommand.y)
                return true;
            else
                return false;
        }

        #endregion

        public void PrintBinaryFrameInfo()
        {
            Console.WriteLine("XGT 프로토콜 정보");
            Console.WriteLine("ASC 코드: " + B2HS.Change(ASC2Protocol));
            Console.WriteLine("국번: {0}", LocalPort);
            Console.WriteLine(string.Format("헤더: {0}", Header == XGTCnetCCType.ENQ ? "ENQ" : Header == XGTCnetCCType.ACK ? "ACK" : "NAK"));
            Console.WriteLine(string.Format("명령: {0}", (char)Command));
            Console.WriteLine("명령타입: " + CommandType.ToString());
            if (Error == XGTCnetProtocolError.OK)
                PrintBinaryMainInfo();
            else
                Console.WriteLine("에러: " + Error.ToString());
            Console.WriteLine(string.Format("테일: {0}", Tail == XGTCnetCCType.EOT ? "EOT" : "EXT"));
            Console.WriteLine(string.Format("BCC: {0}", BCC));
            Console.WriteLine("--------------------------------------------------------------------------------");
        }

        #region ABSTRACT METHOD
        protected abstract void PrintBinaryMainInfo();
        protected abstract void AttachProtocolFrame(List<byte> asc_list);
        protected abstract void DetachProtocolFrame();
        #endregion
    }
}