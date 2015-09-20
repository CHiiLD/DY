
using System;
using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.TEST
{
    [TestFixture]
    public class XGTCnetTranslatorTest
    {
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
        public void WhenConvertAddressDataToASCII_ConvertSuccessfully()
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
        }

        [Test]
        public void WhenConvertASCIIToValueData_ConvertSuccessfully()
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
    }
}
