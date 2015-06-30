
using System;
using System.Collections.Generic;
using System.Linq;

namespace DY.NET.LSIS.XGT
{
    public class XGTFEnetProtocol : AProtocol
    {
        protected const string ERROR_ENQ_IS_NULL_OR_EMPTY = "ENQDATAS HAVE PROBLEM (NULL OR EMPTY DATA)";
        protected const string ERROR_READED_MEM_COUNT_LIMIT = "ENQDATAS OVER LIMIT OF COUNT (NULL OR EMPTY DATA)";
        private const string ERROR_PROTOCOL_SB_DATACNT_LIMIT = "DATA COUNT(ASC BYTES) LIMITED 1400BYTE";
        protected const int READED_MEM_MAX_COUNT = 16;
        private const int INSTRUCTION_BASIC_FORMAT_SIZE = 10;
        public const int PROTOCOL_SB_MAX_DATA_CNT = 1400;
        private const ushort ERROR_STATE_CHECK_0 = 0X00FF;
        private const ushort ERROR_STATE_CHECK_1 = 0XFFFF;

        public XGTFEnetHeader Header { get; private set; } //heaer 
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
        }

        public static XGTFEnetProtocol CreateXGTFEnetProtocol(XGTFEnetHeader header, XGTFEnetCommand cmd, ushort block_cnt)
        {
            XGTFEnetProtocol instance = new XGTFEnetProtocol();
            instance.Command = cmd;
            instance.BlocCnt = block_cnt;
            instance.Header = header;
            return instance;
        }

        public static XGTFEnetProtocol CreateXGTFEnetProtocol(byte[] asc_data, XGTFEnetProtocol reqtProtocol)
        {
            XGTFEnetProtocol instance = new XGTFEnetProtocol();
            instance.ReqeustList.AddRange(reqtProtocol.ReqeustList);
            instance.OtherParty = reqtProtocol;
            instance.ASC2Protocol = asc_data;
            return instance;
        }

        public static XGTFEnetProtocol NewRSSProtocol(ushort tag, List<PValue> pvalues)
        {
            return NewRSSProtocol(XGTFEnetHeader.CreateXGTFEnetHeader(tag), pvalues);
        }

        public static XGTFEnetProtocol NewWSSProtocol(ushort tag, List<PValue> pvalues)
        {
            return NewWSSProtocol(XGTFEnetHeader.CreateXGTFEnetHeader(tag), pvalues);
        }

        public static XGTFEnetProtocol NewRSBProtocol(ushort tag, PValue pvalue, ushort block_cnt)
        {
            return NewRSBProtocol(XGTFEnetHeader.CreateXGTFEnetHeader(tag), pvalue, block_cnt);
        }

        public static XGTFEnetProtocol NewWSBProtocol(ushort tag, List<PValue> pvalues)
        {
            return NewWSBProtocol(XGTFEnetHeader.CreateXGTFEnetHeader(tag), pvalues);
        }

