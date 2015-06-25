/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜
 * 날짜: 2015-03-31
 */

using System;
using System.Collections.Generic;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet 통신을 위한 요청 프로토콜 클래스
    /// </summary>
    public class XGTCnetRequestProtocol : XGTCnetProtocolFrame
    {
        private const string ERROR_ENQ_IS_NULL_OR_EMPTY = "ENQDATAS HAVE PROBLEM (NULL OR EMPTY DATA)";
        private const string ERROR_READED_MEM_COUNT_LIMIT = "ENQDATAS OVER LIMIT OF COUNT (NULL OR EMPTY DATA)";
        private const string ERROR_MONITER_INVALID_REGISTER_NUMBER = "REGISTER_NUMBER HAVE TO REGISTER TO 0 FROM 31";
        private const int READED_MEM_MAX_COUNT = 16;
        private const int MONITER_VAR_REGISTER_MAX_NUMBER = 31;

        /// <summary>
        /// PROTOCOL MAIN DATAS
        /// </summary>
        public ushort SizeOfData { protected set; get; }     //읽거나 쓸 데이터의 바이트 사이즈 (BYTE = 데이터 타입 * 개수) 최대 240byte word는 120byte 가 한계 //2byte
        public List<ReqtDataFmt> ReqtDatas { get; protected set; }

        /// <summary>
        /// XGTCnetExclusiveProtocol 복사생성자
        /// </summary>
        /// <param name="that"> 복사하고자 할 XGTCnetExclusiveProtocol 객체 </param>
        internal XGTCnetRequestProtocol(XGTCnetRequestProtocol that)
            : base(that)
        {
            Init();
            this.SizeOfData = that.SizeOfData;
            this.ReqtDatas.AddRange(that.ReqtDatas);
        }

        protected XGTCnetRequestProtocol()
            : base()
        {
            Init();
        }

        protected XGTCnetRequestProtocol(byte[] binaryDatas)
            : base(binaryDatas)
        {
            Init();
        }

        protected XGTCnetRequestProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCommandType type)
            : base(localPort, cmd, type)
        {
            Init();
        }

        /// <summary>
        /// 변수 초기화
        /// </summary>
        private void Init()
        {
            ReqtDatas = new List<ReqtDataFmt>();
        }

        #region static factory construct method

        /// <summary>
        /// 직접 변수 개별 읽기 RSS
        /// PLC에서 데이터 타입에 맞게 직접 변수이름을 지정하여 읽는 요청의 프로토콜 입니다
        /// 한번에 16개의 독립된 디바이스 메모리를 읽을 수가 있습니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="enqs"> 변수이름과 메모리 주소가 담긴 구조체의 리스트 최대 16개까지 사용 가능합니다 </param>
        /// <returns> RSS 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetRequestProtocol NewRSSProtocol(ushort localPort, List<ReqtDataFmt> enqs)
        {
            if (enqs.Count == 0 || enqs == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (enqs.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCommandType.SS);
            protocol.ReqtDatas = enqs;
            protocol.BlockCount = (ushort)protocol.ReqtDatas.Count;
            return protocol;
        }

        /// <summary>
        /// 직접 변수 개별 읽기 RSS
        /// PLC에서 데이터 타입에 맞게 직접 변수이름을 지정하여 읽는 요청의 프로토콜 입니다
        /// 한번에 16개의 독립된 디바이스 메모리를 읽을 수가 있습니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="enqs"> 변수이름과 메모리 주소가 담긴 구조체, 하나의 변수만 읽습니다 </param>
        /// <returns> RSS 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetRequestProtocol NewRSSProtocol(ushort localPort, ReqtDataFmt enq)
        {
            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCommandType.SS);
            protocol.ReqtDatas.Add(enq);
            protocol.BlockCount = 1;
            return protocol;
        }

        /// <summary>
        /// 직접 변수 개별 쓰기 WSS
        /// PLC 의 메모리 번지를 직접 지정하여 데이터 타입에 맞게 값을 쓰는 프로토콜입니다.
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="enqs"> 변수이름과 메모리 주소가 담긴 구조체의 리스트 최대 16개까지 사용 가능합니다 </param>
        /// <returns> WSS 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetRequestProtocol NewWSSProtocol(ushort localPort, List<ReqtDataFmt> enqs)
        {
            if (enqs.Count == 0 || enqs == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCommandType.SS);
            protocol.ReqtDatas = enqs;
            protocol.BlockCount = (ushort)protocol.ReqtDatas.Count;
            return protocol;
        }

        /// <summary>
        /// 직접 변수 개별 쓰기 WSS
        /// PLC 의 메모리 번지를 직접 지정하여 데이터 타입에 맞게 값을 쓰는 프로토콜입니다.
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="enqs"> 변수이름과 메모리 주소가 담긴 구조체, 하나의 메모리에만 값을 씁니다 </param>
        /// <returns> RSS 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetRequestProtocol NewWSSProtocol(ushort localPort, ReqtDataFmt enq)
        {
            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCommandType.SS);
            protocol.ReqtDatas.Add(enq);
            protocol.BlockCount = 1;
            return protocol;
        }

        /// <summary>
        /// 직접 변수 연속 읽기 RSB
        /// PLC 에서 지정된 번지의 메모리에서 지정된 개수만큼 데이터를 일렬로 읽는 기능을 제공하는 프로토콜 입니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="varName"> 변수이름 </param>
        /// <param name="read_data_cnt"> 읽을 메모리 번지의 개수 </param>
        /// <returns> RSB 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetRequestProtocol NewRSBProtocol(ushort localPort, string varName, ushort read_data_cnt)
        {
            if (Glopa.GetDataType(varName) == PLCVarType.BIT)
                throw new ArgumentException("RSB COMMUNICATION NOT SUPPORTED BIT DATA TYPE");
            if ((read_data_cnt * (Glopa.GetDataType(varName)).ToSize() * 2) > CNET_PROTOCOL_SB_MAX_DATA_CNT)
                throw new ArgumentException(ERROR_CNET_PROTOCOL_SB_DATACNT_LIMIT);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCommandType.SB);
            protocol.ReqtDatas.Add(new ReqtDataFmt(varName));
            protocol.SizeOfData = read_data_cnt; // byte * 변수의 개수로 계산하는 것이 아니다. 변수의 개수만 
            return protocol;
        }

        /// <summary>
        /// 직접 변수 연속 쓰기 WSB
        /// PLC 의 메모리에서 지정된 번지로부터 지정된 길이만큼 데이터를 일렬로 쓰는 기능의 프로토콜 입니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="varName"> 변수이름 </param>
        /// <param name="data"> 값을 쓸 하나의 데이터 </param>
        /// <returns> WSB 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetRequestProtocol NewWSBProtocol(ushort localPort, string varName, object data)
        {
            if (!NumericTypeExtension.IsNumeric(data))
                throw new ArgumentException(NumericTypeExtension.ERROR_NOT_NEMERIC_TYPE);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCommandType.SB);
            protocol.ReqtDatas.Add(new ReqtDataFmt(varName, data));
            protocol.SizeOfData = 1;
            return protocol;
        }

        /// <summary>
        /// 직접 변수 연속 쓰기 WSB
        /// PLC 의 메모리에서 지정된 번지로부터 지정된 길이만큼 데이터를 일렬로 쓰는 기능의 프로토콜 입니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="enqs"> 변수이름과 메모리 주소가 담긴 구조체 </param>
        /// <returns> WSB 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetRequestProtocol NewWSBProtocol(ushort localPort, List<ReqtDataFmt> enqs)
        {
            if (enqs.Count == 0 || enqs == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);

            int size_sum = 0;
            foreach (var ed in enqs)
                size_sum += (Glopa.GetDataType(ed.GlopaVarName).ToSize() * 2);
            if (size_sum > CNET_PROTOCOL_SB_MAX_DATA_CNT)
                throw new ArgumentException(ERROR_CNET_PROTOCOL_SB_DATACNT_LIMIT);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCommandType.SB);
            protocol.ReqtDatas = enqs;
            protocol.SizeOfData = (ushort)enqs.Count;
            return protocol;
        }

        /// <summary>
        /// 모니터 변수 개별 등록 XSS
        /// 모니터 변수 등록은 변수 읽기 명령과 결합하여 최대 32개까지 개별 등록 시킬 수 있으며
        /// 등록 후 모니터 명령에 의해 등록된 것을 실행 시킵니다 
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="register"> 등록 번호 0 ~ 31까지 등록 가능합니다 이미 등록된 번호로 등록하면 현재 실행되는 것이 등록됩니다 </param>
        /// <param name="enqs"> 모니터 등록할 변수이름들의 리스트 </param>
        /// <returns> XSS 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetRequestProtocol NewXSSProtocol(ushort localPort, ushort register, List<ReqtDataFmt> enqs)
        {
            if (!(0 <= register && register <= MONITER_VAR_REGISTER_MAX_NUMBER))
                throw new ArgumentException(ERROR_MONITER_INVALID_REGISTER_NUMBER);

            var protocol = NewRSSProtocol(localPort, enqs);
            protocol.Command = XGTCnetCommand.X;
            protocol.CommandType = XGTCnetCommandType.SS;
            protocol.RegiNumber = register;
            return protocol;
        }

        /// <summary>
        /// 모니터 변수 개별 등록 XSB
        /// 모니터 변수 등록은 변수 읽기 명령과 결합하여 최대 32개까지 개별 등록 시킬 수 있으며
        /// 등록 후 모니터 명령에 의해 등록된 것을 실행 시킵니다 
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="register"> 등록 번호 0 ~ 31까지 등록 가능합니다 이미 등록된 번호로 등록하면 현재 실행되는 것이 등록됩니다 </param>
        /// <param name="varName"> 변수 이름 </param>
        /// <param name="read_data_cnt"> 등록할 메모리의 개수 </param>
        /// <returns> XSB 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetRequestProtocol NewXSBProtocol(ushort localPort, ushort register, string varName, ushort read_data_cnt)
        {
            if (!(0 <= register && register <= MONITER_VAR_REGISTER_MAX_NUMBER))
                throw new ArgumentException(ERROR_MONITER_INVALID_REGISTER_NUMBER);

            var protocol = NewRSBProtocol(localPort, varName, read_data_cnt);
            protocol.Command = XGTCnetCommand.X;
            protocol.CommandType = XGTCnetCommandType.SB;
            protocol.RegiNumber = register;
            return protocol;
        }

        /// <summary>
        /// 모니터 실행 YSS
        /// 모니터 실행은 모니터 등록으로 등록된 디바이스 읽기 기능을 실행시키는 기능을 가집니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="register"> 등록 번호 0 ~ 31 사이입니다 </param>
        /// <returns></returns>
        public static XGTCnetRequestProtocol NewYSSProtocol(ushort localPort, ushort register)
        {
            if (!(0 <= register && register <= MONITER_VAR_REGISTER_MAX_NUMBER))
                throw new ArgumentException(ERROR_MONITER_INVALID_REGISTER_NUMBER);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.Y, XGTCnetCommandType.SS);
            protocol.RegiNumber = register;
            return protocol;
        }

        /// <summary>
        /// 모니터 실행 YSB
        /// 모니터 실행은 모니터 등록으로 등록된 디바이스 읽기 기능을 실행시키는 기능을 가집니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="register"> 등록 번호 0 ~ 31 사이입니다 </param>
        /// <returns></returns>
        public static XGTCnetRequestProtocol NewYSBProtocol(ushort localPort, ushort register, PLCVarType plcVarType)
        {
            if (!(0 <= register && register <= MONITER_VAR_REGISTER_MAX_NUMBER))
                throw new ArgumentException(ERROR_MONITER_INVALID_REGISTER_NUMBER);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.Y, XGTCnetCommandType.SB);
            protocol.RegiNumber = register;

            // 데이터 형식 유추를 위해 가상의 변수를 넣는다. ㅡ_ㅡ;
            var virtual_varName = LD2Glopa.VarConvert("M0", plcVarType);
            protocol.ReqtDatas.Add(new ReqtDataFmt(virtual_varName));
            return protocol;
        }

        /// <summary>
        /// PLC에서 받은 원시 데이터를 분석하여 XGTCnetExclusiveProtocol 클래스로 변환&데이터분석하여 응답 프로토콜 클래스 리턴.
        /// </summary>
        /// <param name="binaryData"> 원시데이터 </param>
        /// <param name="reqtProtocol"> 요청 프로토콜 클래스 </param>
        /// <returns> 응답 프로토콜 클래스 </returns>
        internal static XGTCnetRequestProtocol CreateRequestProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCommandType type)
        {
            XGTCnetRequestProtocol protocol = new XGTCnetRequestProtocol(localPort, cmd, type);
            protocol.Header = XGTCnetControlCodeType.ENQ;
            protocol.Tail = XGTCnetControlCodeType.EOT;
            return protocol;
        }

        #endregion

        #region For ENQ Protocol Type

        protected void AddProtocolRSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ReqtDatas.Count, typeof(ushort)));               // 블록 수
            foreach (ReqtDataFmt e in ReqtDatas)
            {
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName.Length, typeof(ushort)));
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName));
            }
        }

        protected void AddProtocolWSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ReqtDatas.Count, typeof(ushort)));               // 블록 수
            foreach (ReqtDataFmt e in ReqtDatas)
            {
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName.Length, typeof(ushort)));
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName));
                asc_list.AddRange(CA2C.ToASC(e.Data, (PLCVarType)(Glopa.GetDataType(e.GlopaVarName).ToSize() * 2)));
            }
        }

        protected void AddProtocolXSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(RegiNumber));
            asc_list.Add((byte)'R');
            asc_list.Add((byte)'S');
            asc_list.Add((byte)'S');
            asc_list.AddRange(CA2C.ToASC(ReqtDatas.Count, typeof(ushort)));
            foreach (ReqtDataFmt e in ReqtDatas)
            {
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName.Length, typeof(ushort)));
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName));
            }
        }

        protected void AddProtocolYSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(RegiNumber));
        }

        // not support bit data
        protected void AddProtocolRSB(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ReqtDatas[0].GlopaVarName.Length, typeof(ushort)));
            asc_list.AddRange(CA2C.ToASC(ReqtDatas[0].GlopaVarName));
            asc_list.AddRange(CA2C.ToASC(SizeOfData));
        }

        protected void AddProtocolWSB(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ReqtDatas[0].GlopaVarName.Length, typeof(ushort)));
            asc_list.AddRange(CA2C.ToASC(ReqtDatas[0].GlopaVarName));
            asc_list.AddRange(CA2C.ToASC(SizeOfData));
            foreach (ReqtDataFmt e in ReqtDatas)
                asc_list.AddRange(CA2C.ToASC(e.Data, (PLCVarType)(Glopa.GetDataType(e.GlopaVarName).ToSize() * 2)));
        }

        protected void AddProtocolXSB(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(RegiNumber));
            asc_list.Add((byte)'R');
            asc_list.Add((byte)'S');
            asc_list.Add((byte)'B');

            asc_list.AddRange(CA2C.ToASC(ReqtDatas[0].GlopaVarName.Length, typeof(ushort)));
            asc_list.AddRange(CA2C.ToASC(ReqtDatas[0].GlopaVarName));
            asc_list.AddRange(CA2C.ToASC(SizeOfData));
        }

        protected void AddProtocolYSB(List<byte> asc_list)
        {
            AddProtocolYSS(asc_list);
        }

        protected override void AttachProtocolFrame(List<byte> asc_list)
        {
            switch (CommandType)
            {
                case XGTCnetCommandType.SS:
                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        AddProtocolRSS(asc_list);
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        AddProtocolWSS(asc_list);
                    else if (Command == XGTCnetCommand.X || Command == XGTCnetCommand.x)
                        AddProtocolXSS(asc_list);
                    else if (Command == XGTCnetCommand.Y || Command == XGTCnetCommand.y)
                        AddProtocolYSS(asc_list);
                    break;
                case XGTCnetCommandType.SB:
                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        AddProtocolRSB(asc_list);
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        AddProtocolWSB(asc_list);
                    else if (Command == XGTCnetCommand.X || Command == XGTCnetCommand.x)
                        AddProtocolXSB(asc_list);
                    else if (Command == XGTCnetCommand.Y || Command == XGTCnetCommand.y)
                        AddProtocolYSB(asc_list);
                    break;
            }
        }

        protected override void DetachProtocolFrame()
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(false);
#endif
        }

        #endregion
        protected override void PrintBinaryMainInfo()
        {
            Console.WriteLine(string.Format("블록 수: {0}", BlockCount));
            Console.WriteLine(string.Format("등록 번호: {0}", RegiNumber));
            Console.WriteLine(string.Format("데이터 개수: {0}", SizeOfData));

            int cnt = 0;
            foreach (ReqtDataFmt e in ReqtDatas)
            {
                Console.Write(string.Format("[{0:D2}] VAR_NAME: {1}", ++cnt, e.GlopaVarName));
                if (e.Data != null)
                    Console.WriteLine(string.Format(", VALUE: {0}", e.Data));
                else
                    Console.WriteLine("");
            }
        }
    }
}
