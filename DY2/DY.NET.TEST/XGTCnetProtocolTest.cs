using System;
using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.TEST
{
    [TestFixture]
    public class XGTCnetProtocolTest
    {
        [Test]
        public void WhenProtocolInitialized_PropertyIsNone()
        {
            XGTCnetProtocol cnet = new XGTCnetProtocol();
            cnet.Command = XGTCnetCommand.R;

            cnet.Initialize();

            InitSuccessTest(cnet);
        }

        [Test]
        public void WhenProtocolCreated_ConstructorCallInitializeMethod()
        {
            XGTCnetProtocol cnet = new XGTCnetProtocol();

            InitSuccessTest(cnet);
        }
        [Test]
        public void WhenProtocolInitialized_ErrorCodeReturnZero()
        {
            XGTCnetProtocol cnet = new XGTCnetProtocol();

            cnet.Initialize();

            Assert.AreEqual(cnet.GetErrorCode(), 0);
        }

        [Test]
        public void WhenProtocolCreated_SubstitudeArgsToProperty()
        {
            ushort local = 12;
            XGTCnetCommand cmd = XGTCnetCommand.R;

            XGTCnetProtocol cnet = new XGTCnetProtocol(local, cmd);

            Assert.AreEqual(cnet.LocalPort, local);
            Assert.AreEqual(cnet.Command, cmd);
        }

        public void InitSuccessTest(XGTCnetProtocol cnet)
        {
            Assert.AreEqual(cnet.Header, XGTCnetHeader.NONE);
            Assert.AreEqual(cnet.Tail, XGTCnetHeader.NONE);
            Assert.AreEqual(cnet.Command, XGTCnetCommand.NONE);
            Assert.AreEqual(cnet.CommandType, XGTCnetCommandType.NONE);
            Assert.AreEqual(cnet.Error, XGTCnetError.OK);
            Assert.AreEqual(cnet.LocalPort, ushort.MaxValue);
            Assert.AreEqual(cnet.Items, null);
        }

        [Test]
        public void WhenConvertUInt16ToInfoData_ReturnTwoByte()
        {
            ushort value = 0x12;
            byte[] expectResult = new byte[] { (byte)'1', (byte)'2' };

            byte[] result = XGTCnetTranslator.UInt16ToInfoData(value);

            Assert.AreEqual(result, expectResult);
        }

        [Test]
        public void WhenConvertInfoDataToUInt16_ReturnUInt16()
        {
            byte[] bytes = new byte[] { (byte)'1', (byte)'2' };
            ushort aim = 0x12;

            ushort value = XGTCnetTranslator.InfoDataToUInt16(bytes);

            Assert.AreEqual(value, aim);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenConvertValueDataToASCII_NotSupportedArgumentType()
        {
            string addr = "%WM0012";
            XGTCnetTranslator.ValueDataToASCII(addr);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void WhenConvertValueToASCII_ExpectNullReferenceException()
        {
            XGTCnetTranslator.ValueDataToASCII(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenConvertValueToASCII_ExpectArgumentNullException()
        {
            XGTCnetTranslator.ValueDataToASCII(null, typeof(int));
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void WhenConvertASCIIToValueData_ExpectNullReferenceException()
        {
            XGTCnetTranslator.ASCIIToValueData(null, typeof(bool));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenConvertASCIIToValueData_NotSupportedArguementType()
        {
            XGTCnetTranslator.ASCIIToValueData(new byte[] { 0x30, 0x31 }, null);
        }

        [Test]
        public void WhenAddressDataToASCII_ConvertSuccessfully()
        {
            string addr = "%MW100";
            byte[] expectResult = new byte[] { (byte)'%', (byte)'M', (byte)'W', (byte)'1', (byte)'0', (byte)'0' };

            byte[] result = XGTCnetTranslator.AddressDataToASCII(addr);

            Assert.AreEqual(result, expectResult);
        }

        [Test]
        public void WhenConvertValueToASCII_ConvertSuccessfully()
        {
            bool boolean = true;
            sbyte int8 = 0x12;
            byte uint8 = 0xFE;
            short int16 = 0x1234;
            ushort uint16 = 0xFEDC;
            int int32 = 0x12345678;
            uint uint32 = 0xFEDCBA09;
            long int64 = 0x1234567890ABCDEF;
            ulong uint64 = 0xFEDCBA0987654321;

            byte[] bool_aim = new byte[] { (byte)'0', (byte)'1' };
            byte[] int8_aim = new byte[] { (byte)'1', (byte)'2' };
            byte[] uint8_aim = new byte[] { (byte)'F', (byte)'E' };
            byte[] int16_aim = new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4' };
            byte[] uint16_aim = new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C' };
            byte[] int32_aim = new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8' };
            byte[] uint32_aim = new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A', (byte)'0', (byte)'9' };
            byte[] int64_aim = new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'0', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F' };
            byte[] uint64_aim = new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A', (byte)'0', (byte)'9', (byte)'8', (byte)'7', (byte)'6', (byte)'5', (byte)'4', (byte)'3', (byte)'2', (byte)'1' };

            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(boolean), bool_aim);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(int8), int8_aim);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(uint8), uint8_aim);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(int16), int16_aim);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(uint16), uint16_aim);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(int32), int32_aim);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(uint32), uint32_aim);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(int64), int64_aim);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(uint64), uint64_aim);

            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(bool_aim, typeof(bool)), boolean);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(int8_aim, typeof(sbyte)), int8);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(uint8_aim, typeof(byte)), uint8);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(int16_aim, typeof(short)), int16);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(uint16_aim, typeof(ushort)), uint16);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(int32_aim, typeof(int)), int32);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(uint32_aim, typeof(uint)), uint32);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(int64_aim, typeof(long)), int64);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(uint64_aim, typeof(ulong)), uint64);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void WhenProtocolEncoded_BlockCountOverError()
        {
            ushort localport = 20;
            var cmd = XGTCnetCommand.W;
            string addr = "%MW100";
            ushort value = 0x00E2;
            XGTCnetProtocol cnet = new XGTCnetProtocol(localport, cmd);
            cnet.Items = new System.Collections.Generic.List<IProtocolData>();
            for (int i = 0; i < 17; i++)
                cnet.Items.Add(new ProtocolData(addr, value));

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(cnet);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void WhenProtocolEncoded_AddressLengthOverError()
        {
            ushort localport = 20;
            var cmd = XGTCnetCommand.W;
            string addr = "%MW4567890123";
            ushort value = 0x00E2;
            XGTCnetProtocol cnet = new XGTCnetProtocol(localport, cmd);
            cnet.Items = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr, value) };

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(cnet);
        }

        [Test]
        public void WhenWSSProtocolEncoded_CompressSuccessfully()
        {
            byte[] expectResult = new byte[] { 0x05, 0x32, 0x30, 0x57, 0x53, 0x53, 0x30, 0x31, 0x30, 0x36, 0x25, 0x4D, 0x57, 0x31, 0x30, 0x30, 0x30, 0x30, 0x45, 0x32, 0x04 };
            ushort localport = 20;
            var cmd = XGTCnetCommand.W;
            string addr = "%MW100";
            ushort value = 0x00E2;
            XGTCnetProtocol cnet = new XGTCnetProtocol(localport, cmd);
            cnet.Items = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr, value) };

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(cnet);

            Assert.AreEqual(ascii_code, expectResult);
        }

        [Test]
        public void WhenRSSProtocolEncoded_CompressSuccessfully()
        {
            byte[] expectResult = new byte[] { 0x05, 0x32, 0x30, 0x52, 0x53, 0x53, 0x30, 0x31, 0x30, 0x36, 0x25, 0x4D, 0x57, 0x31, 0x30, 0x30, 0x04 };
            ushort localport = 20;
            var cmd = XGTCnetCommand.R;
            string addr = "%MW100";
            XGTCnetProtocol cnet = new XGTCnetProtocol(localport, cmd);
            cnet.Items = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr) };

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(cnet);

            Assert.AreEqual(ascii_code, expectResult);
        }

        [Test]
        public void WhenWSSProtocolDecoded_DecompressSuccessfully()
        {
            byte[] ack_code = new byte[] { 0x06, 0x32, 0x30, 0x57, 0x53, 0x53, 0x03 };
            ushort localport = 20;

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            XGTCnetProtocol cnet = cnet_comp.Decode(ack_code) as XGTCnetProtocol;

            Assert.AreEqual(cnet.Header, XGTCnetHeader.ACK);
            Assert.AreEqual(cnet.Tail, XGTCnetHeader.ETX);
            Assert.AreEqual(cnet.LocalPort, localport);
            Assert.AreEqual(cnet.Command, XGTCnetCommand.W);
            Assert.AreEqual(cnet.CommandType, XGTCnetCommandType.SS);
        }

        [Test]
        public void WhenRSSProtocolDecoded_DecompressSuccessfully()
        {
            byte[] ack_code = new byte[] { 0x06, 0x32, 0x30, 0x52, 0x53, 0x53, 0x30, 0x31, 0x30, 0x32, 0x41, 0x39, 0x46, 0x33, 0x03 };
            ushort localport = 20;

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            XGTCnetProtocol cnet = cnet_comp.Decode(ack_code) as XGTCnetProtocol;

            Assert.AreEqual(cnet.Header, XGTCnetHeader.ACK);
            Assert.AreEqual(cnet.Tail, XGTCnetHeader.ETX);
            Assert.AreEqual(cnet.LocalPort, localport);
            Assert.AreEqual(cnet.Command, XGTCnetCommand.R);
            Assert.AreEqual(cnet.CommandType, XGTCnetCommandType.SS);
            Assert.AreEqual(cnet.Items.Count, 1);
            Assert.AreEqual(cnet.Items[0].Value, 0xA9F3);
        }

        [Test]
        public void WhenWSSProtocolDecoded_DecompressNakResponse()
        {
            byte[] nak_code = new byte[] { 0x15, 0x32, 0x30, 0x57, 0x53, 0x53, 0x31, 0x31, 0x33, 0x32, 0x03 };
            ushort localport = 20;

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            XGTCnetProtocol cnet = cnet_comp.Decode(nak_code) as XGTCnetProtocol;

            Assert.AreEqual(cnet.Header, XGTCnetHeader.NAK);
            Assert.AreEqual(cnet.Tail, XGTCnetHeader.ETX);
            Assert.AreEqual(cnet.LocalPort, localport);
            Assert.AreEqual(cnet.Command, XGTCnetCommand.W);
            Assert.AreEqual(cnet.CommandType, XGTCnetCommandType.SS);
            Assert.AreEqual(cnet.Error, XGTCnetError.DEVICE_MEMORY);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void WhenProtocolDecoded_InvalidAckCodeError1()
        {
            //Header 제거
            byte[] ack_code = new byte[] { 0x32, 0x30, 0x52, 0x53, 0x53, 0x30, 0x31, 0x30, 0x32, 0x41, 0x39, 0x46, 0x33, 0x03 };

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();

            XGTCnetProtocol cnet = cnet_comp.Decode(ack_code) as XGTCnetProtocol;
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void WhenProtocolDecoded_InvalidAckCodeError2()
        {
            //tail 제거
            byte[] ack_code = new byte[] { 0x06, 0x32, 0x30, 0x52, 0x53, 0x53, 0x30, 0x31, 0x30, 0x32, 0x41, 0x39, 0x46, 0x33 };

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();

            XGTCnetProtocol cnet = cnet_comp.Decode(ack_code) as XGTCnetProtocol;
        }
    }
}
