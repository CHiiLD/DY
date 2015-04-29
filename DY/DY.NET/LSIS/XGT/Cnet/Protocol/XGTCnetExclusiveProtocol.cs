/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜 RWSB RWSS XYSS XYSB 에 사용할 수 있는 범용 클래스
 * 날짜: 2015-03-31
 */

using System;
using System.Collections.Generic;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet 통신을 위한 프로토콜 클래스
    /// 단숨함을 위해 데이터와 기능을 함께 넣음
    /// </summary>
    public class XGTCnetExclusiveProtocol : XGTCnetExclusiveProtocolFrame
    {
        //공통형 정보
        public ushort BlockCnt { protected set; get; }      //2byte
        public ushort RegisterNum { protected set; get; }   //2byte
        public ushort DataCnt { protected set; get; }       //읽거나 쓸 데이터의 개수 (BYTE = 데이터 타입 * 개수) 최대 240byte word는 120byte 가 한계 //2byte
        
        //종류별 정보
        public List<ENQDataFormat> ENQDatas = new List<ENQDataFormat>(); //?byte
        public List<ACKDataFormat> ACKDatas = new List<ACKDataFormat>(); //?byte
        public XGTCnetExclusiveProtocol ReqtProtocol; //응답 프로토콜일 경우 요청프로토콜 주소를 저장하는 변수

        /// <summary>
        /// XGTCnetExclusiveProtocol 복사생성자
        /// </summary>
        /// <param name="that"> 복사하고자 할 XGTCnetExclusiveProtocol 객체 </param>
        public XGTCnetExclusiveProtocol(XGTCnetExclusiveProtocol that)
            : base(that)
        {
            this.BlockCnt = that.BlockCnt;
            this.ENQDatas.AddRange(that.ENQDatas);
            this.ACKDatas.AddRange(that.ACKDatas);
            this.RegisterNum = that.RegisterNum;
            this.DataCnt = that.DataCnt;
            this.ReqtProtocol = that.ReqtProtocol;
        }

        protected XGTCnetExclusiveProtocol()
            : base()
        {
        }

        protected XGTCnetExclusiveProtocol(byte[] binaryDatas)
            : base(binaryDatas)
        {
        }

        protected XGTCnetExclusiveProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCommandType type)
            : base(localPort, cmd, type)
        {
        }

        #region allocClass
        public static XGTCnetExclusiveProtocol GetRSSProtocol(ushort localPort, List<ENQDataFormat> enqDatas)
        {
            if (enqDatas.Count == 0 || enqDatas == null)
                throw new ArgumentException("enqDatas argument have problem(null or empty data)");

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCommandType.SS);
            protocol.ENQDatas =  enqDatas;
            protocol.BlockCnt = (ushort)protocol.ENQDatas.Count;
            return protocol;
        }

        public static XGTCnetExclusiveProtocol GetRSSProtocol(ushort localPort, ENQDataFormat enqData)
        {
            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCommandType.SS);
            protocol.ENQDatas.Add(enqData);
            protocol.BlockCnt = 1;
            return protocol;
        }

        public static XGTCnetExclusiveProtocol GetWSSProtocol(ushort localPort, List<ENQDataFormat> enqDatas)
        {
            if (enqDatas.Count == 0 || enqDatas == null)
                throw new ArgumentException("enqDatas argument have problem(null or empty data)");

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCommandType.SS);
            protocol.ENQDatas = enqDatas;
            protocol.BlockCnt = (ushort)protocol.ENQDatas.Count;
            return protocol;
        }

        public static XGTCnetExclusiveProtocol GetWSSProtocol(ushort localPort, ENQDataFormat enqData)
        {
            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCommandType.SS);
            protocol.ENQDatas.Add(enqData);
            protocol.BlockCnt = 1;
            return protocol;
        }

        public static XGTCnetExclusiveProtocol GetRSBProtocol(ushort localPort, string varName, ushort read_data_cnt)
        {
            if (Glopa.GetDataType(varName) == DataType.BIT)
                throw new ArgumentException("RSB communication not supported bit data type");
            if ((read_data_cnt * (Glopa.GetDataType(varName)).SizeOf() * 2) > PROTOCOL_SB_DATACNT_LIMIT)
                throw new ArgumentException("data count(asc bytes) limited 240byte");

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCommandType.SB);
            protocol.ENQDatas.Add(new ENQDataFormat(varName));
            protocol.DataCnt = read_data_cnt;
            return protocol;
        }

        public static XGTCnetExclusiveProtocol GetWSBProtocol(ushort localPort, string varName, object data)
        {
            if (!NumericTypeExtension.IsNumeric(data))
                throw new ArgumentException("data is not numeric type");

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCommandType.SB);
            protocol.ENQDatas.Add(new ENQDataFormat(varName, data));
            protocol.DataCnt = 1;
            return protocol;
        }

        public static XGTCnetExclusiveProtocol GetWSBProtocol(ushort localPort, List<ENQDataFormat> enqDatas)
        {
            if (enqDatas.Count == 0 || enqDatas == null)
                throw new ArgumentException("enqDatas argument have problem(null or empty data)");

            int size_sum = 0;
            foreach(var ed in enqDatas)
                size_sum += (Glopa.GetDataType(ed.GlopaVarName).SizeOf() * 2);
            if (size_sum > PROTOCOL_SB_DATACNT_LIMIT)
                throw new ArgumentException("data count(asc bytes) limited 240byte");

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCommandType.SB);
            protocol.ENQDatas = enqDatas;
            protocol.DataCnt = (ushort)enqDatas.Count;
            return protocol;
        }


        /// <summary>
        /// PLC에서 받은 원시 데이터를 분석하여 XGTCnetExclusiveProtocol 클래스로 변환&데이터분석하여 응답 프로토콜 클래스 리턴.
        /// </summary>
        /// <param name="binaryData"> 원시데이터 </param>
        /// <param name="reqtProtocol"> 요청 프로토콜 클래스 </param>
        /// <returns> 응답 프로토콜 클래스 </returns>
        public static XGTCnetExclusiveProtocol CreateReceiveProtocol(byte[] binaryData, XGTCnetExclusiveProtocol reqtProtocol)
        {
            XGTCnetExclusiveProtocol protocol = new XGTCnetExclusiveProtocol(binaryData);
            protocol.ReqtProtocol = reqtProtocol;
            return protocol;
        }

        protected static XGTCnetExclusiveProtocol CreateRequestProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCommandType type)
        {
            XGTCnetExclusiveProtocol protocol = new XGTCnetExclusiveProtocol(localPort, cmd, type);
            protocol.Header = XGTCnetControlCodeType.ENQ;
            protocol.Tail = XGTCnetControlCodeType.EOT;
            return protocol;
        }

        #endregion

