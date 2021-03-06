﻿using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using System.Diagnostics;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet - 프로토콜의 헤더, 국번, 명령어, 명령어 타입, 테일, 프레임 체크
    /// 등의 정보를 처리하는 abstract 클래스
    /// IProtocol <- AProtocol <- AXGTCnetProtocol <- XGTCnetProtocol
    /// </summary>
    public abstract class AXGTCnetProtocol : AProtocol
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        #region PUBLIC PROPERTIES
        /// <summary>
        /// 프로토콜 프레임 데이터
        /// </summary>
        public XGTCnetCCType Header { protected set; get; }        //헤더         1byte
        public ushort LocalPort { protected set; get; }            //국번         2byte
        public XGTCnetCommand Command { protected set; get; }      //명령어       1byte
        public XGTCnetCmdType CommandType { protected set; get; }  //명령어 타입  2byte
        public XGTCnetCCType Tail { protected set; get; }          //테일         1byte
        public byte BCC { protected set; get; }                    //프레임 체크  1byte or null
        public XGTCnetProtocolError Error { get; internal set; }   //에러
        public ushort BlocCnt { protected set; get; }   //블록 수 - 2byte
        public ushort DataCnt { protected set; get; }   //데이터 개수 - 2byte (읽거나 쓸 데이터의 개수 (BYTE = 데이터 타입 * 개수) 최대 240byte word는 120byte 가 한계)
        #endregion

        #region CONST VARIABLE
        protected const string ERROR_PROTOCOL_HEAD_SIZE = "Ascdata's array length under 6";
        protected const string ERROR_PROTOCOL_ASC_SIZE_MAX_256BYTE = "Protocoldata data's length over protocol_asc_size_limit(256byte)";
        protected const string ERROR_PROTOCOL_WSB_SIZE_MAX_240BYTE = "Data count(asc bytes) limited 240byte";

        public const int PROTOCOL_HEAD_SIZE = 6;
        public const int PROTOCOL_ASC_SIZE_MAX_256BYTE = 256;
        public const int PROTOCOL_MIN_MAIN_DATA_SIZE = 2;
        public const int PROTOCOL_WSB_SIZE_MAX_240BYTE = 240;
        #endregion

        #region PROTECTED METHOD
        /// <summary>
        /// 외부 호출에 의한 객체 생성을 방지한다.
        /// </summary>
        protected AXGTCnetProtocol() { }

        /// <summary>
        /// 복사생성자
        /// </summary>
        /// <param name="that">복사 타겟</param>
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
            this.BlocCnt = that.BlocCnt;
            this.DataCnt = that.DataCnt;
        }

        /// <summary>
        /// 응답 프로토콜 생성자
        /// </summary>
        /// <param name="ascii"></param>
        protected AXGTCnetProtocol(byte[] ascii)
            : base()
        {
            if (ascii != null)
                ASCIIData = ascii;
        }

        /// <summary>
        /// 요청 프로토콜 생성자
        /// </summary>
        /// <param name="localPort">국번</param>
        /// <param name="cmd">주명령어 옵션</param>
        /// <param name="type">명령어 옵션</param>
        protected AXGTCnetProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCmdType type)
            : base()
        {
            LocalPort = localPort;
            Command = cmd;
            CommandType = type;
        }

        /// <summary>
        /// 요청 프로토콜 - 헤더 정보를 추가
        /// </summary>
        /// <param name="ascii">버퍼</param>
        protected void AddHeader(List<byte> ascii)
        {
            ascii.Add(Header.ToByte());
            ascii.AddRange(CA2C.Data2ASCII(LocalPort));
            ascii.Add(Command.ToByte());
            ascii.AddRange(CommandType.ToBytes());
        }

        /// <summary>
        /// 요청 프로토콜 - 헤더가 x,y,w,r인 경우(소문자인 경우) 프레임체크(BCC) 값을 계산한다.
        /// 방법: 명령어가 소문자 프레임 예(w)일 경우 ENQ 에서 EOT 까지 값을 Hex 값으로 변환 한 뒤  
        /// 한 바이트씩 더한 결과의 하위 1바이트만 BCC 에 첨가한다.
        /// 
        /// 사용설명서XGT_Cnet_국문_V2.8(7.2.3 직접변수 개별쓰기(W(w)SS), 7-7)
        /// </summary>
        /// <param name="ascii">byte List</param>
        protected void AddTail(List<byte> ascii)
        {
            ascii.Add(Tail.ToByte());
            var cmd = Command;
            if (cmd == XGTCnetCommand.r || cmd == XGTCnetCommand.w)
            {
                ushort sum = 0;
                foreach (byte b in ascii)
                    sum += b;
                sum = (ushort)((ushort)0xFF & sum);
                BCC = (byte)sum;
                ascii.Add(BCC);
            }
        }

        /// <summary>
        /// 응답 프로토콜 - ascii데이터에서 헤더, 국번, 주명령어, 명령어 데이터를 가져온다.
        /// </summary>
        protected void CatchHeader()
        {
            if (ASCIIData.Length < PROTOCOL_HEAD_SIZE)
                throw new IndexOutOfRangeException(ERROR_PROTOCOL_HEAD_SIZE);

            byte[] head = new byte[PROTOCOL_HEAD_SIZE];
            Buffer.BlockCopy(ASCIIData, 0, head, 0, head.Length);
            //헤더 info
            Header = (XGTCnetCCType)head[0];
            if (!(Header == XGTCnetCCType.ACK || Header == XGTCnetCCType.ENQ || Header == XGTCnetCCType.NAK))
                throw new Exception("Invalid ASCIIData[0] (ACK, ENQ, NAK)");
            //국번 info
            LocalPort = CA2C.ToUInt16Value(new byte[] { head[1], head[2] });
            //주명령어 info
            Command = (XGTCnetCommand)head[3]; 
            //명령어 info
            CommandType = XGTCnetCommandTypeExtensions.ToCmdType(new byte[] { head[4], head[5] });
        }

        /// <summary>
        /// 응답 프로토콜 - ascii데이터에서 테일과 프레임체크(BC)값을 가져온다.
        /// </summary>
        protected void CatchTail()
        {
            bool isBCC_Exist = HasBCC();
            if (isBCC_Exist)
                BCC = ASCIIData.Last();
            Tail = (XGTCnetCCType)ASCIIData[ASCIIData.Length - 1 - (isBCC_Exist ? 1 : 0)];
            if (!(Tail == XGTCnetCCType.EOT || Tail == XGTCnetCCType.ETX))
                throw new Exception("Invalid ASCIIData.Last() (EOT, EXT)");
        }

        /// <summary>
        /// 응답 프로토콜 - ascii 데이터에서 구조화된 데이터만 계산하여 반환한다.
        /// </summary>
        /// <returns>구조화된 데이터</returns>
        protected byte[] GetInstructData()
        {
            int ascii_data_cnt = ASCIIData.Length - PROTOCOL_HEAD_SIZE - (HasBCC() ? 2 : 1);
            if (!(PROTOCOL_MIN_MAIN_DATA_SIZE <= ascii_data_cnt))
                throw new Exception("Impossibie byte asc sturected data count");
            byte[] instruct_ascii_data = new byte[ascii_data_cnt];
            Buffer.BlockCopy(ASCIIData, PROTOCOL_HEAD_SIZE, instruct_ascii_data, 0, ascii_data_cnt);
            return instruct_ascii_data;
        }

        /// <summary>
        /// 응답 프로토콜 - NAK 응답 시 ascii데이터에서 에러 코드를 가져온다.
        /// </summary>
        /// <returns>에러 여부</returns>
        protected bool CatchErrorCode()
        {
            Error = XGTCnetProtocolError.OK;
            bool hasErr = false;
            if (this.Header == XGTCnetCCType.NAK)
            {
                byte[] instruct_data = GetInstructData();
                if (instruct_data.Length == 4)
                {
                    byte[] swap = new byte[4];
                    Buffer.BlockCopy(instruct_data, 0, swap, 2, 2);
                    Buffer.BlockCopy(instruct_data, 2, swap, 0, 2);
                    Error = (XGTCnetProtocolError)CA2C.ToValue(swap, typeof(XGTCnetProtocolError));
                }
                hasErr = true;
            }
            return hasErr;
        }

        #endregion

        #region INTERNAL
        /// <summary>
        /// 응답 프로토콜 - ascii 데이터에서 ETX(테일) 포함 여부를 파악한다.
        /// </summary>
        /// <returns>EXT 포함 여부</returns>
        internal bool HasEXT()
        {
            if (ASCIIData.Length < PROTOCOL_HEAD_SIZE)
                return false;
            return ASCIIData[ASCIIData.Length - 1 - (HasBCC() ? 1 : 0)] == XGTCnetCCType.ETX.ToByte();
        }

        /// <summary>
        /// 응답 프로토콜 - 응답 프로토콜에서 프레임체크(BCC)값 포함여부를 파악한다.
        /// </summary>
        /// <returns>BCC 포함 여부</returns>
        internal bool HasBCC()
        {
            if (ASCIIData == null)
                throw new NullReferenceException("Protocoldata is null.");
            return Command == XGTCnetCommand.r || Command == XGTCnetCommand.w;
        }
        #endregion

        #region PUBLIC

        /// <summary>
        /// 응답 프로토콜 - ascii 데이터를 분석하여 프로토콜 프레임 정보를 파악한다.
        /// </summary>
        public override void AnalysisProtocol()
        {
            if (ASCIIData == null)
                throw new NullReferenceException("ASCIIData is null.");
            if (ASCIIData.Length < PROTOCOL_HEAD_SIZE)
                throw new ArgumentOutOfRangeException(ERROR_PROTOCOL_HEAD_SIZE);
            CatchHeader();
            if (!CatchErrorCode())
                DetachProtocolFrame();
            CatchTail();
        }

        /// <summary>
        /// 요청 프로토콜 - 프로토콜 프레임 정보로 스트림 버퍼에 쓸 ascii 데이터를 구축한다.
        /// </summary>
        public override void AssembleProtocol()
        {
            List<byte> ascii = new List<byte>();
            AddHeader(ascii);
            AttachProtocolFrame(ascii);
            AddTail(ascii);
            ASCIIData = ascii.ToArray();
            if (ASCIIData.Length > PROTOCOL_ASC_SIZE_MAX_256BYTE)
                throw new Exception(ERROR_PROTOCOL_ASC_SIZE_MAX_256BYTE);
        }

        /// <summary>
        /// 디버깅용 프로토콜 프레임 정보 출력
        /// </summary>
        public override void Print()
        {
            LOG.Debug("XGT Cnet 프로토콜 정보");
            LOG.Debug("ASC 코드: " + Bytes2HexString.Change(ASCIIData));
            LOG.Debug("국번: {0}", LocalPort);
            LOG.Debug(string.Format("헤더: {0}", Header == XGTCnetCCType.ENQ ? "ENQ" : Header == XGTCnetCCType.ACK ? "ACK" : "NAK"));
            LOG.Debug(string.Format("명령: {0}", (char)Command));
            LOG.Debug("명령타입: " + CommandType.ToString());
            if (Error == XGTCnetProtocolError.OK)
                PrintInstructPart();
            else
                LOG.Debug("에러: " + Error.ToString());
            LOG.Debug(string.Format("테일: {0}", Tail == XGTCnetCCType.EOT ? "EOT" : "EXT"));
            LOG.Debug(string.Format("BCC: {0}", BCC));
        }

        #endregion

        #region ABSTRACT METHOD
        protected abstract void PrintInstructPart();
        protected abstract void AttachProtocolFrame(List<byte> ascii);
        protected abstract void DetachProtocolFrame();
        #endregion
    }
}