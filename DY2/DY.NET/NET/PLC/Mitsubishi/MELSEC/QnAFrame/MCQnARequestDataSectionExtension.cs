using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public static class MCQnARequestDataSectionExtension
    {
        /// <summary>
        /// QnA호환 3E/3C/4C 프레임에서 READ할 경우의 캐릭터A부 byte[]을 반환한다.
        /// </summary>
        public static byte[] EncodeCharacterSectionA(this IMCQnARequestDataSection protocol, MCProtocolFormat format)
        {
            List<byte> buf = new List<byte>();
            MCDevice dcode = GetDeviceCode(protocol);
            uint lead = protocol.GetLeadDevice();
            ushort len = protocol.GetDevicePoint();
            switch (format)
            {
                case MCProtocolFormat.ASCII:
                    buf.AddRange(dcode.GetDeviceASCIICode()); //2
                    buf.AddRange(StringFormatTranslator.StringToByteArray(lead.ToString("D6"))); //6
                    buf.AddRange(ASCIIFormatTranslator.IntegerToHexASCII<ushort>(len)); //4
                    break;
                case MCProtocolFormat.BINARY:
                    IEnumerable<byte> lead_bin = BinaryFormatTranslator.IntegerToBinary<uint>(lead).Reverse();
                    buf.AddRange(lead_bin);
                    buf.RemoveAt(buf.Count - 1); //3
                    buf.Add(dcode.GetDeviceBinaryCode()); //1
                    buf.AddRange(BinaryFormatTranslator.IntegerToBinary<ushort>(len).Reverse()); //2
                    break;
                default:
                    throw new ArgumentException();
            }
            return buf.ToArray();
        }

        /// <summary>
        /// QnA호환 3E/3C/4C 프레임에서 READ요청 후 응답 받은 SectionB를 분석한다.
        /// </summary>
        public static void DecodeCharacterSectionB(this IMCQnARequestDataSection protocol, byte[] sectionB, MCProtocolFormat format)
        {
            if (format == MCProtocolFormat.NONE)
                throw new ArgumentException();
            switch (format)
            {
                case MCProtocolFormat.ASCII:
                    protocol.DecodeCharacterSectionBByASCII(sectionB);
                    break;
                case MCProtocolFormat.BINARY:
                    protocol.DecodeCharacterSectionBByBinary(sectionB);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public static void DecodeCharacterSectionBByASCII(this IMCQnARequestDataSection protocol, byte[] sectionB)
        {
            const int DATA_LEN_ASCII = 4;
            if (protocol.Data == null)
                protocol.Data = new List<IProtocolData>();
            IList<IProtocolData> dataList = protocol.Data;
            dataList.Clear();
            MCQnADataType mcQnADataType = (MCQnADataType)protocol.DataType;
            switch (mcQnADataType)
            {
                case MCQnADataType.BIT:
                    foreach (byte b in sectionB)
                        dataList.Add(new ProtocolData(b == 0x30 ? false : true));
                    break;
                case MCQnADataType.WORD:
                    for (int i = 0; i < sectionB.Length; i += DATA_LEN_ASCII)
                    {
                        byte[] data = new byte[DATA_LEN_ASCII];
                        Buffer.BlockCopy(sectionB, i, data, 0, data.Length);
                        dataList.Add(new ProtocolData(ASCIIFormatTranslator.HexASCIIToInteger<ushort>(data)));
                    }
                    break;
            }
        }

        public static void DecodeCharacterSectionBByBinary(this IMCQnARequestDataSection protocol, byte[] sectionB)
        {
            const int DATA_LEN_BINARY = 2;
            if (protocol.Data == null)
                protocol.Data = new List<IProtocolData>();
            IList<IProtocolData> dataList = protocol.Data;
            dataList.Clear();
            MCQnADataType mcQnADataType = (MCQnADataType)protocol.DataType;
            switch (mcQnADataType)
            {
                case MCQnADataType.BIT:
                    foreach (byte b in sectionB)
                    {
                        dataList.Add(new ProtocolData((b & (byte)0x10) == 0 ? false : true));
                        dataList.Add(new ProtocolData((b & (byte)0x01) == 0 ? false : true));
                    }
                    break;
                case MCQnADataType.WORD:
                    for (int i = 0; i < sectionB.Length; i += DATA_LEN_BINARY)
                    {
                        byte[] data = new byte[DATA_LEN_BINARY];
                        Buffer.BlockCopy(sectionB, i, data, 0, data.Length);
                        Array.Reverse(data);
                        dataList.Add(new ProtocolData(BinaryFormatTranslator.BinaryToInteger<ushort>(data)));
                    }
                    break;
            }
        }

        /// <summary>
        /// QnA호환 3E/3C/4C 프레임에서 WRITE할 경우의 캐릭터C부 byte[]을 반환한다.
        /// </summary>
        public static byte[] EncodeCharacterSectionC(this IMCQnARequestDataSection protocol, MCProtocolFormat format)
        {
            if (protocol.DataType == null || format == MCProtocolFormat.NONE)
                throw new ArgumentException();
            byte[] result = null;
            switch (format)
            {
                case MCProtocolFormat.ASCII:
                    result = protocol.EncodeCharacterSectionCByASCII();
                    break;
                case MCProtocolFormat.BINARY:
                    result = protocol.EncodeCharacterSectionCByBinary();
                    break;
                default:
                    throw new ArgumentException();
            }
            return result;
        }

        public static byte[] EncodeCharacterSectionCByASCII(this IMCQnARequestDataSection protocol)
        {
            if (protocol.DataType == null)
                throw new ArgumentException();
            List<byte> result = EncodeCharacterSectionA(protocol, MCProtocolFormat.ASCII).ToList();
            IList<IProtocolData> dataList = protocol.Data;
            MCQnADataType mcQnADataType = (MCQnADataType)protocol.DataType;
            for (int i = 0; i < dataList.Count; i++)
            {
                object value = dataList[i];
                switch (mcQnADataType)
                {
                    case MCQnADataType.BIT:
                        result.Add((bool)value == false ? (byte)0x30 : (byte)0x31); //1
                        break;
                    case MCQnADataType.WORD:
                        result.AddRange(ASCIIFormatTranslator.IntegerToHexASCII<ushort>(value)); //4
                        break;
                }
            }
            return result.ToArray();
        }

        public static byte[] EncodeCharacterSectionCByBinary(this IMCQnARequestDataSection protocol)
        {
            if (protocol.DataType == null)
                throw new ArgumentException();
            List<byte> result = EncodeCharacterSectionA(protocol, MCProtocolFormat.BINARY).ToList();
            IList<IProtocolData> dataList = protocol.Data;
            MCQnADataType mcQnADataType = (MCQnADataType)protocol.DataType;
            byte temp = 0;
            for (int i = 0; i < dataList.Count; i++)
            {
                object value = dataList[i];
                switch (mcQnADataType)
                {
                    case MCQnADataType.BIT:
                        if ((bool)value)
                            temp = (i % 2 == 0 ? (byte)(temp & (byte)0x10) : (byte)(temp & (byte)0x01));
                        if (i % 2 == 1 || i == dataList.Count - 1)
                        {
                            result.Add(temp);
                            temp = 0;           
                        }
                        break;
                    case MCQnADataType.WORD:
                        result.AddRange(BinaryFormatTranslator.IntegerToBinary<ushort>(value).Reverse()); //2LH
                        break;
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 디바이스 코드
        /// </summary>
        public static MCDevice GetDeviceCode(this IMCQnARequestDataSection protocol)
        {
            IProtocolData iProtocolData = protocol.Data.FirstOrDefault();
            if (iProtocolData == null)
                throw new ArgumentException();
            byte[] code = new byte[2];
            int idx = 0;
            foreach (char c in iProtocolData.Address)
            {
                if (Char.IsNumber(c))
                    break;
                else
                    code[idx++] = (byte)c;
            }
            if (idx == 1)
                code[1] = (byte)'*';
            return MCDeviceCodeExtension.GetMCDeviceByASCII(code);
        }

        public static IMCQnARequestDataSection SetData4Read(this IMCQnARequestDataSection protocol, string firstAddress, int count)
        {
            if (protocol.Data == null)
                protocol.Data = new List<IProtocolData>();
            IList<IProtocolData> data = protocol.Data;
            data.Clear();
            data.Add(new ProtocolData(firstAddress));
            MCDevice mcDevice = protocol.GetDeviceCode();
            DeviceSystem deviceSystem = mcDevice.GetDeviceSystem();
            uint lead = protocol.GetLeadDevice();
            switch (deviceSystem)
            {
                case DeviceSystem.HEX:
                    for (int i = 1; i < count; i++)
                        data.Add(new ProtocolData((lead + i).ToString("X6")));
                    break;
                case DeviceSystem.DEC:
                    for (int i = 1; i < count; i++)
                        data.Add(new ProtocolData((lead + i).ToString("D6")));
                    break;
            }
            return protocol;
        }

        public static IMCQnARequestDataSection SetData4Write(this IMCQnARequestDataSection protocol, string firstAddress, params object[] args)
        {
            int idx = 0;
            foreach (var data in protocol.SetData4Read(firstAddress, args.Length).Data)
                data.Value = args[idx++];
            return protocol;
        }

        /// <summary>
        /// 선두 디바이스
        /// </summary>
        public static uint GetLeadDevice(this IMCQnARequestDataSection protocol)
        {
            IProtocolData iProtocolData = protocol.Data.FirstOrDefault();
            if (iProtocolData == null)
                throw new ArgumentException();
            StringBuilder sb = new StringBuilder();
            foreach (char c in iProtocolData.Address.Reverse())
            {
                if (Char.IsNumber(c))
                    sb.Append(c);
            }
            return UInt32.Parse(sb.ToString());
        }

        /// <summary>
        /// 디바이스 점수
        /// </summary>
        public static ushort GetDevicePoint(this IMCQnARequestDataSection protocol)
        {
            return (ushort)protocol.Data.Count;
        }
    }
}