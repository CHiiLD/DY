using System;
using System.Collections.Generic;
using System.Linq;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet - 프로토콜 프레임의 구조화된 데이터 처리 담당 클래스
    /// IProtocol <- AProtocol <- AXGTCnetProtocol <- XGTCnetProtocol
    /// </summary>
    public partial class XGTCnetProtocol : AXGTCnetProtocol
    {
        protected const string ERROR_ENQ_IS_NULL_OR_EMPTY = "Enqdatas have problem (null or empty data)";
        protected const string ERROR_READED_MEM_COUNT_LIMIT = "Enqdatas over limit of count (null or empty data)";
        protected const string ERROR_MONITER_INVALID_REGISTER_NUMBER = "Register_number have to register to 0 from 31";
        protected const int READED_MEM_MAX_COUNT = 16;

        #region CONSTRUCTOR
        /// <summary>
        /// 복사생성자
        /// </summary>
        /// <param name="that">복사 타겟</param>
        public XGTCnetProtocol(XGTCnetProtocol that)
            : base(that)
        {
        }

        /// <summary>
        /// 응답 프로토콜 생성자
        /// </summary>
        /// <param name="ascii"></param>
        private XGTCnetProtocol(byte[] ascii)
            : base(ascii)
        {
        }

        /// <summary>
        /// 요청 프로토콜 생성자
        /// </summary>
        /// <param name="localPort">국번</param>
        /// <param name="cmd">주명령어</param>
        /// <param name="type">명령어</param>
        private XGTCnetProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCmdType type)
            : base(localPort, cmd, type)
        {
        }

        #endregion

        #region FACTORY CONSTRUCT METHOD
        /// <summary>
        /// 정적 팩토리 메서드
        /// 직접 변수 개별 읽기 RSS
        /// </summary>
        /// <param name="type">메모리 타입</param>
        /// <param name="localPort">국번</param>
        /// <param name="tickets">변수이름과 메모리 주소가 담긴 Dictionary, 최대 16개까지 등록가능</param>
        /// <returns>RSS-XGTCnetProtocol 객체</returns>
        public static XGTCnetProtocol NewRSSProtocol(Type type, ushort localPort, Dictionary<string, object> tickets)
        {
            if (tickets.Count == 0 || tickets == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (tickets.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var instance = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCmdType.SS);
            instance.TType = type;
            instance.Tickets = new Dictionary<string, object>(tickets);
            instance.BlocCnt = (ushort)instance.Tickets.Count;
            return instance;
        }

        /// <summary>
        /// 정적 팩토리 메서드
        /// 직접 변수 개별 쓰기 WSS 프로토콜 생성
        /// </summary>
        /// <param name="type">메모리 타입</param>
        /// <param name="localPort">국번</param>
        /// <param name="tickets">변수이름과 메모리 주소가 담긴 Dictionary, 최대 16개까지 등록가능</param>
        /// <returns>WSS-XGTCnetProtocol 객체</returns>
        public static XGTCnetProtocol NewWSSProtocol(Type type, ushort localPort, Dictionary<string, object> tickets)
        {
            if (tickets.Count == 0 || tickets == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (tickets.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var instance = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCmdType.SS);
            instance.TType = type;
            instance.Tickets = new Dictionary<string, object>(tickets);
            instance.BlocCnt = (ushort)instance.Tickets.Count;
            return instance;
        }

        /// <summary>
        /// 정적 팩토리 메서드
        /// 직접 변수 연속 읽기 RSB
        /// </summary>
        /// <param name="type">메모리 타입</param>
        /// <param name="localPort">국번</param>
        /// <param name="var_name">변수이름</param>
        /// <param name="block_cnt">읽을 메모리 개수</param>
        /// <returns>RSB-XGTCnetProtocol 객체</returns>
        public static XGTCnetProtocol NewRSBProtocol(Type type, ushort localPort, string var_name, ushort block_cnt)
        {
            string name = var_name;
            if (type == typeof(Boolean)) //BIT 데이터는 연속 읽기를 할 수 없어요 ㅠ_ㅠ
                throw new ArgumentException("Rsb communication not supported bit data type");

            var instance = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCmdType.SB);
            instance.TType = type;
            instance.DataCnt = block_cnt;
            for (int i = 0; i < block_cnt; i++)
            {
                string str_header = name.Substring(0, 3);
                string str_num = name.Substring(3, name.Length - 3);
                int mem_num;
                if (Int32.TryParse(str_num, out mem_num))
                    instance.Tickets.Add(str_header + (mem_num + i).ToString(), type);
            }
            return instance;
        }

        /// <summary>
        /// 정적 팩토리 메서드
        /// 직접 변수 연속 쓰기 WSB
        /// </summary>
        /// <param name="type">메모리 타입</param>
        /// <param name="localPort">국번</param>
        /// <param name="tickets">변수이름과 메모리 주소가 담긴 구조체</param>
        /// <returns>WSB-XGTCnetProtocol 객체</returns>
        public static XGTCnetProtocol NewWSBProtocol(Type type, ushort localPort, Dictionary<string, object> tickets)
        {
            if (tickets.Count == 0 || tickets == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);

            int size_sum = 0;
            foreach (var d in tickets)
                size_sum += type.ToSize() * 2;
            if (size_sum > PROTOCOL_WSB_SIZE_MAX_240BYTE)
                throw new ArgumentException(ERROR_PROTOCOL_WSB_SIZE_MAX_240BYTE);

            var instance = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCmdType.SB);
            instance.TType = type;
            instance.Tickets = new Dictionary<string, object>(tickets);
            instance.DataCnt = (ushort)tickets.Count;
            return instance;
        }

        /// <summary>
        /// 정적 팩토리 메서드
        /// 응답 프로토콜 생성
        /// </summary>
        /// <param name="received_data">PLC에서 받은 ascii데이터</param>
        /// <param name="request">요청 프로토콜</param>
        /// <returns>응답 프로토콜</returns>
        public static XGTCnetProtocol CreateResponseProtocol(byte[] received_data, XGTCnetProtocol request)
        {
            XGTCnetProtocol instance = new XGTCnetProtocol(received_data);
            instance.Tickets = new Dictionary<string, object>(request.Tickets);
            instance.MirrorProtocol = request;
            instance.TType = request.TType;
            instance.Tag = request.Tag;
            instance.UserData = request.UserData;
            instance.Description = request.Description;
            return instance;
        }

        /// <summary>
        /// 정적 팩토리 메서드
        /// 요청 프로토콜 생성
        /// </summary>
        /// <param name="localPort">국번</param>
        /// <param name="cmd">명령어</param>
        /// <param name="type">주명령어</param>
        /// <returns></returns>
        private static XGTCnetProtocol CreateRequestProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCmdType type)
        {
            XGTCnetProtocol instance = new XGTCnetProtocol(localPort, cmd, type);
            instance.Header = XGTCnetCCType.ENQ;
            instance.Tail = XGTCnetCCType.EOT;
            return instance;
        }
        #endregion

        #region FOR REQUEST PROTOCOL TYPE

        private void AddRSSProtocol(List<byte> ascii)
        {
            ascii.AddRange(CA2C.Data2ASCII(Tickets.Count));// 블록 수
            foreach (var d in Tickets)
            {
                ascii.AddRange(CA2C.Data2ASCII(d.Key.Length));// 변수 이름 길이
                ascii.AddRange(CA2C.String2ASCII(d.Key));                       // 변수 이름 정보 
            }
        }

        private void AddWSSProtocol(List<byte> ascii)
        {
            ascii.AddRange(CA2C.Data2ASCII(Tickets.Count));               // 블록 수
            foreach (var d in Tickets)
            {
                ascii.AddRange(CA2C.Data2ASCII(d.Key.Length));// 변수 이름 길이
                ascii.AddRange(CA2C.String2ASCII(d.Key));                       // 변수 이름 정보 
                ascii.AddRange(CA2C.Value2ASCII(d.Value, TType));              // WRITE될 값 
            }
        }

        // not support bit data
        private void AddRSBProtocol(List<byte> ascii)
        {
            ascii.AddRange(CA2C.Data2ASCII(Tickets.First().Key.Length));
            ascii.AddRange(CA2C.String2ASCII(Tickets.First().Key));
            ascii.AddRange(CA2C.Data2ASCII(Tickets.Count));
        }

        private void AddWSBProtocol(List<byte> ascii)
        {
            ascii.AddRange(CA2C.Data2ASCII(Tickets.First().Key.Length));
            ascii.AddRange(CA2C.String2ASCII(Tickets.First().Key));
            ascii.AddRange(CA2C.Data2ASCII(DataCnt));
            foreach (var d in Tickets)
                ascii.AddRange(CA2C.Value2ASCII(d.Value, TType));
        }

        protected override void AttachProtocolFrame(List<byte> ascii)
        {
            switch (CommandType)
            {
                case XGTCnetCmdType.SS:
                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        AddRSSProtocol(ascii);
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        AddWSSProtocol(ascii);
                    break;
                case XGTCnetCmdType.SB:
                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        AddRSBProtocol(ascii);
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        AddWSBProtocol(ascii);
                    break;
            }
        }
        #endregion

        #region FOR RESPONSE PROTOCOL TYPE

        private void QueryRSSProtocol()
        {
            byte[] instruct_data = GetInstructData();
            BlocCnt = CA2C.ToUInt16Value(new byte[] { instruct_data[0], instruct_data[1] });
            int idx = 2;
            var dic = Tickets.ToList();
#if DEBUG
            if (BlocCnt != Tickets.Count())
                System.Diagnostics.Debug.Assert(false);
#endif
            for (int i = 0; i < BlocCnt; i++)
            {
                ushort size = CA2C.ToUInt16Value(new byte[] { instruct_data[idx + 0], instruct_data[idx + 1] });
                idx += 2;
                byte[] value = new byte[size * 2];
                Buffer.BlockCopy(instruct_data, idx, value, 0, value.Length);
                idx += value.Length;
                if (Tickets.ContainsKey(dic[i].Key))
                    Tickets[dic[i].Key] = CA2C.ToValue(value, TType);
#if DEBUG
                else
                    System.Diagnostics.Debug.Assert(false);
#endif
            }
        }

        private void QueryWSSProtocol()
        {
            return;
        }

        private void QueryRSBProtocol()
        {
            byte[] instruct_data = GetInstructData();
            ushort data_len = CA2C.ToUInt16Value(new byte[] { instruct_data[2], instruct_data[3] });// 데이터 개수 정보 쿼리
            int idx = 4;
            var dic = Tickets.ToList();
            int type_size = TType.ToSize();
#if DEBUG
            if (data_len / type_size != Tickets.Count())
                System.Diagnostics.Debug.Assert(false);
#endif
            for (int i = 0; i < data_len / type_size; i++)
            {
                byte[] value = new byte[type_size * 2];
                Buffer.BlockCopy(instruct_data, idx, value, 0, value.Length);
                if (Tickets.ContainsKey(dic[i].Key))
                    Tickets[dic[i].Key] = CA2C.ToValue(value, TType);
                idx += value.Length;
            }
        }

        private void QueryWSBProtocol()
        {
            return;
        }

        protected override void DetachProtocolFrame()
        {
            switch (CommandType)
            {
                case XGTCnetCmdType.SS:

                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        QueryRSSProtocol();
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        QueryWSSProtocol();
                    break;
                case XGTCnetCmdType.SB:
                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        QueryRSBProtocol();
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        QueryWSBProtocol();
                    break;
            }
        }
        #endregion

        /// <summary>
        /// 디버깅용 프로토콜 구조화된 데이터 정보 출력
        /// </summary>
        protected override void PrintInstructPart()
        {
            Console.WriteLine(string.Format("블록 수: {0}", BlocCnt));
            Console.WriteLine(string.Format("데이터 개수: {0}", DataCnt));
            int cnt = 0;
            switch (Header)
            {
                case XGTCnetCCType.ENQ:
                case XGTCnetCCType.ACK:
                    foreach (var d in Tickets)
                    {
                        Console.Write(string.Format("[{0}] name: {1}", ++cnt, d.Key));
                        Console.WriteLine(string.Format(" value: {0}", d.Value));
                    }
                    break;
                case XGTCnetCCType.NAK:
                    Console.WriteLine(string.Format("Error: " + Error.ToString()));
                    break;
            }
        }
    }
}