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
    /// XGT Cnet 통신을 위한 응답 프로토콜 클래스
    /// </summary>
    public class XGTCnetResponseProtocol : XGTCnetProtocolFrame
    {
        /// <summary>
        /// PROTOCOL MAIN DATAS
        /// </summary>
        public List<RespDataFmt> RespDatas { get; protected set; }

        /// <summary>
        /// XGTCnetExclusiveProtocol 복사생성자
        /// </summary>
        /// <param name="that"> 복사하고자 할 XGTCnetExclusiveProtocol 객체 </param>
        internal XGTCnetResponseProtocol(XGTCnetResponseProtocol that)
            : base(that)
        {
            Init();
            
            this.RespDatas.AddRange(that.RespDatas);
        }

        protected XGTCnetResponseProtocol()
            : base()
        {
            Init();
        }

        protected XGTCnetResponseProtocol(byte[] binaryDatas)
            : base(binaryDatas)
        {
            Init();
        }

        /// <summary>
        /// 변수 초기화
        /// </summary>
        private void Init()
        {
            RespDatas = new List<RespDataFmt>();
        }

        #region static factory construct method

        /// <summary>
        /// PLC에서 받은 원시 데이터를 분석하여 XGTCnetExclusiveProtocol 클래스로 변환&데이터분석하여 응답 프로토콜 클래스 리턴.
        /// </summary>
        /// <param name="binaryData"> 원시데이터 </param>
        /// <param name="reqtProtocol"> 요청 프로토콜 클래스 </param>
        /// <returns> 응답 프로토콜 클래스 </returns>
        internal static XGTCnetResponseProtocol CreateResponseProtocol(byte[] binaryData, XGTCnetRequestProtocol reqtProtocol)
        {
            XGTCnetResponseProtocol protocol = new XGTCnetResponseProtocol(binaryData);
            protocol.ProtocolPointer = reqtProtocol;
            return protocol;
        }

        #endregion

        #region For ACK Protocol Type

        protected void QueryProtocolRSS()
        {
            byte[] data = GetMainData();
            byte[] block_arr = { data[0], data[1] };
            BlockCount = (ushort)CA2C.ToValue(block_arr, typeof(ushort));

            int data_idx = 2;
            for (int i = 0; i < BlockCount; i++)
            {
                byte[] data_size_arr = { data[data_idx + 0], data[data_idx + 1] };
                data_idx += 2;
                ushort sizeOfType = (ushort)CA2C.ToValue(data_size_arr, typeof(ushort));

                byte[] data_arr = new byte[sizeOfType * 2];
                Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                data_idx += data_arr.Length;
                object value = CA2C.ToValue(data_arr, ((PLCVarType)sizeOfType));
                RespDatas.Add(new RespDataFmt(sizeOfType, value));
            }
        }

        protected void QueryProtocolWSS()
        {
            return;
        }

        protected void QueryProtocolXSS()
        {
            byte[] type = GetMainData(); // { ProtocolData[4], ProtocolData[5] };
            RegiNumber = (ushort)CA2C.ToValue(type, typeof(ushort));
        }

        protected void QueryProtocolYSS()
        {
            byte[] data = GetMainData();
            byte[] register_arr = { data[0], data[1] };
            RegiNumber = (ushort)CA2C.ToValue(register_arr, typeof(ushort));

            byte[] block_arr = { data[2], data[3] };
            BlockCount = (ushort)CA2C.ToValue(block_arr, typeof(ushort));

            int data_idx = 4;

            for (int i = 0; i < BlockCount; i++)
            {
                byte[] data_size_arr = { data[data_idx + 0], data[data_idx + 1] };
                data_idx += 2;
                ushort sizeOfType = (ushort)CA2C.ToValue(data_size_arr, typeof(ushort));

                byte[] data_arr = new byte[sizeOfType * 2];
                Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                object value = CA2C.ToValue(data_arr, (PLCVarType)sizeOfType);
                data_idx += data_arr.Length;

                RespDatas.Add(new RespDataFmt(sizeOfType, value));
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
            BlockCount = (ushort)CA2C.ToValue(bcnt_arr, typeof(ushort)); // 정말 이해할 수 없는 정보.. LSIS 측에 문의해봐야함

            // 데이터 개수 정보 쿼리
            byte[] size_arr = { data[2], data[3] };
            ushort data_len = (ushort)CA2C.ToValue(size_arr, typeof(ushort));

            // 그로파 변수이름에서 자료형 정보를 얻어낸다
            string var_name = ((XGTCnetRequestProtocol)ProtocolPointer).ReqtDatas[0].GlopaVarName;
            PLCVarType plc_var_type = Glopa.GetDataType(var_name);
            int data_type_size = plc_var_type.ToSize();

            int data_idx = 4;
            // 일렬로 정렬된 데이터들을 데이터타입에 따라 적절하게 파싱하여 데이터 쿼리
            for (int i = 0; i < data_len / data_type_size; i++)
            {
                byte[] temp_arr = new byte[data_type_size * 2];
                Buffer.BlockCopy(data, data_idx, temp_arr, 0, temp_arr.Length);
                object value = CA2C.ToValue(temp_arr, plc_var_type);
                RespDatas.Add(new RespDataFmt((ushort)data_type_size, value));
                data_idx += temp_arr.Length;
            }
        }

        protected void QueryProtocolWSB()
        {
            return;
        }

        protected void QueryProtocolXSB()
        {
            QueryProtocolXSS();
        }

        protected void QueryProtocolYSB()
        {
            var data = GetMainData();
            // 등록 번호 정보 쿼리
            byte[] bcnt_arr = { data[0], data[1] };
            RegiNumber = (ushort)CA2C.ToValue(bcnt_arr, typeof(ushort)); // 정말 이해할 수 없는 정보.. LSIS 측에 문의해봐야함

            // 데이터 개수 정보 쿼리
            byte[] size_arr = { data[2], data[3] };
            ushort data_len = (ushort)CA2C.ToValue(size_arr, typeof(ushort));

            // 그로파 변수이름에서 자료형 정보를 얻어낸다
            string var_name = ((XGTCnetRequestProtocol)ProtocolPointer).ReqtDatas[0].GlopaVarName;
            PLCVarType plc_var_type = Glopa.GetDataType(var_name);
            int data_type_size = plc_var_type.ToSize();

            int data_idx = 4;
            // 일렬로 정렬된 데이터들을 데이터타입에 따라 적절하게 파싱하여 데이터 쿼리
            for (int i = 0; i < data_len / data_type_size; i++)
            {
                byte[] temp_arr = new byte[data_type_size * 2];
                Buffer.BlockCopy(data, data_idx, temp_arr, 0, temp_arr.Length);
                object value = CA2C.ToValue(temp_arr, plc_var_type);
                RespDatas.Add(new RespDataFmt((ushort)data_type_size, value));
                data_idx += temp_arr.Length;
            }
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

        protected override void AttachProtocolFrame(List<byte> asc_list)
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

            int cnt = 0;
            cnt = 0;
            foreach (RespDataFmt a in RespDatas)
            {
                Console.WriteLine(string.Format("[{0}] ACKData 변수 바이트 크기 : {1}", ++cnt, a.SizeOfType));
                if (a.Data != null)
                    Console.WriteLine(string.Format("[{0}] ACKData 값: {1}", cnt, a.Data));
            }
        }
    }
}