        /// <summary>
        /// 직접 변수 개별 읽기 RSS
        /// PLC에서 데이터 타입에 맞게 직접 변수이름을 지정하여 읽는 요청의 프로토콜 입니다
        /// 한번에 16개의 독립된 디바이스 메모리를 읽을 수가 있습니다
        /// <param name="header">XGTFEnetHeader 객체</param>
        /// <param name="pvalues">PValue 리스트</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewRSSProtocol(XGTFEnetHeader header, List<PValue> pvalues)
        {
            if (pvalues.Count == 0 || pvalues == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (pvalues.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var protocol = CreateXGTFEnetProtocol(header, XGTFEnetCommand.READ_REQT, (ushort)pvalues.Count);
            protocol.ReqeustList.AddRange(pvalues); //깊은 복사
            protocol.DataType = pvalues.First().Type.ToXGTFEnetDataType();
            return protocol;
        }

        /// <summary>
        /// 직접 변수 개별 쓰기 WSS
        /// PLC 의 메모리 번지를 직접 지정하여 데이터 타입에 맞게 값을 쓰는 프로토콜입니다.
        /// <param name="header">XGTFEnetHeader 객체</param>
        /// <param name="pvalues">PValue 리스트</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewWSSProtocol(XGTFEnetHeader header, List<PValue> pvalues)
        {
            if (pvalues.Count == 0 || pvalues == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (pvalues.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var protocol = CreateXGTFEnetProtocol(header, XGTFEnetCommand.WRITE_REQT, (ushort)pvalues.Count);
            protocol.ReqeustList.AddRange(pvalues); //깊은 복사
            protocol.DataType = pvalues.First().Type.ToXGTFEnetDataType();
            return protocol;
        }

        /// <summary>
        /// 직접 변수 연속 읽기 RSB
        /// PLC 에서 지정된 번지의 메모리에서 지정된 개수만큼 데이터를 일렬로 읽는 기능을 제공하는 프로토콜 입니다
        /// 연속 읽기에는 바이트 타입 직접변수만 가능합니다
        /// </summary>
        /// <param name="header">XGTFEnetHeader 객체</param>
        /// <param name="pvalue">READ 시작점</param>
        /// <param name="block_cnt">읽을 개수 (최대 16개)</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewRSBProtocol(XGTFEnetHeader header, PValue pvalue, ushort block_cnt)
        {
            string glopa_name = pvalue.Name;
            Type type = pvalue.Type;
            if (!(pvalue.Type == typeof(Byte) || pvalue.Type == typeof(Byte))) //BYTE타입만 가능
                throw new ArgumentException("RSB COMMUNICATION ONLY SUPPORTED BYTE DATA TYPE");
            int buf_size = (block_cnt * pvalue.Type.ToSize() * 2);
            if (buf_size > PROTOCOL_SB_MAX_DATA_CNT)
                throw new ArgumentException(ERROR_PROTOCOL_SB_DATACNT_LIMIT);

            var protocol = CreateXGTFEnetProtocol(header, XGTFEnetCommand.WRITE_REQT, 1);
            protocol.DataType = XGTFEnetDataType.CONTINUATION;
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
        /// <param name="header">XGTFEnetHeader 객체</param>
        /// <param name="pvalues">PValue 리스트</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        private static XGTFEnetProtocol NewWSBProtocol(XGTFEnetHeader header, List<PValue> pvalues)
        {
            if (pvalues.Count == 0 || pvalues == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            foreach (var pv in pvalues)
                if (!(pv.Type == typeof(Byte) || pv.Type == typeof(Byte))) //BYTE타입만 가능
                    throw new ArgumentException("RSB COMMUNICATION ONLY SUPPORTED BYTE DATA TYPE");
            int size_sum = 0;
            foreach (var pv in pvalues)
                size_sum += pv.Type.ToSize() * 2;
            if (size_sum > PROTOCOL_SB_MAX_DATA_CNT)
                throw new ArgumentException(ERROR_PROTOCOL_SB_DATACNT_LIMIT);

            var protocol = CreateXGTFEnetProtocol(header, XGTFEnetCommand.WRITE_REQT, 1);
            protocol.ReqeustList.AddRange(pvalues); //깊은 복사
            protocol.DataType = XGTFEnetDataType.CONTINUATION;
            return protocol;
        }

        /// <summary>
        /// 맴머 변수의 정보를 토대로 원시 프로토콜 데이터를 계산합니다.
        /// </summary>
        internal override void AssembleProtocol()
        {
            List<byte> asc_data = new List<byte>();
            asc_data.AddRange(Command.ToBytes().Reverse());     //명령어
            asc_data.AddRange(DataType.ToBytes().Reverse());    //데이터 타입
            asc_data.AddRange(new byte[] {0,0}); //예약
            asc_data.AddRange(CV2BR.ToBytes(BlocCnt, typeof(ushort))); //블록수
            //RSS
            if (Command == XGTFEnetCommand.READ_REQT && DataType != XGTFEnetDataType.CONTINUATION)
            {
                foreach(var pv in ReqeustList)
                {
                    asc_data.AddRange(CV2BR.ToBytes(pv.Name.Length, typeof(UInt16))); //변수 길이
                    asc_data.AddRange(CV2BR.ToBytes(pv.Name)); //변수 이름
                }
            }
            //RSB
            else if (Command == XGTFEnetCommand.READ_REQT && DataType == XGTFEnetDataType.CONTINUATION)
            {
                asc_data.AddRange(CV2BR.ToBytes(ReqeustList.First().Name.Length, typeof(UInt16))); //변수 길이
                asc_data.AddRange(CV2BR.ToBytes(ReqeustList.First().Name)); //변수 이름
                asc_data.AddRange(CV2BR.ToBytes(BlocCnt, typeof(UInt16))); //읽을 개수
            }
            //WSS
            else if (Command == XGTFEnetCommand.WRITE_REQT && DataType != XGTFEnetDataType.CONTINUATION)
            {
                foreach (var pv in ReqeustList)
                {
                    asc_data.AddRange(CV2BR.ToBytes(pv.Name.Length, typeof(UInt16))); //변수 길이
                    asc_data.AddRange(CV2BR.ToBytes(pv.Name)); //변수 이름
                    asc_data.AddRange(CV2BR.ToBytes(pv.Type.ToSize())); //변수 크기
                    asc_data.AddRange(CV2BR.ToBytes(pv.Value, pv.Type)); //변수 값
                }
            }
            //WSB
            else if (Command == XGTFEnetCommand.READ_REQT && DataType == XGTFEnetDataType.CONTINUATION)
            {
                asc_data.AddRange(CV2BR.ToBytes(ReqeustList.First().Name.Length, typeof(UInt16))); //변수 길이
                asc_data.AddRange(CV2BR.ToBytes(ReqeustList.First().Name)); //변수 이름
                asc_data.AddRange(CV2BR.ToBytes(ReqeustList.Count(), typeof(UInt16))); //쓸 데이터 개수
                foreach (PValue pv in ReqeustList)
                    asc_data.AddRange(CV2BR.ToBytes(pv.Value, pv.Type));
            }
            var header_byte_data = Header.GetBytes(asc_data.Count());
            ASC2Protocol = new byte[header_byte_data.Length + asc_data.Count()];
            Buffer.BlockCopy(header_byte_data, 0, ASC2Protocol, 0, header_byte_data.Length);
            Buffer.BlockCopy(asc_data.ToArray(), 0, ASC2Protocol, header_byte_data.Length, asc_data.Count);
        }
        /// <summary>
        /// 받은 원시 프로토콜 데이터를 바탕으로 프로토콜 구조와 데이터를 파악합니다.
        /// </summary>
        internal override void AnalysisProtocol()
        {
            Header = XGTFEnetHeader.CreateXGTFEnetHeader(ASC2Protocol);
            Command = (XGTFEnetCommand)CV2BR.ToValue(new byte[] { ASC2Protocol[20], ASC2Protocol[21] }, typeof(UInt16));
            DataType = (XGTFEnetDataType)CV2BR.ToValue(new byte[] { ASC2Protocol[22], ASC2Protocol[23] }, typeof(UInt16));
            //var reserved = CV2BR.ToValue(new byte[] { ASC2Protocol[24], ASC2Protocol[25] }, typeof(UInt16)); //예약 영역
            ushort err_state = (ushort)CV2BR.ToValue(new byte[] { ASC2Protocol[26], ASC2Protocol[27] }, typeof(UInt16)); //에러 상태
            ushort bloc_errcode = (ushort) CV2BR.ToValue(new byte[] { ASC2Protocol[28], ASC2Protocol[29] }, typeof(UInt16)); //에러코드 or 블록 수 
            if (err_state == ERROR_STATE_CHECK_0 || err_state == ERROR_STATE_CHECK_1)
            {
                Error = (XGTFEnetProtocolError)bloc_errcode;
                return;
            }
            BlocCnt = bloc_errcode;
            byte[] data = new byte[ASC2Protocol.Length - XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE - INSTRUCTION_BASIC_FORMAT_SIZE];
            int data_idx = 0;
            Buffer.BlockCopy(ASC2Protocol, XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE + INSTRUCTION_BASIC_FORMAT_SIZE, data, 0, data.Length);
            //RSS
            if (Command == XGTFEnetCommand.READ_RESP && DataType != XGTFEnetDataType.CONTINUATION)
            {
                for(int i = 0; i < BlocCnt; i++)
                {
                    ushort sizeOfType = (ushort)CV2BR.ToValue(new byte[] { data[data_idx + 0], data[data_idx + 1] }, typeof(ushort));
                    data_idx += 2;
                    byte[] data_arr = new byte[sizeOfType];
                    Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                    data_idx += data_arr.Length;
                    ResponseDic.Add(ReqeustList[i].Name, CV2BR.ToValue(data_arr, ReqeustList[i].Type));
                }
            }
            //RSB
            else if (Command == XGTFEnetCommand.READ_RESP && DataType == XGTFEnetDataType.CONTINUATION)
            {
                int data_type_size = ReqeustList.First().Type.ToSize();
                for (int i = 0; i < BlocCnt / data_type_size; i++)
                {
                    byte[] data_arr = new byte[data_type_size];
                    Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                    ResponseDic.Add(ReqeustList[i].Name, CV2BR.ToValue(data_arr, ReqeustList[i].Type));
                    data_idx += data_arr.Length;
                }
            }
        }
    }
}