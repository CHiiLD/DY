
using System;
using System.Collections.Generic;
using System.Linq;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet - 프로토콜 클래스
    /// IProtocol <- AProtocol <- XGTFEnetProtocol
    /// </summary>
    public partial class XGTFEnetProtocol : AProtocol
    {
        private const string ERROR_ENQ_IS_NULL_OR_EMPTY = "Enqdatas have problem (null or empty data)";
        private const string ERROR_READED_MEM_COUNT_LIMIT = "Enqdatas over limit of count (null or empty data)";
        private const string ERROR_PROTOCOL_SB_DATACNT_LIMIT = "Data count(asc bytes) limited 1400byte";
        private const ushort ERROR_STATE_CHECK_0 = 0X00FF;
        private const ushort ERROR_STATE_CHECK_1 = 0XFFFF;

        public const int PROTOCOL_SB_MAX_DATA_CNT = 1400;
        public const int READED_MEM_MAX_COUNT = 16;
        public const int INSTRUCTION_BASIC_FORMAT_SIZE = 10;

        public XGTFEnetHeader Header { get; private set; } //헤더 
        public ushort BlocCnt { get; private set; } //블록수 
        public XGTFEnetProtocolError Error { get; internal set; } //에러코드 2byte
        public XGTFEnetCommand Command { get; private set; } // 명령어
        public XGTFEnetDataType DataType { get; private set; } // 데이터 타입

        /// <summary>
        /// 생성자
        /// </summary>
        private XGTFEnetProtocol()
            : base()
        {
            Error = XGTFEnetProtocolError.OK;
        }

        /// <summary>
        /// 복사생성자
        /// </summary>
        /// <param name="that">복사 타겟</param>
        public XGTFEnetProtocol(XGTFEnetProtocol that)
            : base(that)
        {
            Header = new XGTFEnetHeader(that.Header);
            BlocCnt = that.BlocCnt;
            Error = that.Error;
            Command = that.Command;
            DataType = that.DataType;
        }

        /// <summary>
        /// 정적 생성 팩토리 메서드
        /// 요청 프로토콜 
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
        /// 정적 생성 팩토리 메서드
        /// 응답 프로토콜 
        /// </summary>
        /// <param name="asc_data">바이트 배열 데이터</param>
        /// <param name="reqt">응답 프로토콜 객체</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        public static XGTFEnetProtocol CreateResponseProtocol(byte[] asc_data, XGTFEnetProtocol reqt)
        {
            XGTFEnetProtocol instance = new XGTFEnetProtocol();
            instance.StorageDictionary = new Dictionary<string, object>(reqt.StorageDictionary);
            instance.MirrorProtocol = reqt;
            instance.ASCIIData = asc_data;
            instance.Tag = reqt.Tag;
            instance.UserData = reqt.UserData;
            instance.Description = reqt.Description;
            instance.TType = reqt.TType;
            return instance;
        }

        public static XGTFEnetProtocol NewRSSProtocol(Type type, ushort tag, Dictionary<string, object> datas)
        {
            return NewRSSProtocol(type, XGTFEnetHeader.CreateXGTFEnetHeader(tag), datas);
        }

        public static XGTFEnetProtocol NewWSSProtocol(Type type, ushort tag, Dictionary<string, object> datas)
        {
            return NewWSSProtocol(type, XGTFEnetHeader.CreateXGTFEnetHeader(tag), datas);
        }

        public static XGTFEnetProtocol NewRSBProtocol(Type type, ushort tag, string name, ushort block_cnt)
        {
            return NewRSBProtocol(type, XGTFEnetHeader.CreateXGTFEnetHeader(tag), name, block_cnt);
        }

        public static XGTFEnetProtocol NewWSBProtocol(Type type, ushort tag, Dictionary<string, object> datas)
        {
            return NewWSBProtocol(type, XGTFEnetHeader.CreateXGTFEnetHeader(tag), datas);
        }

        /// <summary>
        /// 정적 생성 팩토리 메서드
        /// 직접 변수 개별 읽기 RSS
        /// </summary>
        /// <param name="type">메모리 타입</param>
        /// <param name="header">XGTFEnetHeader</param>
        /// <param name="storage">변수이름과 메모리 주소가 담긴 Dictionary, 최대 16개까지 등록가능</param>
        /// <returns>RSS-XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewRSSProtocol(Type type, XGTFEnetHeader header, Dictionary<string, object> storage)
        {
            if (storage.Count == 0 || storage == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (storage.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var instance = CreateRequestProtocol(header, XGTFEnetCommand.READ_REQT, (ushort)storage.Count);
            instance.TType = type;
            instance.StorageDictionary = new Dictionary<string, object>(storage);
            instance.DataType = type.ToXGTFEnetDataType();
            return instance;
        }

        /// <summary>
        /// 정적 생성 팩토리 메서드
        /// 직접 변수 개별 쓰기 WSS
        /// <param name="type">메모리 타입</param>
        /// <param name="header">XGTFEnetHeader</param>
        /// <param name="storage">변수이름과 메모리 주소가 담긴 Dictionary, 최대 16개까지 등록가능</param>
        /// <returns>WSS-XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewWSSProtocol(Type type, XGTFEnetHeader header, Dictionary<string, object> storage)
        {
            if (storage.Count == 0 || storage == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (storage.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var instance = CreateRequestProtocol(header, XGTFEnetCommand.WRITE_REQT, (ushort)storage.Count);
            instance.TType = type;
            instance.StorageDictionary = new Dictionary<string, object>(storage);
            instance.DataType = type.ToXGTFEnetDataType();
            return instance;
        }

        /// <summary>
        /// 정적 생성 팩토리 메서드
        /// 직접 변수 연속 읽기 RSB
        /// </summary>
        /// <param name="type">메모리 타입</param>
        /// <param name="header">XGTFEnetHeader</param>
        /// <param name="name">읽어들일 시작 메모리 주소</param>
        /// <param name="block_cnt">읽을 개수(최대 16개)</param>
        /// <returns>RSB-XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewRSBProtocol(Type type, XGTFEnetHeader header, string name, ushort block_cnt)
        {
            string gname = name;
            if (!(type == typeof(Byte) || type == typeof(SByte))) //BYTE타입만 가능
                throw new ArgumentException("Rsb communication only supported byte data type");
            if (!UsableRSBDeviceNameList.Contains(name[1]))
                throw new ArgumentException("this device type can not service");
            var instance = CreateRequestProtocol(header, XGTFEnetCommand.WRITE_REQT, 1);
            instance.TType = type;
            instance.DataType = XGTFEnetDataType.CONTINUATION;
            for (int i = 0; i < block_cnt; i++)
            {
                string str_header = gname.Substring(0, 3);
                string str_num = gname.Substring(3, gname.Length - 3);
                int mem_num;
                if (Int32.TryParse(str_num, out mem_num))
                    instance.StorageDictionary.Add(str_header + (mem_num + i).ToString(), null);
            }
            return instance;
        }

        /// <summary>
        /// 정적 생성 팩토리 메서드
        /// 직접 변수 연속 쓰기 WSB
        /// <param name="type">메모리 타입</param>
        /// <param name="header">XGTFEnetHeader 객체</param>
        /// <param name="storage">변수이름과 메모리 주소가 담긴 Dictionary, 최대 16개까지 등록가능</param>
        /// <returns>RSB-XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewWSBProtocol(Type type, XGTFEnetHeader header, Dictionary<string, object> storage)
        {
            if (storage.Count == 0 || storage == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (!(type == typeof(Byte) || type == typeof(SByte))) //BYTE타입만 가능
                throw new ArgumentException("Rsb communication only supported byte data type");
            if (!UsableRSBDeviceNameList.Contains(storage.First().Key[1]))
                throw new ArgumentException("this device type can not service");
            var instance = CreateRequestProtocol(header, XGTFEnetCommand.WRITE_REQT, 1);
            instance.TType = type;
            instance.StorageDictionary = new Dictionary<string, object>(storage);
            instance.DataType = XGTFEnetDataType.CONTINUATION;
            return instance;
        }

        /// <summary>
        /// 요청 프로토콜 - 프로토콜 프레임 정보로 스트림 버퍼에 쓸 ASCII 데이터를 구축한다.
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
                foreach (var d in StorageDictionary)
                {
                    asc_data.AddRange(CV2BR.ToBytes(d.Key.Length, typeof(UInt16))); //변수 길이
                    asc_data.AddRange(CV2BR.ToBytes(d.Key)); //변수 이름
                }
            }
            //RSB
            else if (Command == XGTFEnetCommand.READ_REQT && DataType == XGTFEnetDataType.CONTINUATION)
            {
                asc_data.AddRange(CV2BR.ToBytes(StorageDictionary.First().Key.Length, typeof(UInt16))); //변수 길이
                asc_data.AddRange(CV2BR.ToBytes(StorageDictionary.First().Key)); //변수 이름
                int byte_size = StorageDictionary.Count() * TType.ToSize();
                if (byte_size > PROTOCOL_SB_MAX_DATA_CNT)
                    throw new Exception(ERROR_PROTOCOL_SB_DATACNT_LIMIT);
                asc_data.AddRange(CV2BR.ToBytes(byte_size, typeof(UInt16))); //읽을 개수
            }
            //WSS
            else if (Command == XGTFEnetCommand.WRITE_REQT && DataType != XGTFEnetDataType.CONTINUATION)
            {
                foreach (var d in StorageDictionary)
                {
                    asc_data.AddRange(CV2BR.ToBytes(d.Key.Length, typeof(UInt16))); //변수 길이
                    asc_data.AddRange(CV2BR.ToBytes(d.Key)); //변수 이름
                    asc_data.AddRange(CV2BR.ToBytes(TType.ToSize(), typeof(UInt16))); //변수 크기
                    asc_data.AddRange(CV2BR.ToBytes(d.Value)); //변수 값
                }
            }
            //WSB
            else if (Command == XGTFEnetCommand.READ_REQT && DataType == XGTFEnetDataType.CONTINUATION)
            {
                asc_data.AddRange(CV2BR.ToBytes(StorageDictionary.First().Key.Length, typeof(UInt16))); //변수 길이
                asc_data.AddRange(CV2BR.ToBytes(StorageDictionary.First().Key)); //변수 이름
                asc_data.AddRange(CV2BR.ToBytes(StorageDictionary.Count(), typeof(UInt16))); //쓸 데이터 개수
                int sum = 0;
                foreach (var d in StorageDictionary)
                {
                    var bytes = CV2BR.ToBytes(d.Value);
                    sum += bytes.Length;
                    asc_data.AddRange(bytes);
                }
                if (sum > PROTOCOL_SB_MAX_DATA_CNT)
                    throw new Exception(ERROR_PROTOCOL_SB_DATACNT_LIMIT);
            }
            var header_byte_data = Header.GetBytes(asc_data.Count());
            ASCIIData = new byte[header_byte_data.Length + asc_data.Count()];
            Buffer.BlockCopy(header_byte_data, 0, ASCIIData, 0, header_byte_data.Length);
            Buffer.BlockCopy(asc_data.ToArray(), 0, ASCIIData, header_byte_data.Length, asc_data.Count);
        }

        /// <summary>
        /// 응답 프로토콜 - ASCII 데이터를 분석하여 프로토콜 프레임 정보를 파악한다.
        /// </summary>
        public override void AnalysisProtocol()
        {
            Header = XGTFEnetHeader.CreateXGTFEnetHeader(ASCIIData);
            Command = (XGTFEnetCommand)CV2BR.ToValue(new byte[] { ASCIIData[20], ASCIIData[21] }, typeof(UInt16));
            DataType = (XGTFEnetDataType)CV2BR.ToValue(new byte[] { ASCIIData[22], ASCIIData[23] }, typeof(UInt16));
            ushort err_state = (ushort)CV2BR.ToValue(new byte[] { ASCIIData[26], ASCIIData[27] }, typeof(UInt16)); //에러 상태
            ushort bloc_errcode = (ushort)CV2BR.ToValue(new byte[] { ASCIIData[28], ASCIIData[29] }, typeof(UInt16)); //에러코드 or 블록 수 
            if (err_state == ERROR_STATE_CHECK_0 || err_state == ERROR_STATE_CHECK_1)
            {
                Error = (XGTFEnetProtocolError)bloc_errcode;
                return;
            }
            BlocCnt = bloc_errcode;
            byte[] data = new byte[ASCIIData.Length - XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE - INSTRUCTION_BASIC_FORMAT_SIZE];
            int data_idx = 0;
            Buffer.BlockCopy(ASCIIData, XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE + INSTRUCTION_BASIC_FORMAT_SIZE, data, 0, data.Length);
            //RSS
            var list = StorageDictionary.ToList();
            if (Command == XGTFEnetCommand.READ_RESP && DataType != XGTFEnetDataType.CONTINUATION)
            {
                for (int i = 0; i < BlocCnt; i++)
                {
                    ushort sizeOfType = (ushort)CV2BR.ToValue(new byte[] { data[data_idx + 0], data[data_idx + 1] }, typeof(ushort));
                    data_idx += 2;
                    byte[] data_arr = new byte[sizeOfType];
                    Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                    StorageDictionary[list[i].Key] = CV2BR.ToValue(data_arr, TType);
                    data_idx += data_arr.Length;
                }
            }
            //RSB
            else if (Command == XGTFEnetCommand.READ_RESP && DataType == XGTFEnetDataType.CONTINUATION)
            {
                int data_type_size = TType.ToSize();
                for (int i = 0; i < BlocCnt / data_type_size; i++)
                {
                    byte[] data_arr = new byte[data_type_size];
                    Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                    StorageDictionary[list[i].Key] = CV2BR.ToValue(data_arr, TType);
                    data_idx += data_arr.Length;
                }
            }
        }

        /// <summary>
        /// 디버깅용 프로토콜 프레임 정보 출력
        /// </summary>
        public override void Print()
        {
            Console.WriteLine("XGT FEnet 프로토콜 정보");
            Console.WriteLine("ASC 코드: " + Bytes2HexStr.Change(ASCIIData));
            Console.WriteLine("명령: " + Command.ToString());
            Console.WriteLine("데이터 타입: " + DataType.ToString());

            Console.WriteLine(string.Format("블록 수: {0}", BlocCnt));
            Console.WriteLine(string.Format("에러: " + Error.ToString()));
            int i = 0;
            if (Command == XGTFEnetCommand.READ_REQT || Command == XGTFEnetCommand.READ_RESP)
            {
                foreach (var d in StorageDictionary)
                {
                    Console.Write(string.Format("[{0}] name: {1}", ++i, d.Key));
                    Console.WriteLine(string.Format(" value: {0}", d.Value));
                }
            }
        }
    }
}