using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGTCnetProtocol를 LSIS-XGT PLC에 보낼 Cnet Protocol-ASCII구조로 압축하고
    /// LSIS-XGT PLC에서 받은 Cnet Protocol-ASCII 데이터를 XGTCnetProtocol로 해석한다.
    /// </summary>
    public class XGTCnetCompressor : IProtocolCompressor
    {
        /// <summary>
        /// XGTCnetProtocol를 LSIS-XGT PLC에 보낼 Cnet Protocol-ASCII구조로 압축하여 반환한다.
        /// </summary>
        /// <param name="protocol">XGTCnetProtocol</param>
        /// <returns>Cnet Protocol-ASCII</returns>
        public virtual byte[] Encode(IProtocol protocol)
        {
            const int ITEM_MAX_COUNT = 16;
            const int ADDRESS_STRING_MAX_LENGTH = 16;

            List<byte> buf = new List<byte>();
            var cnet = protocol as XGTCnetProtocol;
            cnet.Header = ControlChar.ENQ;
            cnet.Tail = ControlChar.EOT;
            cnet.Error = XGTCnetError.OK;

            buf.Add(cnet.Header.ToByte());
            buf.AddRange(XGTCnetTranslator.LocalPortToASCII(cnet.LocalPort));
            buf.Add(cnet.Command.ToByte());
            buf.AddRange(cnet.CommandType.ToBytes());
            buf.AddRange(XGTCnetTranslator.BlockDataToASCII(cnet.Data.Count));
            if (cnet.Data.Count > ITEM_MAX_COUNT)
                throw new Exception("블록 수 초과 에러");

            foreach (var item in cnet.Data)
            {
                if (item.Address.Length > ADDRESS_STRING_MAX_LENGTH)
                    throw new Exception("주소 문자열 길이 초과 에러");
                buf.AddRange(XGTCnetTranslator.BlockDataToASCII(item.Address.Length));
                buf.AddRange(XGTCnetTranslator.AddressDataToASCII(item.Address));
                if (cnet.Command == XGTCnetCommand.W)
                    buf.AddRange(XGTCnetTranslator.ValueDataToASCII(item.Value, protocol.Type));
            }
            buf.Add(cnet.Tail.ToByte());
            return buf.ToArray();
        }

        /// <summary>
        /// LSIS-XGT PLC에서 받은 Cnet Protocol-ASCII 데이터를 XGTCnetProtocol로 해석하여 반환한다.
        /// </summary>
        /// <param name="ascii">Cnet Protocol-ASCII</param>
        /// <returns>XGTCnetProtocol</returns>
        public virtual IProtocol Decode(byte[] ascii, Type type)
        {
            const int HEADER_IDX = 0;
            const int LOCOL_IDX1 = 1;
            const int LOCOL_IDX2 = 2;
            const int COMMAND_IDX = 3;
            //const int CMDTYPE_IDX1 = 4;
            //const int CMDTYPE_IDX2 = 5;
            const int BLOCK_IDX1 = 6;
            const int BLOCK_IDX2 = 7;

            var header = (ControlChar)ascii[HEADER_IDX];
            var tail = (ControlChar)ascii.Last();
            if (!(header == ControlChar.ACK || header == ControlChar.NAK))
                throw new Exception("ASCII Header 분석 실패 에러");
            if (tail != ControlChar.ETX)
                throw new Exception("ASCII Tail 분석 실패 에러");
            XGTCnetCommand cmd = (XGTCnetCommand)ascii[COMMAND_IDX];
            ushort localport = XGTCnetTranslator.ASCIIToLocalPort(new byte[] { ascii[LOCOL_IDX1], ascii[LOCOL_IDX2] });
            XGTCnetProtocol cnet = new XGTCnetProtocol(typeof(ushort), localport, cmd);
            cnet.Type = type;
            cnet.Header = header;
            cnet.Tail = tail;

            if (cnet.Header == ControlChar.NAK)
            {
                var error_bytes = new byte[] { ascii[6], ascii[7], ascii[8], ascii[9] };
                cnet.Error = (XGTCnetError)XGTCnetTranslator.ErrorCodeToInteger(error_bytes);
                return cnet;
            }

            if (cmd == XGTCnetCommand.R)
            {
                if (cnet.Data == null) cnet.Data = new List<IProtocolData>(); else cnet.Data.Clear();
                int count = XGTCnetTranslator.ASCIIToBlockData(new byte[] { ascii[BLOCK_IDX1], ascii[BLOCK_IDX2] });
                int idx = 8;
                for (int i = 0; i < count; i++)
                {
                    ushort size = XGTCnetTranslator.ASCIIToBlockData(new byte[] { ascii[idx], ascii[idx + 1] });
                    idx += 2;
                    byte[] code = new byte[size * 2];
                    Buffer.BlockCopy(ascii, idx, code, 0, code.Length);
                    idx += code.Length;
                    object value = XGTCnetTranslator.ASCIIToValueData(code, type);
                    cnet.Data.Add(new ProtocolData(value));
                }
            }
            return cnet;
        }

        /// <summary>
        /// PLC Address에 해당되는 정수byte[]정보를 사용하여 적절한 자료형의 변수로 해석하여 반환한다.
        /// </summary>
        /// <param name="code">정수byte[]정보</param>
        /// <returns>정수</returns>
        public virtual object ConvertAutomatically(byte[] code, int size)
        {
            object value = null;
            switch (size)
            {
                case 1:
                    value = XGTCnetTranslator.ASCIIToValueData(code, typeof(byte));
                    break;
                case 2:
                    value = XGTCnetTranslator.ASCIIToValueData(code, typeof(ushort));
                    break;
                case 4:
                    value = XGTCnetTranslator.ASCIIToValueData(code, typeof(uint));
                    break;
                case 8:
                    value = XGTCnetTranslator.ASCIIToValueData(code, typeof(ulong));
                    break;
            }
            return value;
        }
    }
}
