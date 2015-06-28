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
    /// </summary>
    public class XGTCnetProtocol : AXGTCnetProtocol
    {
        private const int MONITER_VAR_REGISTER_MAX_NUMBER = 31;
        
        #region PUBLIC PROPERTIES
        /// <summary>
        /// PROTOCOL MAIN DATAS
        /// </summary>
        public ushort BlocCnt { private set; get; }   //2byte
        public ushort DataCnt { private set; get; }   //읽거나 쓸 데이터의 개수 (BYTE = 데이터 타입 * 개수) 최대 240byte word는 120byte 가 한계 //2byte
        public ushort RegiNum { private set; get; }   //등록 번호    2byte
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// XGTCnetExclusiveProtocol 복사생성자
        /// </summary>
        /// <param name="that"> 복사하고자 할 XGTCnetExclusiveProtocol 객체 </param>
        internal XGTCnetProtocol(XGTCnetProtocol that)
            : base(that)
        {
            BlocCnt = that.BlocCnt;
            DataCnt = that.DataCnt;
            RegiNum = that.RegiNum;
        }

        private XGTCnetProtocol()
            : base()
        {
        }

        private XGTCnetProtocol(byte[] binaryDatas)
            : base(binaryDatas)
        {
        }

        private XGTCnetProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCmdType type)
            : base(localPort, cmd, type)
        {
        }

        #endregion

        #region STATIC FACTORY CONSTRUCT METHOD
        /// <summary>
        /// 직접 변수 개별 읽기 RSS
        /// PLC에서 데이터 타입에 맞게 직접 변수이름을 지정하여 읽는 요청의 프로토콜 입니다
        /// 한번에 16개의 독립된 디바이스 메모리를 읽을 수가 있습니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="pvalues"> 변수이름과 메모리 주소가 담긴 구조체의 리스트 최대 16개까지 사용 가능합니다 </param>
        /// <returns> RSS 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetProtocol NewRSSProtocol(ushort localPort, List<PValue> pvalues)
        {
            if (pvalues.Count == 0 || pvalues == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (pvalues.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCmdType.SS);
            protocol.ReqeustList.AddRange(pvalues); //깊은 복사
            protocol.BlocCnt = (ushort)protocol.ReqeustList.Count;
            return protocol;
        }

        /// <summary>
        /// 직접 변수 개별 쓰기 WSS
        /// PLC 의 메모리 번지를 직접 지정하여 데이터 타입에 맞게 값을 쓰는 프로토콜입니다.
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="pvalues"> 변수이름과 메모리 주소가 담긴 구조체의 리스트 최대 16개까지 사용 가능합니다 </param>
        /// <returns> WSS 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetProtocol NewWSSProtocol(ushort localPort, List<PValue> pvalues)
        {
            if (pvalues.Count == 0 || pvalues == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (pvalues.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCmdType.SS);
            protocol.ReqeustList.AddRange(pvalues);
            protocol.BlocCnt = (ushort)protocol.ReqeustList.Count;
            return protocol;
        }

        /// <summary>
        /// 직접 변수 연속 읽기 RSB
        /// PLC 에서 지정된 번지의 메모리에서 지정된 개수만큼 데이터를 일렬로 읽는 기능을 제공하는 프로토콜 입니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="glopa_name"> 변수이름 </param>
        /// <param name="block_cnt"> 읽을 메모리 번지의 개수 </param>
        /// <returns> RSB 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetProtocol NewRSBProtocol(ushort localPort, PValue pvalue, ushort block_cnt)
        {
            string glopa_name = pvalue.Name;
            Type type = pvalue.Type;
            if (pvalue.Type == typeof(Boolean)) //BIT 데이터는 연속 읽기를 할 수 없어요 ㅠ_ㅠ
                throw new ArgumentException("RSB COMMUNICATION NOT SUPPORTED BIT DATA TYPE");
            int buf_size = (block_cnt * pvalue.Type.ToSize() * 2);
            if (buf_size > PROTOCOL_SB_SIZE_MAX_240BYTE)
                throw new ArgumentException(ERROR_PROTOCOL_SB_SIZE_MAX_240BYTE);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCmdType.SB);
            protocol.DataCnt = block_cnt;
            for (int i = 0; i < block_cnt; i++)
            {
                string str_header = glopa_name.Substring(0, 3);
                string str_num = glopa_name.Substring(3, glopa_name.Length - 3);
                int mem_num;
                if (Int32.TryParse(str_num, out mem_num))
                    protocol.ReqeustList.Add(new PValue() { Name = str_header + (mem_num + i).ToString(), Type = type });
            }
            return protocol;
        }

        /// <summary>
        /// 직접 변수 연속 쓰기 WSB
        /// PLC 의 메모리에서 지정된 번지로부터 지정된 길이만큼 데이터를 일렬로 쓰는 기능의 프로토콜 입니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="pvalues"> 변수이름과 메모리 주소가 담긴 구조체 </param>
        /// <returns> WSB 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetProtocol NewWSBProtocol(ushort localPort, List<PValue> pvalues)
        {
            if (pvalues.Count == 0 || pvalues == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);

            int size_sum = 0;
            foreach (var pv in pvalues)
                size_sum += pv.Type.ToSize() * 2;
            if (size_sum > PROTOCOL_SB_SIZE_MAX_240BYTE)
                throw new ArgumentException(ERROR_PROTOCOL_SB_SIZE_MAX_240BYTE);

            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCmdType.SB);
            protocol.ReqeustList.AddRange(pvalues);
            protocol.DataCnt = (ushort)pvalues.Count;
            return protocol;
        }

        /// <summary>
        /// 모니터 변수 개별 등록 XSS
        /// 모니터 변수 등록은 변수 읽기 명령과 결합하여 최대 32개까지 개별 등록 시킬 수 있으며
        /// 등록 후 모니터 명령에 의해 등록된 것을 실행 시킵니다 
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="register_num"> 등록 번호 0 ~ 31까지 등록 가능합니다 이미 등록된 번호로 등록하면 현재 실행되는 것이 등록됩니다 </param>
        /// <param name="pvalues"> 모니터 등록할 변수이름들의 리스트 </param>
        /// <returns> XSS 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetProtocol NewXSSProtocol(ushort localPort, ushort register_num, List<PValue> pvalues)
        {
            if (!(0 <= register_num && register_num <= MONITER_VAR_REGISTER_MAX_NUMBER))
                throw new ArgumentException(ERROR_MONITER_INVALID_REGISTER_NUMBER);

            var protocol = NewRSSProtocol(localPort, pvalues);
            protocol.Command = XGTCnetCommand.X;
            protocol.CommandType = XGTCnetCmdType.SS;
            protocol.RegiNum = register_num;
            return protocol;
        }

        /// <summary>
        /// 모니터 변수 개별 등록 XSB
        /// 모니터 변수 등록은 변수 읽기 명령과 결합하여 최대 32개까지 개별 등록 시킬 수 있으며
        /// 등록 후 모니터 명령에 의해 등록된 것을 실행 시킵니다 
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="register_num"> 등록 번호 0 ~ 31까지 등록 가능합니다 이미 등록된 번호로 등록하면 현재 실행되는 것이 등록됩니다 </param>
        /// <param name="glopa_name"> 변수 이름 </param>
        /// <param name="block_cnd"> 등록할 메모리의 개수 </param>
        /// <returns> XSB 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetProtocol NewXSBProtocol(ushort localPort, ushort register_num, PValue pvalue, ushort block_cnd)
        {
            if (!(0 <= register_num && register_num <= MONITER_VAR_REGISTER_MAX_NUMBER))
                throw new ArgumentException(ERROR_MONITER_INVALID_REGISTER_NUMBER);

            var protocol = NewRSBProtocol(localPort, pvalue, block_cnd);
            protocol.Command = XGTCnetCommand.X;
            protocol.CommandType = XGTCnetCmdType.SB;
            protocol.RegiNum = register_num;
            return protocol;
        }

        /// <summary>
        /// 모니터 실행 YSS
        /// 모니터 실행은 모니터 등록으로 등록된 디바이스 읽기 기능을 실행시키는 기능을 가집니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="register_num"> 등록 번호 0 ~ 31 사이입니다 </param>
        /// <returns></returns>
        public static XGTCnetProtocol NewYSSProtocol(ushort localPort, ushort register_num, XGTCnetProtocol xss_protocol)
        {
            if (!(0 <= register_num && register_num <= MONITER_VAR_REGISTER_MAX_NUMBER))
                throw new ArgumentException(ERROR_MONITER_INVALID_REGISTER_NUMBER);
            if ((!(xss_protocol.Command == XGTCnetCommand.X || xss_protocol.Command == XGTCnetCommand.x)) || (xss_protocol.CommandType != XGTCnetCmdType.SS))
                throw new ArgumentException("XSS_PROTOCOL IS NOT XSS TYPE.");
            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.Y, XGTCnetCmdType.SS);
            protocol.RegiNum = register_num;
            protocol.ReqeustList.AddRange(xss_protocol.ReqeustList);
            return protocol;
        }

        /// <summary>
        /// 모니터 실행 YSB
        /// 모니터 실행은 모니터 등록으로 등록된 디바이스 읽기 기능을 실행시키는 기능을 가집니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="register"> 등록 번호 0 ~ 31 사이입니다 </param>
        /// <returns></returns>
        public static XGTCnetProtocol NewYSBProtocol(ushort localPort, ushort register, XGTCnetProtocol xsb_protocol)
        {
            if (!(0 <= register && register <= MONITER_VAR_REGISTER_MAX_NUMBER))
                throw new ArgumentException(ERROR_MONITER_INVALID_REGISTER_NUMBER);
            if ((!(xsb_protocol.Command == XGTCnetCommand.X || xsb_protocol.Command == XGTCnetCommand.x)) || (xsb_protocol.CommandType != XGTCnetCmdType.SB))
                throw new ArgumentException("XSS_PROTOCOL IS NOT XSS TYPE.");
            var protocol = CreateRequestProtocol(localPort, XGTCnetCommand.Y, XGTCnetCmdType.SB);
            protocol.RegiNum = register;
            protocol.ReqeustList.AddRange(xsb_protocol.ReqeustList);
            return protocol;
        }

        /// <summary>
        /// PLC에서 받은 원시 데이터를 분석하여 XGTCnetExclusiveProtocol 클래스로 변환&데이터분석하여 응답 프로토콜 클래스 리턴.
        /// </summary>
        /// <param name="binaryData"> 원시데이터 </param>
        /// <param name="reqtProtocol"> 요청 프로토콜 클래스 </param>
        /// <returns> 응답 프로토콜 클래스 </returns>
        internal static XGTCnetProtocol CreateReceiveProtocol(byte[] binaryData, XGTCnetProtocol reqtProtocol)
        {
            XGTCnetProtocol instance = new XGTCnetProtocol(binaryData);
            instance.ReqeustList.AddRange(reqtProtocol.ReqeustList);
            instance.OtherParty = reqtProtocol;
            return instance;
        }

        internal static XGTCnetProtocol CreateRequestProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCmdType type)
        {
            XGTCnetProtocol instance = new XGTCnetProtocol(localPort, cmd, type);
            instance.Header = XGTCnetCCType.ENQ;
            instance.Tail = XGTCnetCCType.EOT;
            return instance;
        }
        #endregion

        #region FOR REQUEST PROTOCOL TYPE

        private void AddProtocolRSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ReqeustList.Count, typeof(UInt16)));// 블록 수
            foreach (PValue e in ReqeustList)
            {
                asc_list.AddRange(CA2C.ToASC(e.Name.Length, typeof(UInt16)));// 변수 이름 길이
                asc_list.AddRange(CA2C.ToASC(e.Name));                       // 변수 이름 정보 
            }
        }

        private void AddProtocolWSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ReqeustList.Count, typeof(UInt16)));               // 블록 수
            foreach (PValue e in ReqeustList)
            {
                asc_list.AddRange(CA2C.ToASC(e.Name.Length, typeof(UInt16)));// 변수 이름 길이
                asc_list.AddRange(CA2C.ToASC(e.Name));                       // 변수 이름 정보 
                asc_list.AddRange(CA2C.ToASC(e.Value, e.Type));              // WRITE될 값 
            }
        }

        private void AddProtocolXSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(RegiNum));
            asc_list.Add((byte)'R');
            asc_list.Add((byte)'S');
            asc_list.Add((byte)'S');
            asc_list.AddRange(CA2C.ToASC(ReqeustList.Count, typeof(UInt16)));
            foreach (PValue e in ReqeustList)
            {
                asc_list.AddRange(CA2C.ToASC(e.Name.Length, typeof(UInt16)));
                asc_list.AddRange(CA2C.ToASC(e.Name));
            }
        }

        private void AddProtocolYSS(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(RegiNum));
        }

        // not support bit data
        private void AddProtocolRSB(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ReqeustList[0].Name.Length, typeof(UInt16)));
            asc_list.AddRange(CA2C.ToASC(ReqeustList[0].Name));
            asc_list.AddRange(CA2C.ToASC(DataCnt));
        }

        private void AddProtocolWSB(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(ReqeustList[0].Name.Length, typeof(UInt16)));
            asc_list.AddRange(CA2C.ToASC(ReqeustList[0].Name));
            asc_list.AddRange(CA2C.ToASC(DataCnt));
            foreach (PValue e in ReqeustList)
                asc_list.AddRange(CA2C.ToASC(e.Value, e.Type));
        }

        private void AddProtocolXSB(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(RegiNum));
            asc_list.Add((byte)'R');
            asc_list.Add((byte)'S');
            asc_list.Add((byte)'B');

            asc_list.AddRange(CA2C.ToASC(ReqeustList[0].Name.Length, typeof(UInt16)));
            asc_list.AddRange(CA2C.ToASC(ReqeustList[0].Name));
            asc_list.AddRange(CA2C.ToASC(DataCnt));
        }

        private void AddProtocolYSB(List<byte> asc_list)
        {
            AddProtocolYSS(asc_list);
        }

        protected override void AttachProtocolFrame(List<byte> asc_list)
        {
            switch (CommandType)
            {
                case XGTCnetCmdType.SS:
                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        AddProtocolRSS(asc_list);
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        AddProtocolWSS(asc_list);
                    else if (Command == XGTCnetCommand.X || Command == XGTCnetCommand.x)
                        AddProtocolXSS(asc_list);
                    else if (Command == XGTCnetCommand.Y || Command == XGTCnetCommand.y)
                        AddProtocolYSS(asc_list);
                    break;
                case XGTCnetCmdType.SB:
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

        #region FOR RESPONSE PROTOCOL TYPE

        private void QueryProtocolRSS()
        {
            byte[] data = GetMainData();
            BlocCnt = (ushort)CA2C.ToValue(new byte[] { data[0], data[1] }, typeof(UInt16));
            int data_idx = 2;
            for (int i = 0; i < BlocCnt; i++)
            {
                ushort sizeOfType = (ushort)CA2C.ToValue(new byte[] { data[data_idx + 0], data[data_idx + 1] }, typeof(ushort));
                data_idx += 2;
                byte[] data_arr = new byte[sizeOfType * 2];
                Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                data_idx += data_arr.Length;
                ResponseDic.Add(ReqeustList[i].Name, CA2C.ToValue(data_arr, ReqeustList[i].Type));
            }
        }

        private void QueryProtocolWSS()
        {
            return;
        }

        private void QueryProtocolXSS()
        {
            byte[] type = GetMainData(); // { ProtocolData[4], ProtocolData[5] };
            RegiNum = (ushort)CA2C.ToValue(type, typeof(UInt16));
        }

        private void QueryProtocolYSS()
        {
            byte[] data = GetMainData();
            RegiNum = (ushort)CA2C.ToValue(new byte[] { data[0], data[1] }, typeof(UInt16));
            BlocCnt = (ushort)CA2C.ToValue(new byte[] { data[2], data[3] }, typeof(UInt16));
            int data_idx = 4;
            for (int i = 0; i < BlocCnt; i++)
            {
                ushort sizeOfType = (ushort)CA2C.ToValue(new byte[] { data[data_idx + 0], data[data_idx + 1] }, typeof(UInt16));
                data_idx += 2;
                byte[] data_arr = new byte[sizeOfType * 2];
                Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                data_idx += data_arr.Length;
                ResponseDic.Add(ReqeustList[i].Name, CA2C.ToValue(data_arr, ReqeustList[i].Type));
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

        private void QueryProtocolRSB()
        {
            byte[] data = GetMainData();
            ushort data_len = (ushort)CA2C.ToValue(new byte[] { data[2], data[3] }, typeof(UInt16));// 데이터 개수 정보 쿼리
            int data_idx = 4;
            int data_type_size = ReqeustList[0].Type.ToSize();
            // 일렬로 정렬된 데이터들을 데이터타입에 따라 적절하게 파싱하여 데이터 쿼리
            for (int i = 0; i < data_len / data_type_size; i++)
            {
                byte[] temp_arr = new byte[data_type_size * 2];
                Buffer.BlockCopy(data, data_idx, temp_arr, 0, temp_arr.Length);
                ResponseDic.Add(ReqeustList[i].Name, CA2C.ToValue(temp_arr, ReqeustList[i].Type));
                data_idx += temp_arr.Length;
            }
        }

        private void QueryProtocolWSB()
        {
            return;
        }

        private void QueryProtocolXSB()
        {
            QueryProtocolXSS();
        }

        private void QueryProtocolYSB()
        {
            var data = GetMainData();
            // 등록 번호 정보 쿼리
            RegiNum = (ushort)CA2C.ToValue(new byte[] { data[0], data[1] }, typeof(UInt16));
            // 데이터 개수 정보 쿼리
            ushort data_len = (ushort)CA2C.ToValue(new byte[] { data[2], data[3] }, typeof(UInt16));
            // 그로파 변수이름에서 자료형 정보를 얻어낸다
            int data_type_size = ReqeustList[0].Type.ToSize();
            int data_idx = 4;
            // 일렬로 정렬된 데이터들을 데이터타입에 따라 적절하게 파싱하여 데이터 쿼리
            for (int i = 0; i < data_len / data_type_size; i++)
            {
                byte[] temp_arr = new byte[data_type_size * 2];
                Buffer.BlockCopy(data, data_idx, temp_arr, 0, temp_arr.Length);
                ResponseDic.Add(ReqeustList[i].Name, CA2C.ToValue(temp_arr, ReqeustList[i].Type));
                data_idx += temp_arr.Length;
            }
        }

        protected override void DetachProtocolFrame()
        {
            switch (CommandType)
            {
                case XGTCnetCmdType.SS:

                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        QueryProtocolRSS();
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        QueryProtocolWSS();
                    else if (Command == XGTCnetCommand.X || Command == XGTCnetCommand.x)
                        QueryProtocolXSS();
                    else if (Command == XGTCnetCommand.Y || Command == XGTCnetCommand.y)
                        QueryProtocolYSS();
                    break;
                case XGTCnetCmdType.SB:
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
        #endregion

        protected override void PrintBinaryMainInfo()
        {
            Console.WriteLine(string.Format("블록 수: {0}", BlocCnt));
            Console.WriteLine(string.Format("등록 번호: {0}", RegiNum));
            Console.WriteLine(string.Format("데이터 개수: {0}", DataCnt));
            int cnt = 0;
            switch (Header)
            {
                case XGTCnetCCType.ENQ:
                    foreach (var data in ReqeustList)
                    {
                        Console.Write(string.Format("[{0}] 변수이름: {1}", ++cnt, data.Name));
                        if (data.Value != null)
                            Console.WriteLine(string.Format(" ENQData 값: {1}", data.Value));
                    }
                    break;
                case XGTCnetCCType.ACK:
                    foreach (var dic in ResponseDic)
                    {
                        Console.Write(string.Format("[{0}] 변수이름: {1}", ++cnt, dic.Key));
                        Console.WriteLine(string.Format(" 값: {1}", dic.Value));
                    }
                    break;
                case XGTCnetCCType.NAK:
                    Console.WriteLine(string.Format("Error: " + Error.ToString()));
                    break;
            }
        }
    }
}
