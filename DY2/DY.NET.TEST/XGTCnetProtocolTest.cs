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

            TestXGTCnetProtocolInit(cnet);
        }

        [Test]
        public void WhenProtocolCreated_ConstructorCallInitialize()
        {
            XGTCnetProtocol cnet = new XGTCnetProtocol();

            TestXGTCnetProtocolInit(cnet);
        }

        public void TestXGTCnetProtocolInit(XGTCnetProtocol cnet)
        {
            Assert.AreEqual(cnet.Header, XGTCnetHeader.NONE);
            Assert.AreEqual(cnet.Tail, XGTCnetHeader.NONE);
            Assert.AreEqual(cnet.Command, XGTCnetCommand.NONE);
            Assert.AreEqual(cnet.CommandType, XGTCnetCommandType.NONE);
            Assert.AreEqual(cnet.Error, XGTCnetProtocolError.OK);
            Assert.AreEqual(cnet.LocalPort, ushort.MaxValue);
            Assert.AreEqual(cnet.Items, null);
        }

        [Test]
        public void WhenProtocolInitialized_ErrorCodeReturnZero()
        {
            XGTCnetProtocol cnet = new XGTCnetProtocol();

            cnet.Initialize();

            Assert.AreEqual(cnet.GetErrorCode(), 0);
        }

        [Test]
        public void WhenProtocolCreated_ArgsToProperty()
        {
            ushort local = 12;
            XGTCnetCommand cmd = XGTCnetCommand.R;
            XGTCnetCommandType cmd_type = XGTCnetCommandType.SS;

            XGTCnetProtocol cnet = new XGTCnetProtocol(local, cmd, cmd_type);

            Assert.AreEqual(cnet.LocalPort, local);
            Assert.AreEqual(cnet.Command, cmd);
            Assert.AreEqual(cnet.CommandType, cmd_type);
        }

        [Test]
        public void WhenConvertUInt16ToTwoByte_ReturnTwoByte()
        {
            ushort value = 0x12;
            byte[] aim = new byte[] { (byte)'1', (byte)'2' };

            byte[] bytes = XGTCnetASCIITranslator.UInt16ToTwoByte(value);

            Assert.AreEqual(bytes, aim);
        }

        [Test]
        public void WhenConvertTwoByteToUInt16_ReturnUInt16()
        {
            byte[] bytes = new byte[] { (byte)'1', (byte)'2' };
            ushort aim = 0x12;

            ushort value = XGTCnetASCIITranslator.TwoByteToUInt16(bytes);

            Assert.AreEqual(value, aim);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WhenConvertTwoByteToUInt16_ThrowException()
        {
            byte[] bytes = new byte[] { (byte)'1', (byte)'2', (byte)'3' };
            ushort value = XGTCnetASCIITranslator.TwoByteToUInt16(bytes);
        }

        [Test]
        public void WhenConvertValueToASCII_ReturnCorrectValue()
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
            string str = "%WM0012";
            byte[] bool_aim = new byte[] { (byte)'0', (byte)'1' };
            byte[] int8_aim = new byte[] { (byte)'1', (byte)'2' };
            byte[] uint8_aim = new byte[] { (byte)'F', (byte)'E' };
            byte[] int16_aim = new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4' };
            byte[] uint16_aim = new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C' };
            byte[] int32_aim = new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8' };
            byte[] uint32_aim = new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A', (byte)'0', (byte)'9' };
            byte[] int64_aim = new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'0', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F' };
            byte[] uint64_aim = new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A', (byte)'0', (byte)'9', (byte)'8', (byte)'7', (byte)'6', (byte)'5', (byte)'4', (byte)'3', (byte)'2', (byte)'1' };
            byte[] str_aim = new byte[] { (byte)'%', (byte)'W', (byte)'M', (byte)'0', (byte)'0', (byte)'1', (byte)'2' };

            Assert.AreEqual(XGTCnetASCIITranslator.ValueToASCII(boolean), bool_aim);
            Assert.AreEqual(XGTCnetASCIITranslator.ValueToASCII(int8), int8_aim);
            Assert.AreEqual(XGTCnetASCIITranslator.ValueToASCII(uint8), uint8_aim);
            Assert.AreEqual(XGTCnetASCIITranslator.ValueToASCII(int16), int16_aim);
            Assert.AreEqual(XGTCnetASCIITranslator.ValueToASCII(uint16), uint16_aim);
            Assert.AreEqual(XGTCnetASCIITranslator.ValueToASCII(int32), int32_aim);
            Assert.AreEqual(XGTCnetASCIITranslator.ValueToASCII(uint32), uint32_aim);
            Assert.AreEqual(XGTCnetASCIITranslator.ValueToASCII(int64), int64_aim);
            Assert.AreEqual(XGTCnetASCIITranslator.ValueToASCII(uint64), uint64_aim);
            Assert.AreEqual(XGTCnetASCIITranslator.ValueToASCII(str), str_aim);
            Assert.AreEqual(XGTCnetASCIITranslator.ASCIIToValue(bool_aim, typeof(bool)), boolean);
            Assert.AreEqual(XGTCnetASCIITranslator.ASCIIToValue(int8_aim, typeof(sbyte)), int8);
            Assert.AreEqual(XGTCnetASCIITranslator.ASCIIToValue(uint8_aim, typeof(byte)), uint8);
            Assert.AreEqual(XGTCnetASCIITranslator.ASCIIToValue(int16_aim, typeof(short)), int16);
            Assert.AreEqual(XGTCnetASCIITranslator.ASCIIToValue(uint16_aim, typeof(ushort)), uint16);
            Assert.AreEqual(XGTCnetASCIITranslator.ASCIIToValue(int32_aim, typeof(int)), int32);
            Assert.AreEqual(XGTCnetASCIITranslator.ASCIIToValue(uint32_aim, typeof(uint)), uint32);
            Assert.AreEqual(XGTCnetASCIITranslator.ASCIIToValue(int64_aim, typeof(long)), int64);
            Assert.AreEqual(XGTCnetASCIITranslator.ASCIIToValue(uint64_aim, typeof(ulong)), uint64);
            Assert.AreEqual(XGTCnetASCIITranslator.ASCIIToValue(str_aim, typeof(string)), str);
        }

        [Test]
        public void WhenProtocolCompressed_ReturnIsNormal()
        {
            ushort local = 00;
            XGTCnetProtocol cnet = new XGTCnetProtocol(local, XGTCnetCommand.R, XGTCnetCommandType.SS);
            XGTCnetProtocolCompressor cnet_comp = new XGTCnetProtocolCompressor();

            byte[] ascii_code = cnet_comp.Encode(cnet);
            byte[] normal_result = new byte[] { XGTCnetHeader.ENQ.ToByte(), };
        }
    }
}
