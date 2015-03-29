/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜 RWSB RWSS XYSS XYSB 에 사용할 수 있는 범용 클래스
 * 날짜: 2015-03-25
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetExclusiveProtocol : XGTCnetExclusiveProtocolFrame
    {
        public ushort BlockCnt;         //2byte
        List<ENQDataFormat> ENQDatas = new List<ENQDataFormat>();   //?byte
        List<ACKDataFormat> ACKDatas = new List<ACKDataFormat>();   //?byte
        public ushort RegisterNum;       //2byte
        public ushort WillDoDataCnt;     //읽거나 쓸 데이터의 개수 (BYTE = 데이터 타입 * 개수) 최대 240byte word는 120byte 가 한계 //2byte

        public static DataType GetDataTypeFromGlopaVarName(string VarName)
        {
            if (VarName[0] != '%')
                throw new ArgumentException("it's not glopa type variable name");
            DataType type = DataType.WORD;
            char t = VarName[2];
            switch (t)
            {
                case 'X':
                    type = DataType.BIT;
                    break;
                case 'B':
                    type = DataType.BYTE;
                    break;
                case 'W':
                    type = DataType.WORD;
                    break;
                case 'D':
                    type = DataType.DWORD;
                    break;
                case 'L':
                    type = DataType.LWORD;
                    break;
                default:
                    throw new ArgumentException("unknown data type");
            }
            return type;
        }

        #region For ENQ Protocol Type

        protected void AddProtocolRSS(List<byte> asc_list)
        {
            asc_list.AddRange(TransData.ToASC(ENQDatas.Count, 2));               // 블록 수
            foreach (ENQDataFormat e in ENQDatas)
            {
                asc_list.AddRange(TransData.ToASC(e.Var_Name.Length, 2));
                asc_list.AddRange(TransData.ToASC(e.Var_Name, 1));
            }
        }

        protected void AddProtocolWSS(List<byte> asc_list)
        {
            asc_list.AddRange(TransData.ToASC(ENQDatas.Count, 2));               // 블록 수
            foreach (ENQDataFormat e in ENQDatas)
            {
                asc_list.AddRange(TransData.ToASC(e.Var_Name.Length, 2));
                asc_list.AddRange(TransData.ToASC(e.Var_Name, 1));
                asc_list.AddRange(TransData.IntDataToASC(e.Data, GetDataTypeFromGlopaVarName(e.Var_Name)));
            }
        }

        protected void AddProtocolXSS(List<byte> asc_list)
        {
            //RSS 블록 수(2 Byte) 변수 길이(2 Byte) 변수 이름(16 Byte) 
            asc_list.AddRange(TransData.ToASC(RegisterNum, 2));                  // 등록번호
            asc_list.Add((byte)'R');
            asc_list.Add((byte)'S');
            asc_list.Add((byte)'S');
            asc_list.AddRange(TransData.ToASC(ENQDatas.Count, 2));               // 블록 수 
            foreach (ENQDataFormat e in ENQDatas)
            {
                if (e.Var_Name.Length > 16)
                    throw new ArgumentOutOfRangeException("Var_Name's length over 16.");
                asc_list.AddRange(TransData.ToASC(e.Var_Name.Length, 2));        // 변수이름길이
                asc_list.AddRange(TransData.ToASC(e.Var_Name, 1));               // 변수이름
            }
        }

        protected void AddProtocolYSS(List<byte> asc_list)
        {
            asc_list.AddRange(TransData.ToASC(RegisterNum, 2));                  // 등록번호
        }

        // not support bit data
        protected void AddProtocolRSB(List<byte> asc_list)
        {
            asc_list.AddRange(TransData.ToASC(ENQDatas[0].Var_Name.Length, 2));  // 변수이름길이
            asc_list.AddRange(TransData.ToASC(ENQDatas[0].Var_Name, 1));         // 변수이름
            asc_list.AddRange(TransData.ToASC(WillDoDataCnt, 2));                // 읽어들일 데이터의 개수
        }

        protected void AddProtocolWSB(List<byte> asc_list)
        {
            asc_list.AddRange(TransData.ToASC(ENQDatas[0].Var_Name.Length, 2));  // 변수이름길이
            asc_list.AddRange(TransData.ToASC(ENQDatas[0].Var_Name, 1));         // 변수이름
            asc_list.AddRange(TransData.ToASC(WillDoDataCnt, 2));                // 쓸 데이터의 개수
            asc_list.AddRange(TransData.ToASC(ENQDatas[0].Data, 1));             // 데이터
        }

        protected void AddProtocolXSB(List<byte> asc_list)
        {
            asc_list.AddRange(TransData.ToASC(RegisterNum, 2));                  // 등록번호
            asc_list.Add((byte)'R');
            asc_list.Add((byte)'S');
            asc_list.Add((byte)'B');

            if (ENQDatas[0].Var_Name.Length > 16)
                throw new ArgumentOutOfRangeException("Var_Name's length over 16.");
            asc_list.AddRange(TransData.ToASC(ENQDatas[0].Var_Name.Length, 2));  // 변수이름길이
            asc_list.AddRange(TransData.ToASC(ENQDatas[0].Var_Name, 1));         // 변수이름
            asc_list.AddRange(TransData.ToASC(WillDoDataCnt, 2));                // 쓸 데이터의 개수
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
        //헤더 국번 명령어 명령어타입 테일 프레임체크를 제외한 메인 데이터 부분만 추출해서 리턴합니다.
        protected byte[] GetMainData()
        {
            int asc_data_cnt = ASCData.Length - PROTOCOL_HEAD_SIZE - (IsExistBCCFromASCData() ? 2 : 1);
            byte[] asc_arr = new byte[asc_data_cnt];
            Buffer.BlockCopy(ASCData, PROTOCOL_HEAD_SIZE, asc_arr, 0, asc_data_cnt);
            return asc_arr;
        }

        protected void QueryProtocolRSS()
        {
            byte[] data = GetMainData();
            byte[] block_arr = { data[0], data[1] };
            BlockCnt = (ushort)TransData.ToHex(block_arr, typeof(ushort));

            int data_idx = 2;
            for (int i = 0; i < BlockCnt; i++)
            {
                byte[] data_size_arr = { data[data_idx + 0], data[data_idx + 1] };
                data_idx += 2;
                ACKDataFormat ack = new ACKDataFormat();
                ack.SizeOfType = (ushort)TransData.ToHex(data_size_arr, typeof(ushort));

                byte[] data_arr = new byte[ack.SizeOfType * 2];
                Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                data_idx += data_arr.Length;
                ack.Data = TransData.ToHex(data_arr, DataTypeExtensions.TypeOf((DataType)ack.SizeOfType));

                ACKDatas.Add(ack);
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
            RegisterNum = (ushort)TransData.ToHex(register_arr, typeof(ushort));
        }

        protected void QueryProtocolYSS()
        {
            byte[] data = GetMainData();
            byte[] register_arr = { data[0], data[1] };
            RegisterNum = (ushort)TransData.ToHex(register_arr, typeof(ushort));

            byte[] block_arr = { data[2], data[3] };
            BlockCnt = (ushort)TransData.ToHex(block_arr, typeof(ushort));

            int data_idx = 4;

            for (int i = 0; i < BlockCnt; i++)
            {
                ACKDataFormat ack = new ACKDataFormat();
                byte[] data_size_arr = { data[4], data[5] };
                data_idx += 2;
                ack.SizeOfType = (ushort)TransData.ToHex(data_size_arr, typeof(ushort));

                byte[] data_arr = new byte[ack.SizeOfType * 2];
                Buffer.BlockCopy(data, data_idx, data_arr, 0, data_arr.Length);
                ack.Data = TransData.ToHex(data_arr, DataTypeExtensions.TypeOf((DataType)ack.SizeOfType));
                data_idx += data_arr.Length;

                ACKDatas.Add(ack);
            }
        }

        protected void QueryProtocolRSB()
        {
            byte[] data = GetMainData();
            byte[] data_size_arr = { data[0], data[1] };
            ACKDataFormat ack = new ACKDataFormat();
            ack.SizeOfType = (ushort)TransData.ToHex(data_size_arr, typeof(ushort));

            byte[] data_arr = new byte[ack.SizeOfType * 2];
            Buffer.BlockCopy(data, data_size_arr.Length, data_arr, 0, data_arr.Length);
            ack.Data = TransData.ToHex(data_arr, DataTypeExtensions.TypeOf((DataType)ack.SizeOfType));
            ACKDatas.Add(ack);
        }

        protected void QueryProtocolWSB()
        {
            return;
        }

        protected void QueryProtocolXSB()
        {
            byte[] data = GetMainData();
            byte[] register_arr = { data[0], data[1] };
            RegisterNum = (ushort)TransData.ToHex(register_arr, typeof(ushort));
        }

        protected void QueryProtocolYSB()
        {
            byte[] data = GetMainData();
            byte[] register_arr = { data[0], data[1] };
            RegisterNum = (ushort)TransData.ToHex(register_arr, typeof(ushort));

            byte[] data_size_arr = { data[2], data[3] };
            ACKDataFormat ack = new ACKDataFormat();
            ack.SizeOfType = (ushort)TransData.ToHex(data_size_arr, typeof(ushort));

            byte[] data_arr = new byte[ack.SizeOfType * 2];
            Buffer.BlockCopy(data, data_size_arr.Length + register_arr.Length, data_arr, 0, data_arr.Length);
            ack.Data = TransData.ToHex(data_arr, DataTypeExtensions.TypeOf((DataType)ack.SizeOfType));

            ACKDatas.Add(ack);
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
        #endregion
    }
}