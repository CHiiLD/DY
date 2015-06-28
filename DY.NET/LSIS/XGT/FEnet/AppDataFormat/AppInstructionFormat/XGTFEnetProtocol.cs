
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTFEnetProtocol : AXGTProtocol
    {
        public const int PROTOCOL_SB_MAX_DATA_CNT = 1400;
        protected const string ERROR_PROTOCOL_SB_DATACNT_LIMIT = "DATA COUNT(ASC BYTES) LIMITED 1400BYTE";

        public XGTFEnetHeader Header { get; private set; } //heaer 
        public int BlocCnt { get; private set; } //블록수 
        public int ByteSize { get; private set; } //byte data size 
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

        public static XGTFEnetProtocol CreateXGTFEnetProtocol(XGTFEnetHeader header, XGTFEnetCommand cmd, int block_cnt)
        {
            XGTFEnetProtocol instance = new XGTFEnetProtocol();
            instance.Command = cmd;
            instance.BlocCnt = block_cnt;
            instance.Header = header;
            return instance;
        }

        /// <summary>
        /// 직접 변수 개별 읽기 RSS
        /// PLC에서 데이터 타입에 맞게 직접 변수이름을 지정하여 읽는 요청의 프로토콜 입니다
        /// 한번에 16개의 독립된 디바이스 메모리를 읽을 수가 있습니다
        /// <param name="header">XGTFEnetHeader 객체</param>
        /// <param name="pvalues">PValue 리스트</param>
        /// <returns>XGTFEnetProtocol 객체</returns>
        public static XGTFEnetProtocol NewRSSProtocol(XGTFEnetHeader header, List<PValue> pvalues)
        {
            if (pvalues.Count == 0 || pvalues == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (pvalues.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var protocol = CreateXGTFEnetProtocol(header, XGTFEnetCommand.READ_REQT, pvalues.Count);
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
        public static XGTFEnetProtocol NewWSSProtocol(XGTFEnetHeader header, List<PValue> pvalues)
        {
            if (pvalues.Count == 0 || pvalues == null)
                throw new ArgumentException(ERROR_ENQ_IS_NULL_OR_EMPTY);
            if (pvalues.Count > READED_MEM_MAX_COUNT)
                throw new ArgumentException(ERROR_READED_MEM_COUNT_LIMIT);

            var protocol = CreateXGTFEnetProtocol(header, XGTFEnetCommand.WRITE_REQT, pvalues.Count);
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
        public static XGTFEnetProtocol NewRSBProtocol(XGTFEnetHeader header, PValue pvalue, ushort block_cnt)
        {
            string glopa_name = pvalue.Name;
            Type type = pvalue.Type;
            if (!(pvalue.Type == typeof(Byte) || pvalue.Type == typeof(Byte))) //BYTE타입만 가능
                throw new ArgumentException("RSB COMMUNICATION ONLY SUPPORTED BYTE DATA TYPE");
            int buf_size = (block_cnt * pvalue.Type.ToSize() * 2);
            if (buf_size > PROTOCOL_SB_MAX_DATA_CNT)
                throw new ArgumentException(ERROR_PROTOCOL_SB_DATACNT_LIMIT);

            var protocol = CreateXGTFEnetProtocol(header, XGTFEnetCommand.WRITE_REQT, block_cnt);
            protocol.BlocCnt = block_cnt;
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
        public static XGTFEnetProtocol NewWSBProtocol(XGTFEnetHeader header, List<PValue> pvalues)
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

            var protocol = CreateXGTFEnetProtocol(header, XGTFEnetCommand.WRITE_REQT, pvalues.Count);
            protocol.ReqeustList.AddRange(pvalues); //깊은 복사
            protocol.DataType = XGTFEnetDataType.CONTINUATION;
            return protocol;
        }

        /// <summary>
        /// 맴머 변수의 정보를 토대로 원시 프로토콜 데이터를 구합니다.
        /// </summary>
        internal override void AssembleProtocol()
        {

        }
        /// <summary>
        /// 받은 원시 프로토콜 데이터를 바탕으로 프로토콜 구조와 데이터를 파악합니다.
        /// </summary>
        internal override void AnalysisProtocol()
        {

        }
    }
}