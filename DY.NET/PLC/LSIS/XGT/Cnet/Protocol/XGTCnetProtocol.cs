/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜 RWSB RWSS XYSS XYSB 에 사용할 수 있는 범용 클래스
 * 날짜: 2015-03-31
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet 통신을 위한 프로토콜 클래스
    /// </summary>
    public partial class XGTCnetProtocol : AXGTCnetProtocol
    {
        protected const string ERROR_ENQ_IS_NULL_OR_EMPTY = "Enqdatas have problem (null or empty data)";
        protected const string ERROR_READED_MEM_COUNT_LIMIT = "Enqdatas over limit of count (null or empty data)";
        protected const string ERROR_MONITER_INVALID_REGISTER_NUMBER = "Register_number have to register to 0 from 31";
        protected const int READED_MEM_MAX_COUNT = 16;

        public ushort BlocCnt { private set; get; }   //2byte
        public ushort DataCnt { private set; get; }   //읽거나 쓸 데이터의 개수 (BYTE = 데이터 타입 * 개수) 최대 240byte word는 120byte 가 한계 //2byte
        public ushort RegiNum { private set; get; }   //등록 번호    2byte

        #region CONSTRUCTOR
        /// <summary>
        /// XGTCnetExclusiveProtocol 복사생성자
        /// </summary>
        /// <param name="that"> 복사하고자 할 XGTCnetExclusiveProtocol 객체 </param>
        public XGTCnetProtocol(XGTCnetProtocol that)
            : base(that)
        {
            BlocCnt = that.BlocCnt;
            DataCnt = that.DataCnt;
            RegiNum = that.RegiNum;
            TargetType = that.TargetType;
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

        #region FACTORY CONSTRUCT METHOD
        /// <summary>
        /// 직접 변수 개별 읽기 RSS
        /// PLC에서 데이터 타입에 맞게 직접 변수이름을 지정하여 읽는 요청의 프로토콜 입니다
        /// 한번에 16개의 독립된 디바이스 메모리를 읽을 수가 있습니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="datas"> 변수이름과 메모리 주소가 담긴 구조체의 리스트 최대 16개까지 사용 가능합니다 </param>
        /// <returns> RSS 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetProtocol NewRSSProtocol(Type type, ushort localPort, List<string> datas)
        {
            if (datas.Count == 0 || datas == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (datas.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var instance = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCmdType.SS);
            instance.TargetType = type;
            foreach (var l in datas)
                instance.DataStorageDictionary.Add(l, null);
            instance.BlocCnt = (ushort)instance.DataStorageDictionary.Count;
            return instance;
        }

        /// <summary>
        /// 직접 변수 개별 쓰기 WSS
        /// PLC 의 메모리 번지를 직접 지정하여 데이터 타입에 맞게 값을 쓰는 프로토콜입니다.
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="datas"> 변수이름과 메모리 주소가 담긴 구조체의 리스트 최대 16개까지 사용 가능합니다 </param>
        /// <returns> WSS 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetProtocol NewWSSProtocol(Type type, ushort localPort, Dictionary<string, object> datas)
        {
            if (datas.Count == 0 || datas == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (datas.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var instance = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCmdType.SS);
            instance.TargetType = type;
            instance.DataStorageDictionary = new Dictionary<string, object>(datas);
            instance.BlocCnt = (ushort)instance.DataStorageDictionary.Count;
            return instance;
        }

        /// <summary>
        /// 직접 변수 연속 읽기 RSB
        /// PLC 에서 지정된 번지의 메모리에서 지정된 개수만큼 데이터를 일렬로 읽는 기능을 제공하는 프로토콜 입니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="glopa_name"> 변수이름 </param>
        /// <param name="block_cnt"> 읽을 메모리 번지의 개수 </param>
        /// <returns> RSB 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetProtocol NewRSBProtocol(Type type, ushort localPort, string var_name, ushort block_cnt)
        {
            string name = var_name;
            if (type == typeof(Boolean)) //BIT 데이터는 연속 읽기를 할 수 없어요 ㅠ_ㅠ
                throw new ArgumentException("Rsb communication not supported bit data type");

            var instance = CreateRequestProtocol(localPort, XGTCnetCommand.R, XGTCnetCmdType.SB);
            instance.TargetType = type;
            instance.DataCnt = block_cnt;
            for (int i = 0; i < block_cnt; i++)
            {
                string str_header = name.Substring(0, 3);
                string str_num = name.Substring(3, name.Length - 3);
                int mem_num;
                if (Int32.TryParse(str_num, out mem_num))
                    instance.DataStorageDictionary.Add(str_header + (mem_num + i).ToString(), type);
            }
            return instance;
        }

        /// <summary>
        /// 직접 변수 연속 쓰기 WSB
        /// PLC 의 메모리에서 지정된 번지로부터 지정된 길이만큼 데이터를 일렬로 쓰는 기능의 프로토콜 입니다
        /// </summary>
        /// <param name="localPort"> 국번 </param>
        /// <param name="datas"> 변수이름과 메모리 주소가 담긴 구조체 </param>
        /// <returns> WSB 모드의 XGTCnetExclusiveProtocol 프로토콜 </returns>
        public static XGTCnetProtocol NewWSBProtocol(Type type, ushort localPort, Dictionary<string, object> datas)
        {
            if (datas.Count == 0 || datas == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);

            int size_sum = 0;
            foreach (var d in datas)
                size_sum += type.ToSize() * 2;
            if (size_sum > PROTOCOL_WSB_SIZE_MAX_240BYTE)
                throw new ArgumentException(ERROR_PROTOCOL_WSB_SIZE_MAX_240BYTE);

            var instance = CreateRequestProtocol(localPort, XGTCnetCommand.W, XGTCnetCmdType.SB);
            instance.TargetType = type;
            instance.DataStorageDictionary = new Dictionary<string, object>(datas);
            instance.DataCnt = (ushort)datas.Count;
            return instance;
        }

        /// <summary>
        /// PLC에서 받은 원시 데이터를 분석하여 XGTCnetExclusiveProtocol 클래스로 변환&데이터분석하여 응답 프로토콜 클래스 리턴.
        /// </summary>
        /// <param name="binaryData"> 원시데이터 </param>
        /// <param name="reqtProtocol"> 요청 프로토콜 클래스 </param>
        /// <returns> 응답 프로토콜 클래스 </returns>
        public static XGTCnetProtocol CreateResponseProtocol(byte[] received_data, XGTCnetProtocol reqt)
        {
            XGTCnetProtocol instance = new XGTCnetProtocol(received_data);
            instance.DataStorageDictionary = new Dictionary<string, object>(reqt.DataStorageDictionary);
            instance.MirrorProtocol = reqt;
            instance.TargetType = reqt.TargetType;
            instance.Tag = reqt.Tag;
            instance.UserData = reqt.UserData;
            instance.Description = reqt.Description;
            return instance;
        }

        private static XGTCnetProtocol CreateRequestProtocol(ushort localPort, XGTCnetCommand cmd, XGTCnetCmdType type)
        {
            XGTCnetProtocol instance = new XGTCnetProtocol(localPort, cmd, type);
            instance.Header = XGTCnetCCType.ENQ;
            instance.Tail = XGTCnetCCType.EOT;
            return instance;
        }
        #endregion

        #region FOR REQUEST PROTOCOL TYPE

        private void AddRSSProtocol(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(DataStorageDictionary.Count, typeof(UInt16)));// 블록 수
            foreach (var d in DataStorageDictionary)
            {
                asc_list.AddRange(CA2C.ToASC(d.Key.Length, typeof(UInt16)));// 변수 이름 길이
                asc_list.AddRange(CA2C.ToASC(d.Key));                       // 변수 이름 정보 
            }
        }

        private void AddWSSProtocol(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(DataStorageDictionary.Count, typeof(UInt16)));               // 블록 수
            foreach (var d in DataStorageDictionary)
            {
                asc_list.AddRange(CA2C.ToASC(d.Key.Length, typeof(UInt16)));// 변수 이름 길이
                asc_list.AddRange(CA2C.ToASC(d.Key));                       // 변수 이름 정보 
                asc_list.AddRange(CA2C.ToASC(d.Value));              // WRITE될 값 
            }
        }

        // not support bit data
        private void AddRSBProtocol(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(DataStorageDictionary.First().Key.Length, typeof(UInt16)));
            asc_list.AddRange(CA2C.ToASC(DataStorageDictionary.First().Key));
            asc_list.AddRange(CA2C.ToASC(DataStorageDictionary));
        }

        private void AddWSBProtocol(List<byte> asc_list)
        {
            asc_list.AddRange(CA2C.ToASC(DataStorageDictionary.First().Key.Length, typeof(UInt16)));
            asc_list.AddRange(CA2C.ToASC(DataStorageDictionary.First().Key));
            asc_list.AddRange(CA2C.ToASC(DataCnt));
            foreach (var d in DataStorageDictionary)
                asc_list.AddRange(CA2C.ToASC(d.Value));
        }

        protected override void AttachProtocolFrame(List<byte> asc_list)
        {
            switch (CommandType)
            {
                case XGTCnetCmdType.SS:
                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        AddRSSProtocol(asc_list);
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        AddWSSProtocol(asc_list);
                    else if (Command == XGTCnetCommand.X || Command == XGTCnetCommand.x)
                        throw new NotImplementedException();
                    else if (Command == XGTCnetCommand.Y || Command == XGTCnetCommand.y)
                        throw new NotImplementedException();
                    break;
                case XGTCnetCmdType.SB:
                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        AddRSBProtocol(asc_list);
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        AddWSBProtocol(asc_list);
                    else if (Command == XGTCnetCommand.X || Command == XGTCnetCommand.x)
                        throw new NotImplementedException();
                    else if (Command == XGTCnetCommand.Y || Command == XGTCnetCommand.y)
                        throw new NotImplementedException();
                    break;
            }
        }
        #endregion

        #region FOR RESPONSE PROTOCOL TYPE

        private void QueryRSSProtocol()
        {
            byte[] data = GetInstructData();
            BlocCnt = (ushort)CA2C.ToValue(new byte[] { data[0], data[1] }, typeof(UInt16));
            int data_idx = 2;
            var list = DataStorageDictionary.ToList();
            for (int i = 0; i < BlocCnt; i++)
            {
                ushort sizeOfType = (ushort)CA2C.ToValue(new byte[] { data[data_idx + 0], data[data_idx + 1] }, typeof(UInt16));
                data_idx += 2;
                byte[] data_arr = new byte[sizeOfType * 2];
                Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                data_idx += data_arr.Length;
                DataStorageDictionary[list[i].Key] = CA2C.ToValue(data_arr, TargetType);
            }
        }

        private void QueryWSSProtocol()
        {
            return;
        }

        private void QueryRSBProtocol()
        {
            byte[] data = GetInstructData();
            ushort data_len = (ushort)CA2C.ToValue(new byte[] { data[2], data[3] }, typeof(UInt16));// 데이터 개수 정보 쿼리
            int data_idx = 4;
            var list = DataStorageDictionary.ToList();
            int data_type_size = TargetType.ToSize();
            for (int i = 0; i < data_len / data_type_size; i++)
            {
                byte[] data_arr = new byte[data_type_size * 2];
                Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                DataStorageDictionary[list[i].Key] = CA2C.ToValue(data_arr, TargetType);
                data_idx += data_arr.Length;
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
                    else if (Command == XGTCnetCommand.X || Command == XGTCnetCommand.x)
                        throw new NotImplementedException();
                    else if (Command == XGTCnetCommand.Y || Command == XGTCnetCommand.y)
                        throw new NotImplementedException();
                    break;
                case XGTCnetCmdType.SB:
                    if (Command == XGTCnetCommand.R || Command == XGTCnetCommand.r)
                        QueryRSBProtocol();
                    else if (Command == XGTCnetCommand.W || Command == XGTCnetCommand.w)
                        QueryWSBProtocol();
                    else if (Command == XGTCnetCommand.X || Command == XGTCnetCommand.x)
                        throw new NotImplementedException();
                    else if (Command == XGTCnetCommand.Y || Command == XGTCnetCommand.y)
                        throw new NotImplementedException();
                    break;
            }
        }
        #endregion

        protected override void PrintInstruct()
        {
            Console.WriteLine(string.Format("블록 수: {0}", BlocCnt));
            Console.WriteLine(string.Format("등록 번호: {0}", RegiNum));
            Console.WriteLine(string.Format("데이터 개수: {0}", DataCnt));
            int cnt = 0;
            switch (Header)
            {
                case XGTCnetCCType.ENQ:
                case XGTCnetCCType.ACK:
                    foreach (var d in DataStorageDictionary)
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