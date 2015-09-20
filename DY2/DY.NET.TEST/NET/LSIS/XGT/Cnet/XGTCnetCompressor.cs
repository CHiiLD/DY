using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetCompressor : IProtocolCompressor
    {
        public virtual byte[] Encode(IProtocol protocol)
        {
            const int ITEM_MAX_COUNT = 16;
            const int ADDRESS_STRING_MAX_LENGTH = 12;

            List<byte> buf = new List<byte>();
            var cnet = protocol as XGTCnetProtocol;
            cnet.Header = XGTCnetHeader.ENQ;
            cnet.Tail = XGTCnetHeader.EOT;
            cnet.Error = XGTCnetError.OK;

            buf.Add(cnet.Header.ToByte());
            buf.AddRange(XGTCnetTranslator.LocalPortToInfoData(cnet.LocalPort));
            buf.Add(cnet.Command.ToByte());
            buf.AddRange(cnet.CommandType.ToBytes());
            buf.AddRange(XGTCnetTranslator.Int32ToInfoData(cnet.Items.Count));
            if (cnet.Items.Count > ITEM_MAX_COUNT)
                throw new Exception("블록 수 초과 에러");

            foreach (var item in cnet.Items)
            {
                if (item.Address.Length > ADDRESS_STRING_MAX_LENGTH)
                    throw new Exception("주소 문자열 길이 초과 에러");
                buf.AddRange(XGTCnetTranslator.Int32ToInfoData(item.Address.Length));
                buf.AddRange(XGTCnetTranslator.AddressDataToASCII(item.Address));
                if (cnet.Command == XGTCnetCommand.W)
                    buf.AddRange(XGTCnetTranslator.ValueDataToASCII(item.Value));
            }

            buf.Add(cnet.Tail.ToByte());
            return buf.ToArray();
        }

        public virtual IProtocol Decode(byte[] ascii)
        {
            var header = (XGTCnetHeader)ascii.First();
            var tail = (XGTCnetHeader)ascii.Last();
            if (!(header == XGTCnetHeader.ACK || header == XGTCnetHeader.NAK))
                throw new Exception("ASCII Header 분석 실패 에러");
            if (tail != XGTCnetHeader.ETX)
                throw new Exception("ASCII Tail 분석 실패 에러");
            XGTCnetCommand cmd = (XGTCnetCommand)ascii[3];
            ushort localport = XGTCnetTranslator.InfoDataToLocalPort(new byte[] { ascii[1], ascii[2] });
            XGTCnetProtocol cnet = new XGTCnetProtocol(localport, cmd);
            cnet.Header = header;
            cnet.Tail = tail;

            if (cnet.Header == XGTCnetHeader.NAK)
            {
                var error_bytes = new byte[] { ascii[6], ascii[7], ascii[8], ascii[9] };
                cnet.Error = (XGTCnetError)XGTCnetTranslator.ErrorCodeToInteger(error_bytes);
                return cnet;
            }

            if (cmd == XGTCnetCommand.R)
            {
                cnet.Items = new List<IProtocolData>();
                int count = XGTCnetTranslator.InfoDataToUInt16(new byte[] { ascii[6], ascii[7] });
                int idx = 8;
                for (int i = 0; i < count; i++)
                {
                    ushort size = XGTCnetTranslator.InfoDataToUInt16(new byte[] { ascii[idx], ascii[idx + 1] });
                    idx += 2;
                    byte[] code = new byte[size * 2];
                    Buffer.BlockCopy(ascii, idx, code, 0, code.Length);
                    idx += code.Length;
                    object value = ConvertAutomatically(code);
                    cnet.Items.Add(new ProtocolData(value));
                }
            }
            return cnet;
        }

        public virtual object ConvertAutomatically(byte[] code)
        {
            object value = null;
            switch (code.Length)
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
