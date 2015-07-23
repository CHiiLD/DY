
using System;
using System.Collections.Generic;
using System.Linq;

namespace DY.NET.LSIS.XGT
{
    public partial class XGTFEnetProtocol : AProtocol
    {
        protected const string ERROR_ENQ_IS_NULL_OR_EMPTY = "Enqdatas have problem (null or empty data)";
        protected const string ERROR_READED_MEM_COUNT_LIMIT = "Enqdatas over limit of count (null or empty data)";
        private const string ERROR_PROTOCOL_SB_DATACNT_LIMIT = "Data count(asc bytes) limited 1400byte";
        public const int PROTOCOL_SB_MAX_DATA_CNT = 1400;
        private const int READED_MEM_MAX_COUNT = 16;
        private const int INSTRUCTION_BASIC_FORMAT_SIZE = 10;
        private const ushort ERROR_STATE_CHECK_0 = 0X00FF;
        private const ushort ERROR_STATE_CHECK_1 = 0XFFFF;

        public XGTFEnetHeader Header { get; private set; } //헤더 
        public ushort BlocCnt { get; private set; } //블록수 
        public XGTFEnetProtocolError Error { get; internal set; } //error code 1byte
        public XGTFEnetCommand Command { get; private set; } // read write, reqt resp
        public XGTFEnetDataType DataType { get; private set; } // data type 

        protected XGTFEnetProtocol()
            : base()
        {
            Error = XGTFEnetProtocolError.OK;
        }

        public XGTFEnetProtocol(XGTFEnetProtocol that)
            : base(that)
        {
            Header = that.Header;
            BlocCnt = that.BlocCnt;
            Error = that.Error;
            Command = that.Command;
            TargetType = that.TargetType;
        }

        /// <summary>
        /// 요청 프로토콜 정적 생성 팩토리 메서드
        /// </summary>
        /// <param name="header">해더 정보</param>
        /// <param name="cmd">XGTFEnetCommand 열거</param>
        /// <param name="block_cnt">읽을 변수의 개수</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        public static XGTFEnetProtocol CreateRequestProtocol(XGTFEnetHeader header, XGTFEnetCommand cmd, ushort block_cnt)
        {
            XGTFEnetProtocol instance = new XGTFEnetProtocol();
            instance.Command = cmd;
            instance.BlocCnt = block_cnt;
            instance.Header = header;
            return instance;
        }

        /// <summary>
        /// 응답 프로토콜 정적 생성 팩토리 메서드
        /// </summary>
        /// <param name="asc_data">바이트 배열 데이터</param>
        /// <param name="reqt">응답 프로토콜 객체</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        public static XGTFEnetProtocol CreateResponseProtocol(byte[] asc_data, XGTFEnetProtocol reqt)
        {
            XGTFEnetProtocol instance = new XGTFEnetProtocol();
            instance.DataStorageDictionary = new Dictionary<string, object>(reqt.DataStorageDictionary);
            instance.MirrorProtocol = reqt;
            instance.ProtocolData = asc_data;
            instance.Tag = reqt.Tag;
            instance.UserData = reqt.UserData;
            instance.Description = reqt.Description;
            instance.TargetType = reqt.TargetType;
            return instance;
        }

        public static XGTFEnetProtocol NewRSSProtocol(ushort tag, Type type, List<string> datas)
        {
            return NewRSSProtocol(XGTFEnetHeader.CreateXGTFEnetHeader(tag), type, datas);
        }

        public static XGTFEnetProtocol NewWSSProtocol(ushort tag, Type type, Dictionary<string, object> datas)
        {
            return NewWSSProtocol(XGTFEnetHeader.CreateXGTFEnetHeader(tag), type, datas);
        }

        public static XGTFEnetProtocol NewRSBProtocol(ushort tag, Type type, string name, ushort block_cnt)
        {
            return NewRSBProtocol(XGTFEnetHeader.CreateXGTFEnetHeader(tag), type, name, block_cnt);
        }