#if flase
        protected static XGTCnetExclusiveProtocol CreateACKProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCommandType type)
        {
            XGTCnetExclusiveProtocol protocol = new XGTCnetExclusiveProtocol(localPort, cmd, type);
            protocol.Header = XGTCnetControlCodeType.ACK;
            protocol.Tail = XGTCnetControlCodeType.ETX;
            return protocol;
        }

        public static XGTCnetExclusiveProtocol CreateACKProtocol(byte[] recvASCData)
        {
            XGTCnetExclusiveProtocol protocol = new XGTCnetExclusiveProtocol();
            protocol.AnalysisProtocol(recvASCData);
            return protocol;
        }

        protected static XGTCnetExclusiveProtocol CreateNAKProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCommandType type)
        {
            XGTCnetExclusiveProtocol protocol = new XGTCnetExclusiveProtocol(localPort, cmd, type);
            protocol.Header = XGTCnetControlCodeType.NAK;
            protocol.Tail = XGTCnetControlCodeType.ETX;
            return protocol;
        }
#endif
        #region For ENQ Protocol Type

        protected void AddProtocolRSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ENQDatas.Count, typeof(ushort)));               // 블록 수
            foreach (ENQDataFormat e in ENQDatas)
            {
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName.Length, typeof(ushort)));
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName));
            }
        }

        protected void AddProtocolWSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ENQDatas.Count, typeof(ushort)));               // 블록 수
            foreach (ENQDataFormat e in ENQDatas)
            {
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName.Length, typeof(ushort)));
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName));
                asc_list.AddRange(CA2C.ToASC(e.Data));
            }
        }

        protected void AddProtocolXSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(RegisterNum));
            asc_list.Add((byte)'R');
            asc_list.Add((byte)'S');
            asc_list.Add((byte)'S');
            asc_list.AddRange(CA2C.ToASC(ENQDatas.Count, typeof(ushort)));
            foreach (ENQDataFormat e in ENQDatas)
            {
                if (e.GlopaVarName.Length > 16)
                    throw new ArgumentOutOfRangeException("Var_Name's length over 16.");
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName.Length, typeof(ushort)));
                asc_list.AddRange(CA2C.ToASC(e.GlopaVarName));
            }
        }

        protected void AddProtocolYSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(RegisterNum));
        }

        // not support bit data
        protected void AddProtocolRSB(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ENQDatas[0].GlopaVarName.Length, typeof(ushort)));
            asc_list.AddRange(CA2C.ToASC(ENQDatas[0].GlopaVarName));
            asc_list.AddRange(CA2C.ToASC(DataCnt));
        }

        protected void AddProtocolWSB(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ENQDatas[0].GlopaVarName.Length, typeof(ushort)));
            asc_list.AddRange(CA2C.ToASC(ENQDatas[0].GlopaVarName));
            asc_list.AddRange(CA2C.ToASC(DataCnt));
            foreach (ENQDataFormat f in ENQDatas)
                asc_list.AddRange(CA2C.ToASC(f.Data));
        }

        protected void AddProtocolXSB(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(RegisterNum));
            asc_list.Add((byte)'R');
            asc_list.Add((byte)'S');
            asc_list.Add((byte)'B');

            if (ENQDatas[0].GlopaVarName.Length > 16)
                throw new ArgumentOutOfRangeException("Var_Name's length over 16.");
            asc_list.AddRange(CA2C.ToASC(ENQDatas[0].GlopaVarName.Length, typeof(ushort)));
            asc_list.AddRange(CA2C.ToASC(ENQDatas[0].GlopaVarName));
            asc_list.AddRange(CA2C.ToASC(DataCnt));
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
        #endregion

        #region For ACK Protocol Type

        protected void QueryProtocolRSS()
        {
            byte[] data = GetMainData();
            byte[] block_arr = { data[0], data[1] };
            BlockCnt = (ushort)CA2C.ToValue(block_arr, typeof(ushort));

            int data_idx = 2;
            for (int i = 0; i < BlockCnt; i++)
            {
                byte[] data_size_arr = { data[data_idx + 0], data[data_idx + 1] };
                data_idx += 2;
                ushort sizeOfType = (ushort)CA2C.ToValue(data_size_arr, typeof(ushort));

                byte[] data_arr = new byte[sizeOfType * 2];
                Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                data_idx += data_arr.Length;
                object value = CA2C.ToValue(data_arr, DataTypeExtensions.TypeOf((DataType)sizeOfType));

                ACKDatas.Add(new ACKDataFormat(sizeOfType, value));
            }
        }

        protected void QueryProtocolWSS()
        {
            return;
        }

        protected void QueryProtocolXSS()
        {
            byte[] data = GetMainData();
            byte[] register_arr = { data[0], data[1] };
            RegisterNum = (ushort)CA2C.ToValue(register_arr, typeof(ushort));
        }

        protected void QueryProtocolYSS()
        {
            byte[] data = GetMainData();
            byte[] register_arr = { data[0], data[1] };
            RegisterNum = (ushort)CA2C.ToValue(register_arr, typeof(ushort));

            byte[] block_arr = { data[2], data[3] };
            BlockCnt = (ushort)CA2C.ToValue(block_arr, typeof(ushort));

            int data_idx = 4;

            for (int i = 0; i < BlockCnt; i++)
            {
                byte[] data_size_arr = { data[4], data[5] };
                data_idx += 2;
                ushort sizeOfType = (ushort)CA2C.ToValue(data_size_arr, typeof(ushort));

                byte[] data_arr = new byte[sizeOfType * 2];
                Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                object value = CA2C.ToValue(data_arr, DataTypeExtensions.TypeOf((DataType)sizeOfType));
                data_idx += data_arr.Length;

                ACKDatas.Add(new ACKDataFormat(sizeOfType, value));
            }
        }

        /*
         * RSB ACK 
         * 데이터 개수 - HEX형의 Byte 개수를 의미하며 ASC2로 변환 되어 있습니다.
         * 이 개수는 Byte의 수를 의미합니다.
         * 
         * 데이터 - 데이터 영역에는 Hex데이터를 ASC2코드로 변환된 값이 들어 있습니다.
         * 사용 예1 - PC요구포맷의 직접 변수 이름에 포함되어 있는 메모리 타입이 WORD이고 PC요구포맷의 
         * 데이터 개수가 03인 경우 명령 실행 후 PLC ACK응답의 데이터의 개수는 06 = (2 * 03)로 전달되고 
         * 이 값의 ASC2코드 값 3036으로 들어 있게 됩니다.
         * 4 // 30 30 44 34 30 30 44 34
         * 
         * 메뉴얼을 보면 예제가 2개 있는데 ACK응답에서 하나는 블록수가 들어있고 또 하나는 블록 수가 없습니다.
         * LS산전 측이 실수한 것으로 생각됩니다. 일단 예제에선 블록수도 같이 오는데 도대체 어떻게 처리를 하라는건지 알 수가 없습니다.
         * 파악되는 즉시 수정해야 합니다. 일단은 자료형에 맞추어 ASC데이터를 적절하게 끊어서 컨버트 처리합니다.
         */

        protected void QueryProtocolRSB()
        {
            byte[] data = GetMainData();

            // 블록 수 정보 쿼리
            byte[] bcnt_arr = { data[0], data[1] };
            BlockCnt = (ushort)CA2C.ToValue(bcnt_arr, typeof(ushort)); // 정말 이해할 수 없는 정보.. LSIS 측에 문의해봐야함

            // 데이터 개수 정보 쿼리
            byte[] size_arr = { data[2], data[3] };
            ushort data_len = (ushort)CA2C.ToValue(size_arr, typeof(ushort));

            // 그로파 변수이름에서 자료형 정보를 얻어낸다
            string var_name = ReqtProtocol.ENQDatas[0].GlopaVarName;
            DataType data_type = Glopa.GetDataType(var_name);
            int data_type_size = DataTypeExtensions.SizeOf(data_type);

            int data_idx = 4;
            // 일렬로 정렬된 데이터들을 데이터타입에 따라 적절하게 파싱하여 데이터 쿼리
            for (int i = 0; i < data_len / data_type_size; i++)
            {
                byte[] temp_arr = new byte[data_type_size * 2];
                Buffer.BlockCopy(data, data_idx, temp_arr, 0, temp_arr.Length);
                object value = CA2C.ToValue(temp_arr, DataTypeExtensions.TypeOf(data_type));
                ACKDatas.Add(new ACKDataFormat((ushort)data_type_size, value));
                data_idx += temp_arr.Length;
            }
        }

        protected void QueryProtocolWSB()
        {
            return;
        }

        protected void QueryProtocolXSB()
        {
            byte[] data = GetMainData();
            byte[] register_arr = { data[0], data[1] };
            RegisterNum = (ushort)CA2C.ToValue(register_arr, typeof(ushort));
        }

        protected void QueryProtocolYSB()
        {
            byte[] data = GetMainData();
            byte[] register_arr = { data[0], data[1] };
            RegisterNum = (ushort)CA2C.ToValue(register_arr, typeof(ushort));

            byte[] data_size_arr = { data[2], data[3] };
            ushort sizeOfType = (ushort)CA2C.ToValue(data_size_arr, typeof(ushort));

            byte[] data_arr = new byte[sizeOfType * 2];
            Buffer.BlockCopy(data, data_size_arr.Length + register_arr.Length, data_arr, 0, data_arr.Length);
            object value = CA2C.ToValue(data_arr, DataTypeExtensions.TypeOf((DataType)sizeOfType));

            ACKDatas.Add(new ACKDataFormat(sizeOfType, value));
        }

        protected override void DetachProtocolFrame()
        {
            switch (CommandType)
            {
                case XGTCnetCommandType.SS:

                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        QueryProtocolRSS();
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        QueryProtocolWSS();
                    else if (Command == XGTCnetCommand.X || Command == XGTCnetCommand.x)
                        QueryProtocolXSS();
                    else if (Command == XGTCnetCommand.Y || Command == XGTCnetCommand.y)
                        QueryProtocolYSS();
                    break;
                case XGTCnetCommandType.SB:
                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        QueryProtocolRSB();
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        QueryProtocolWSB();
                    else if (Command == XGTCnetCommand.X || Command == XGTCnetCommand.x)
                        QueryProtocolXSB();
                    else if (Command == XGTCnetCommand.Y || Command == XGTCnetCommand.y)
                        QueryProtocolYSB();
                    break;
            }
        }

        protected override void PrintBinaryDataInfo()
        {
            Console.WriteLine(string.Format("Block number: {0}", BlockCnt));
            Console.WriteLine(string.Format("Register number: {0}", RegisterNum));
            Console.WriteLine(string.Format("Data size: {0}", DataCnt));

            int cnt = 0;
            foreach (ENQDataFormat e in ENQDatas)
            {
                Console.WriteLine(string.Format("[{0}] ENQData variable name: {1}", ++cnt, e.GlopaVarName));
                if (e.Data != null)
                    Console.WriteLine(string.Format("[{0}] ENQData data: {1}", cnt, e.Data));
            }

            cnt = 0;
            foreach (ACKDataFormat a in ACKDatas)
            {
                Console.WriteLine(string.Format("[{0}] ACKData size of type : {1}", ++cnt, a.SizeOfType));
                if (a.Data != null)
                    Console.WriteLine(string.Format("[{0}] ACKData data: {1}", cnt, a.Data));
            }
        }
        #endregion
    }
}
