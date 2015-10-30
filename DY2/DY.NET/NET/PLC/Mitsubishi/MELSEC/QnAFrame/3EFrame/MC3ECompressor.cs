using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public class MC3ECompressor : IMCProtocolCompressor
    {
        public MCProtocolFormat Format { get; set; }

        /* * * * * * * * * * * * * * * * * * * * * *
         * Request Data Frame
         * * * * * * * * * * * * * * * * * * * * * *
         * ASCII
         * 
         * |      | READ | WRITE |
         * | REQT |   A  |   C   |
         * | RESP |   B  |   *   |
         * * * * * * * * * * * * * * * * * * * * * *
         * BINARY
         * 
         * |      |   READ   | WRITE |
         * | REQT |   C(A?)  |   C   |
         * | RESP |   B      |   *   |
         * * * * * * * * * * * * * * * * * * * * * */

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
         * 캐릭터 부
         * | * | 부분은 3.1항에서 설명하고 있습니다.
         * 지정 디바이스 점수분의 데이터의 내용과 배열은 3.1항을 참조
         * 
         * ASCII * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
         * 읽기 시
         * | 캐릭터A || * | 서브명령어(4) | 디바이스코드(2) | 선두디바이스(6) | 디바이스점수(4) | * |
         * | 캐릭터B || * | 지정디바이스 점수분의 데이터 | * |
         * | 캐릭터C || * | 서브명령어(4) | 디바이스코드(2) | 선두디바이스(6) | 디바이스점수(4) | 지정디바이스 점수분의 데이터 | * |
         * BINARY* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
         * 읽기 시
         * | 캐릭터A || * | 서브명령어(2) | 선두디바이스(3) | 디바이스코드(1) | 디바이스점수(2) | * |
         * | 캐릭터B || * | 지정디바이스 점수분의 데이터 | * |
         * 쓰기 시
         * | 캐릭터C || * | 서브명령어(2) | 선두디바이스(3) | 디바이스코드(1) | 디바이스점수(2) | 지정디바이스 점수분의 데이터 | * |
         * 
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        /* 3.3.2 119p
         * 비트단위 일괄읽기 0401 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
         * ASCII 교신으로 비트 데이터를 읽는 경우 
         * 송신으로 0x00, 0x01로 비트를 나타냄(1byte)
         * M100에서 8점분을 읽었을 경우: 0x30 0x30 0x30 0x31 0x30 0x31 0x31 => M103, M106, M107은 ON
         * 
         * BIARY 교신으로 비트 데이터를 읽는 경우  (HL)
         * M100에서 8점분을 읽었을 경우: 0x00 0x01 0x00 0x11 => M103, M106, M107은 ON
         * 디바이스 점수가 홀수인 경우 최후에 1/2바이트(0x00)가 부가된다 -> ? 3.1.7항 참조
         * 
         * 비트단위 일괄쓰기 1401 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
         * ASCII 교신으로 비트 데이터를 쓰는 경우
         * M100에서 8점분을 쓸 경우: 0x31 0x31 0x30 0x30 0x31 0x31 0x30 0x30 => M100, M101, M104, M105를 ON
         * BIARY 교신으로 비트 데이터를 쓰는 경우 (HL)
         * M100에서 8점분을 쓰는 경우: 0x11 0x00 0x11 0x00 => M100, M101, M104, M105 ON
         * 
         * 워드단위 일괄읽기 0401
         * ! 워드단위로 비트단위 디바이스를 읽을 때와 워드단위 디바이스를 읽을 때 차이가 남
         */

        public MC3ECompressor()
        {
            Format = MCProtocolFormat.ASCII;
        }

        public virtual byte[] Encode(IProtocol protocol)
        {
            return null;
        }

        public virtual IProtocol Decode(byte[] ascii, IProtocol request)
        {
            return null;
        }

        private object ToValue(Type type, params byte[] bytes)
        {
            object result = null;
            if (type == typeof(String))
            {
                result = StringFormatTranslator.ByteArrayToString(bytes);
            }
            else if (Format == MCProtocolFormat.BINARY)
            {
                if (type == typeof(Int16) || type == typeof(UInt16))
                    Array.Reverse(bytes);
                result = BinaryFormatTranslator.BinaryToInteger(type, bytes);
            }
            else if (Format == MCProtocolFormat.ASCII)
            {
                result = ASCIIFormatTranslator.HexASCIIToInteger(type, bytes);
            }
            return result;
        }

        private TOutput ToValue<TOutput>(params byte[] bytes)
        {
            return (TOutput)ToValue(typeof(TOutput), bytes);
        }

        private byte[] ToCode(Type type, object value)
        {
            byte[] result = null;
            if (type == typeof(String))
            {
                result = StringFormatTranslator.StringToByteArray(value as string);
            }
            else if (Format == MCProtocolFormat.BINARY)
            {
                result = BinaryFormatTranslator.IntegerToBinary(type, value);
                if (type == typeof(Int16) || type == typeof(UInt16))
                    Array.Reverse(result);
            }
            else if (Format == MCProtocolFormat.ASCII)
            {
                result = ASCIIFormatTranslator.IntegerToHexASCII(type, value);
            }
            return result;
        }

        private byte[] ToCode<TOutput>(object value)
        {
            return ToCode(typeof(TOutput), value);
        }

        private delegate byte[] ToByteArray(Type type, object value);

        public byte[] EncodeQHeader(IMCQHeader qHeader)
        {
            List<byte> buf = new List<byte>();
            buf.AddRange(ToCode(qHeader.NetworkNumber.GetType(), qHeader.NetworkNumber));
            buf.AddRange(ToCode(qHeader.PLCNumber.GetType(), qHeader.PLCNumber));
            buf.AddRange(ToCode(qHeader.ModuleIONumber.GetType(), qHeader.ModuleIONumber));
            buf.AddRange(ToCode(qHeader.ModuleLocalNumber.GetType(), qHeader.ModuleLocalNumber));
            buf.AddRange(ToCode(qHeader.DataLength.GetType(), qHeader.DataLength));
            buf.AddRange(ToCode(qHeader.CPUMonitorTimer.GetType(), qHeader.CPUMonitorTimer));
            return buf.ToArray();
        }

        public void DecodeQHeader(byte[] d, IMCQHeader qHeader)
        {
            switch (Format)
            {
                case MCProtocolFormat.ASCII:
                    DecodeQHeaderByASCII(d, qHeader);
                    break;
                case MCProtocolFormat.BINARY:
                    DecodeQHeaderByBinary(d, qHeader);
                    break;
            }
        }

        private void DecodeQHeaderByBinary(byte[] d, IMCQHeader qHeader)
        {
            const int NET_IDX = 2;
            const int PLC_IDX = 3;
            const int IO_IDX1 = 4;
            const int IO_IDX2 = 5;
            const int LO_IDX = 6;
            const int LEN_IDX1 = 7;
            const int LEN_IDX2 = 8;
            const int END_IDX1 = 9;
            const int END_IDX2 = 10;

            qHeader.NetworkNumber = ToValue<byte>(d[NET_IDX]);
            qHeader.PLCNumber = ToValue<byte>(d[PLC_IDX]);
            qHeader.ModuleIONumber = ToValue<ushort>(d[IO_IDX1], d[IO_IDX2]);
            qHeader.ModuleLocalNumber = ToValue<byte>(d[LO_IDX]);
            qHeader.DataLength = ToValue<ushort>(d[LEN_IDX1], d[LEN_IDX2]);
            qHeader.Error = (MCEFrameError)ToValue<ushort>(d[END_IDX1], d[END_IDX2]);
        }

        private void DecodeQHeaderByASCII(byte[] d, IMCQHeader qHeader)
        {
            const int NET_IDX1 = 4;
            const int NET_IDX2 = 5;
            const int PLC_IDX1 = 6;
            const int PLC_IDX2 = 7;
            const int IO_IDX1 = 8;
            const int IO_IDX2 = 9;
            const int IO_IDX3 = 10;
            const int IO_IDX4 = 11;
            const int LO_IDX1 = 12;
            const int LO_IDX2 = 13;
            const int LEN_IDX1 = 14;
            const int LEN_IDX2 = 15;
            const int LEN_IDX3 = 16;
            const int LEN_IDX4 = 17;
            const int END_IDX1 = 18;
            const int END_IDX2 = 19;
            const int END_IDX3 = 20;
            const int END_IDX4 = 21;

            qHeader.NetworkNumber = ToValue<byte>(d[NET_IDX1], d[NET_IDX2]);
            qHeader.PLCNumber = ToValue<byte>(d[PLC_IDX1], d[PLC_IDX2]);
            qHeader.ModuleIONumber = ToValue<ushort>(d[IO_IDX1], d[IO_IDX2], d[IO_IDX3], d[IO_IDX4]);
            qHeader.ModuleLocalNumber = ToValue<byte>(d[LO_IDX1], d[LO_IDX2]);
            qHeader.DataLength = ToValue<ushort>(d[LEN_IDX1], d[LEN_IDX2], d[LEN_IDX3], d[LEN_IDX4]);
            qHeader.Error = (MCEFrameError)ToValue<ushort>(d[END_IDX1], d[END_IDX2], d[END_IDX3], d[END_IDX4]);
        }

        private void DecodeErrorInfoPartByBinary(byte[] d, MC3EProtocol mc3e)
        {
            const int NET_IDX = 22;
            const int PLC_IDX = 23;
            const int IO_IDX1 = 24;
            const int IO_IDX2 = 25;
            const int LO_IDX = 26;
            const int CMD_IDX1 = 27;
            const int CMD_IDX2 = 28;
            const int SCMD_IDX1 = 29;
            const int SCMD_IDX2 = 30;

            mc3e.ErrorNetworkNumber = ToValue<byte>(d[NET_IDX]);
            mc3e.ErrorPLCNumber = ToValue<byte>(d[PLC_IDX]);
            mc3e.ErrorModuleIONumber = ToValue<ushort>(d[IO_IDX1], d[IO_IDX2]);
            mc3e.ErrorModuleLocalNumber = ToValue<byte>(d[LO_IDX]);
            mc3e.ErrorCommand = (MCQnACommand)ToValue<ushort>(d[CMD_IDX1], d[CMD_IDX2]);
            mc3e.AnalysisSubCommand(ToValue<ushort>(d[SCMD_IDX1], d[SCMD_IDX2]));
        }

        private void DecodeErrorInfoPartByASCII(byte[] d, MC3EProtocol mc3e)
        {
            const int NET_IDX1 = 22;
            const int NET_IDX2 = 23;
            const int PLC_IDX1 = 24;
            const int PLC_IDX2 = 25;
            const int IO_IDX1 = 26;
            const int IO_IDX2 = 27;
            const int IO_IDX3 = 28;
            const int IO_IDX4 = 29;
            const int LO_IDX1 = 30;
            const int LO_IDX2 = 31;
            const int CMD_IDX1 = 32;
            const int CMD_IDX2 = 33;
            const int CMD_IDX3 = 34;
            const int CMD_IDX4 = 35;
            const int SCMD_IDX1 = 36;
            const int SCMD_IDX2 = 37;
            const int SCMD_IDX3 = 38;
            const int SCMD_IDX4 = 39;

            mc3e.ErrorNetworkNumber = ToValue<byte>(d[NET_IDX1], d[NET_IDX2]);
            mc3e.ErrorPLCNumber = ToValue<byte>(d[PLC_IDX1], d[PLC_IDX2]);
            mc3e.ErrorModuleIONumber = ToValue<ushort>(d[IO_IDX1], d[IO_IDX2], d[IO_IDX3], d[IO_IDX4]);
            mc3e.ErrorModuleLocalNumber = ToValue<byte>(d[LO_IDX1], d[LO_IDX2]);
            mc3e.ErrorCommand = (MCQnACommand)ToValue<ushort>(d[CMD_IDX1], d[CMD_IDX2], d[CMD_IDX3], d[CMD_IDX4]);
            mc3e.AnalysisSubCommand(ToValue<ushort>(d[SCMD_IDX1], d[SCMD_IDX2], d[SCMD_IDX3], d[SCMD_IDX4]));
        }
    }
}