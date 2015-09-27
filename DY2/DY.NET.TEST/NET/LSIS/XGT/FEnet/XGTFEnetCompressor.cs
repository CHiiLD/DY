using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGTFEnetProtocol를 LSIS-XGT PLC에 보낼 FEnet Protocol-ASCII구조로 압축하고
    /// LSIS-XGT PLC에서 받은 FEnet Protocol-ASCII 데이터를 XGTFEnetProtocol로 해석한다.
    /// </summary>
    public class XGTFEnetCompressor : IProtocolCompressor
    {
        /// <summary>
        /// XGTFEnetProtocol를 LSIS-XGT PLC에 보낼 FEnet Protocol-ASCII구조로 압축하여 반환한다.
        /// </summary>
        /// <param name="protocol">XGTCnetProtocol</param>
        /// <returns>FEnet Protocol-ASCII</returns>
        public virtual byte[] Encode(IProtocol protocol)
        {
            const int HEADER_SIZE = 20;
            XGTFEnetProtocol fenet = protocol as XGTFEnetProtocol;
            if (!(fenet.Command == XGTFEnetCommand.READ_REQT || fenet.Command == XGTFEnetCommand.WRITE_REQT))
                throw new ArgumentException("Response command not supported.");
            List<byte> buf = new List<byte>();
            fenet.CompanyID = XGTFEnetCompanyID.LSIS_XGT;
            fenet.StreamDirection = XGTFEnetStreamDirection.PC2PLC;

            buf.AddRange(fenet.Command.ToBytes().Reverse());
            buf.AddRange(fenet.DataType.ToBytes().Reverse());
            buf.AddRange(new byte[] { 0x00, 0x00 }); //reserved
            buf.AddRange(XGTFEnetTranslator.ToASCII(fenet.Items.Count, typeof(ushort)).Reverse()); //블록 수

            foreach (var item in fenet.Items)
            {
                buf.AddRange(XGTFEnetTranslator.ToASCII(item.Address.Length, typeof(ushort)).Reverse()); //addr str length
                buf.AddRange(XGTFEnetTranslator.ToASCII(item.Address)); //addr str
                if (fenet.Command == XGTFEnetCommand.WRITE_REQT)
                {
                    buf.AddRange(fenet.DataType.ToBytes().Reverse()); //data type
                    byte[] value = XGTFEnetTranslator.ToASCII(item.Value);
                    if (fenet.DataType == XGTFEnetDataType.WORD)
                        Array.Reverse(value);
                    buf.AddRange(value); //value
                }
            }

            byte[] header_buf = new byte[HEADER_SIZE];
            byte[] company_name = fenet.CompanyID.ToBytes();
            byte direction = fenet.StreamDirection.ToByte();
            byte[] invoke = XGTFEnetTranslator.ToASCII(fenet.InvokeID).Reverse().ToArray();
            byte[] length = XGTFEnetTranslator.ToASCII(buf.Sum(x => x), typeof(ushort)).Reverse().ToArray();
            Buffer.BlockCopy(company_name, 0, header_buf, 0, company_name.Length);
            Buffer.SetByte(header_buf, 13, direction);
            Buffer.BlockCopy(invoke, 0, header_buf, 14, invoke.Length);
            Buffer.BlockCopy(length, 0, header_buf, 16, length.Length);

            buf.InsertRange(0, header_buf);
            return buf.ToArray();
        }

        /// <summary>
        /// LSIS-XGT PLC에서 받은 FEnet Protocol-ASCII 데이터를 XGTCnetProtocol로 해석하여 반환한다.
        /// </summary>
        /// <param name="ascii">FEnet Protocol-ASCII</param>
        /// <returns>XGTCnetProtocol</returns>
        public virtual IProtocol Decode(byte[] ascii)
        {
            XGTFEnetCommand command = (XGTFEnetCommand)XGTFEnetTranslator.ToValue(new byte[] { ascii[21], ascii[20] }, typeof(ushort));
            if (!(command == XGTFEnetCommand.READ_RESP || command == XGTFEnetCommand.WRITE_RESP))
                throw new ArgumentException("Request command not supported.");
            XGTFEnetDataType datatype = (XGTFEnetDataType)XGTFEnetTranslator.ToValue(new byte[] { ascii[23], ascii[22] }, typeof(ushort));
            XGTFEnetProtocol fenet = new XGTFEnetProtocol(command, datatype);
            fenet.CompanyID = XGTFEnetCompanyID.LSIS_XGT.ToBytes().SequenceEqual(
                new byte[] { ascii[0], ascii[1], ascii[2], ascii[3], ascii[4], ascii[5], ascii[6], ascii[7] }) ?
                XGTFEnetCompanyID.LSIS_XGT : XGTFEnetCompanyID.NONE;
            fenet.CpuType = (XGTFEnetCpuType)(0xFC & ascii[10]);
            fenet.Class = (XGTFEnetClass)(0x02 & ascii[10]);
            fenet.CpuState = (XGTFEnetCpuState)(0x01 & ascii[10]);
            fenet.PLCState = (XGTFEnetPLCSystemState)(0x1F & ascii[11]);
            fenet.CpuInfo = (XGTFEnetCpuInfo)ascii[12];
            fenet.StreamDirection = (XGTFEnetStreamDirection)ascii[13];
            fenet.InvokeID = (ushort)XGTFEnetTranslator.ToValue(new byte[] { ascii[15], ascii[14] }, typeof(ushort));
            fenet.ByteSum = (ushort)XGTFEnetTranslator.ToValue(new byte[] { ascii[17], ascii[16] }, typeof(ushort));
            fenet.BasePosition = (byte)(ascii[18] >> 4);
            fenet.SlotPosition = (byte)((byte)0x0F & ascii[18]);

            if (ascii[26] == 0xFF || ascii[27] == 0xFF)
            {
                fenet.Error = (XGTFEnetError)XGTFEnetTranslator.ToValue(new byte[] { ascii[29], ascii[28] }, typeof(ushort));
                return fenet;
            }

            if (fenet.Command == XGTFEnetCommand.READ_RESP)
            {
                if (fenet.Items == null) fenet.Items = new List<IProtocolData>(); else fenet.Items.Clear();
                ushort count = (ushort)XGTFEnetTranslator.ToValue(new byte[] { ascii[27], ascii[26] }, typeof(ushort));
                int idx = 28;
                for (int i = 0; i < count; i++)
                {
                    ushort size = (ushort)XGTFEnetTranslator.ToValue(new byte[] { ascii[idx + 1], ascii[idx] }, typeof(ushort));
                    idx += 2;
                    byte[] code = new byte[size];
                    Buffer.BlockCopy(ascii, idx, code, 0, size);
                    if (fenet.DataType == XGTFEnetDataType.WORD)
                        Array.Reverse(code);
                    fenet.Items.Add(new ProtocolData(ConvertAutomatically(code)));
                    idx += size;
                }
            }
            return fenet;
        }

        /// <summary>
        /// PLC Address에 해당되는 정수byte[]정보를 사용하여 적절한 자료형의 변수로 해석하여 반환한다.
        /// </summary>
        /// <param name="code">정수byte[]정보</param>
        /// <returns>정수</returns>
        public virtual object ConvertAutomatically(byte[] code)
        {
            object value = null;
            switch (code.Length)
            {
                case 1:
                    value = XGTFEnetTranslator.ToValue(code, typeof(byte));
                    break;
                case 2:
                    value = XGTFEnetTranslator.ToValue(code, typeof(ushort));
                    break;
                case 4:
                    value = XGTFEnetTranslator.ToValue(code, typeof(uint));
                    break;
                case 8:
                    value = XGTFEnetTranslator.ToValue(code, typeof(ulong));
                    break;
            }
            return value;
        }
    }
}