        public static XGTFEnetProtocol NewWSBProtocol(ushort tag, Type type, Dictionary<string, object> datas)
        {
            return NewWSBProtocol(XGTFEnetHeader.CreateXGTFEnetHeader(tag), type, datas);
        }

        /// <summary>
        /// 직접 변수 개별 읽기 RSS
        /// PLC에서 데이터 타입에 맞게 직접 변수이름을 지정하여 읽는 요청의 프로토콜 입니다
        /// 한번에 16개의 독립된 디바이스 메모리를 읽을 수가 있습니다
        /// <param name="header">XGTFEnetHeader 객체</param>
        /// <param name="pvalues">PValue 리스트</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewRSSProtocol(XGTFEnetHeader header, Type type, List<string> datas)
        {
            if (datas.Count == 0 || datas == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (datas.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var instance = CreateRequestProtocol(header, XGTFEnetCommand.READ_REQT, (ushort)datas.Count);
            instance.TargetType = type;
            foreach (var l in datas)
                instance.DataStorageDictionary.Add(l, null);
            instance.DataType = type.ToXGTFEnetDataType();
            return instance;
        }

        /// <summary>
        /// 직접 변수 개별 쓰기 WSS
        /// PLC 의 메모리 번지를 직접 지정하여 데이터 타입에 맞게 값을 쓰는 프로토콜입니다.
        /// <param name="header">XGTFEnetHeader 객체</param>
        /// <param name="pvalues">PValue 리스트</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewWSSProtocol(XGTFEnetHeader header, Type type, Dictionary<string, object> datas)
        {
            if (datas.Count == 0 || datas == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (datas.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var instance = CreateRequestProtocol(header, XGTFEnetCommand.WRITE_REQT, (ushort)datas.Count);
            instance.TargetType = type;
            instance.DataStorageDictionary = new Dictionary<string, object>(datas);
            instance.DataType = type.ToXGTFEnetDataType();
            return instance;
        }

        /// <summary>
        /// 직접 변수 연속 읽기 RSB
        /// PLC 에서 지정된 번지의 메모리에서 지정된 개수만큼 데이터를 일렬로 읽는 기능을 제공하는 프로토콜 입니다
        /// 연속 읽기에는 바이트 타입 직접변수만 가능합니다
        /// </summary>
        /// <param name="header">XGTFEnetHeader 객체</param>
        /// <param name="name">READ 시작점</param>
        /// <param name="block_cnt">읽을 개수 (최대 16개)</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewRSBProtocol(XGTFEnetHeader header, Type type, string name, ushort block_cnt)
        {
            string gname = name;
            if (!(type == typeof(Byte) || type == typeof(SByte))) //BYTE타입만 가능
                throw new ArgumentException("Rsb communication only supported byte data type");
            if (!PossibleRSBList.Contains(name[1]))
                throw new ArgumentException("this device type can not service");
            var instance = CreateRequestProtocol(header, XGTFEnetCommand.WRITE_REQT, 1);
            instance.TargetType = type;
            instance.DataType = XGTFEnetDataType.CONTINUATION;
            for (int i = 0; i < block_cnt; i++)
            {
                string str_header = gname.Substring(0, 3);
                string str_num = gname.Substring(3, gname.Length - 3);
                int mem_num;
                if (Int32.TryParse(str_num, out mem_num))
                    instance.DataStorageDictionary.Add(str_header + (mem_num + i).ToString(), null);
            }
            return instance;
        }

        /// <summary>
        /// 직접 변수 연속 쓰기 WSB
        /// PLC 의 메모리에서 지정된 번지로부터 지정된 길이만큼 데이터를 일렬로 쓰는 기능의 프로토콜 입니다
        /// <param name="header">XGTFEnetHeader 객체</param>
        /// <param name="pvalues">PValue 리스트</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewWSBProtocol(XGTFEnetHeader header, Type type, Dictionary<string, object> datas)
        {
            if (datas.Count == 0 || datas == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (!(type == typeof(Byte) || type == typeof(SByte))) //BYTE타입만 가능
                throw new ArgumentException("Rsb communication only supported byte data type");
            if (!PossibleRSBList.Contains(datas.First().Key[1]))
                throw new ArgumentException("this device type can not service");
            var instance = CreateRequestProtocol(header, XGTFEnetCommand.WRITE_REQT, 1);
            instance.TargetType = type;
            instance.DataStorageDictionary = new Dictionary<string, object>(datas);
            instance.DataType = XGTFEnetDataType.CONTINUATION;
            return instance;
        }

        /// <summary>
        /// 맴머 변수의 정보를 토대로 원시 프로토콜 데이터를 계산합니다.
        /// </summary>
        public override void AssembleProtocol()
        {
            List<byte> asc_data = new List<byte>();
            asc_data.AddRange(Command.ToBytes().Reverse());     //명령어
            asc_data.AddRange(DataType.ToBytes().Reverse());    //데이터 타입
            asc_data.AddRange(new byte[] { 0, 0 }); //예약
            asc_data.AddRange(CV2BR.ToBytes(BlocCnt, typeof(ushort))); //블록수
            //RSS
            if (Command == XGTFEnetCommand.READ_REQT && DataType != XGTFEnetDataType.CONTINUATION)
            {
                foreach (var d in DataStorageDictionary)
                {
                    asc_data.AddRange(CV2BR.ToBytes(d.Key.Length, typeof(UInt16))); //변수 길이
                    asc_data.AddRange(CV2BR.ToBytes(d.Key)); //변수 이름
                }
            }
            //RSB
            else if (Command == XGTFEnetCommand.READ_REQT && DataType == XGTFEnetDataType.CONTINUATION)
            {
                asc_data.AddRange(CV2BR.ToBytes(DataStorageDictionary.First().Key.Length, typeof(UInt16))); //변수 길이
                asc_data.AddRange(CV2BR.ToBytes(DataStorageDictionary.First().Key)); //변수 이름
                int byte_size = DataStorageDictionary.Count() * TargetType.ToSize();
                if (byte_size > PROTOCOL_SB_MAX_DATA_CNT)
                    throw new Exception(ERROR_PROTOCOL_SB_DATACNT_LIMIT);
                asc_data.AddRange(CV2BR.ToBytes(byte_size, typeof(UInt16))); //읽을 개수
            }
            //WSS
            else if (Command == XGTFEnetCommand.WRITE_REQT && DataType != XGTFEnetDataType.CONTINUATION)
            {
                foreach (var d in DataStorageDictionary)
                {
                    asc_data.AddRange(CV2BR.ToBytes(d.Key.Length, typeof(UInt16))); //변수 길이
                    asc_data.AddRange(CV2BR.ToBytes(d.Key)); //변수 이름
                    asc_data.AddRange(CV2BR.ToBytes(TargetType.ToSize(), typeof(UInt16))); //변수 크기
                     asc_data.AddRange(CV2BR.ToBytes(d.Value)); //변수 값
                }
            }
            //WSB
            else if (Command == XGTFEnetCommand.READ_REQT && DataType == XGTFEnetDataType.CONTINUATION)
            {
                asc_data.AddRange(CV2BR.ToBytes(DataStorageDictionary.First().Key.Length, typeof(UInt16))); //변수 길이
                asc_data.AddRange(CV2BR.ToBytes(DataStorageDictionary.First().Key)); //변수 이름
                asc_data.AddRange(CV2BR.ToBytes(DataStorageDictionary.Count(), typeof(UInt16))); //쓸 데이터 개수
                int sum = 0;
                foreach (var d in DataStorageDictionary)
                {
                    var bytes = CV2BR.ToBytes(d.Value);
                    sum += bytes.Length;
                    asc_data.AddRange(bytes);
                }
                if (sum > PROTOCOL_SB_MAX_DATA_CNT)
                    throw new Exception(ERROR_PROTOCOL_SB_DATACNT_LIMIT);
            }
            var header_byte_data = Header.GetBytes(asc_data.Count());
            ProtocolData = new byte[header_byte_data.Length + asc_data.Count()];
            Buffer.BlockCopy(header_byte_data, 0, ProtocolData, 0, header_byte_data.Length);
            Buffer.BlockCopy(asc_data.ToArray(), 0, ProtocolData, header_byte_data.Length, asc_data.Count);
        }

        /// <summary>
        /// 받은 원시 프로토콜 데이터를 바탕으로 프로토콜 구조와 데이터를 파악합니다.
        /// </summary>
        public override void AnalysisProtocol()
        {
            Header = XGTFEnetHeader.CreateXGTFEnetHeader(ProtocolData);
            Command = (XGTFEnetCommand)CV2BR.ToValue(new byte[] { ProtocolData[20], ProtocolData[21] }, typeof(UInt16));
            DataType = (XGTFEnetDataType)CV2BR.ToValue(new byte[] { ProtocolData[22], ProtocolData[23] }, typeof(UInt16));
            ushort err_state = (ushort)CV2BR.ToValue(new byte[] { ProtocolData[26], ProtocolData[27] }, typeof(UInt16)); //에러 상태
            ushort bloc_errcode = (ushort)CV2BR.ToValue(new byte[] { ProtocolData[28], ProtocolData[29] }, typeof(UInt16)); //에러코드 or 블록 수 
            if (err_state == ERROR_STATE_CHECK_0 || err_state == ERROR_STATE_CHECK_1)
            {
                Error = (XGTFEnetProtocolError)bloc_errcode;
                return;
            }
            BlocCnt = bloc_errcode;
            byte[] data = new byte[ProtocolData.Length - XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE - INSTRUCTION_BASIC_FORMAT_SIZE];
            int data_idx = 0;
            Buffer.BlockCopy(ProtocolData, XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE + INSTRUCTION_BASIC_FORMAT_SIZE, data, 0, data.Length);
            //RSS
            var list = DataStorageDictionary.ToList();
            if (Command == XGTFEnetCommand.READ_RESP && DataType != XGTFEnetDataType.CONTINUATION)
            {
                for (int i = 0; i < BlocCnt; i++)
                {
                    ushort sizeOfType = (ushort)CV2BR.ToValue(new byte[] { data[data_idx + 0], data[data_idx + 1] }, typeof(ushort));
                    data_idx += 2;
                    byte[] data_arr = new byte[sizeOfType];
                    Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                    DataStorageDictionary[list[i].Key] = CV2BR.ToValue(data_arr, TargetType);
                    data_idx += data_arr.Length;
                }
            }
            //RSB
            else if (Command == XGTFEnetCommand.READ_RESP && DataType == XGTFEnetDataType.CONTINUATION)
            {
                int data_type_size = TargetType.ToSize();
                for (int i = 0; i < BlocCnt / data_type_size; i++)
                {
                    byte[] data_arr = new byte[data_type_size];
                    Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                    DataStorageDictionary[list[i].Key] = CV2BR.ToValue(data_arr, TargetType);
                    data_idx += data_arr.Length;
                }
            }
        }

        public override void Print()
        {
            Console.WriteLine("XGT FEnet 프로토콜 정보");
            Console.WriteLine("ASC 코드: " + ByteArray2HexStr.Change(ProtocolData));
            Console.WriteLine("명령: " + Command.ToString());
            Console.WriteLine("데이터 타입: " + DataType.ToString());

            Console.WriteLine(string.Format("블록 수: {0}", BlocCnt));
            if (Error != XGTFEnetProtocolError.OK)
            {
                Console.WriteLine(string.Format("Error: " + Error.ToString()));
                return;
            }
            int i = 0;
            if (Command == XGTFEnetCommand.READ_REQT || Command == XGTFEnetCommand.READ_RESP)
            {
                foreach (var d in DataStorageDictionary)
                {
                    Console.Write(string.Format("[{0}] name: {1}", ++i, d.Key));
                    Console.WriteLine(string.Format(" value: {0}", d.Value));
                }
            }
        }
    }
}