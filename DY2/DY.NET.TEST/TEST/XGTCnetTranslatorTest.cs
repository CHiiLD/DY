
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

        private bool m_Boolean = true;
        private sbyte m_Int8 = 0x12;
        private byte m_Uint8 = 0xFE;
        private short m_Int16 = 0x1234;
        private ushort m_Uint16 = 0xFEDC;
        private int m_Int32 = 0x12345678;
        private uint m_Uint32 = 0xFEDCBA09;
        private long m_Int64 = 0x1234567890ABCDEF;
        private ulong m_Uint64 = 0xFEDCBA0987654321;

        private byte[] m_BoolASCII = new byte[] { (byte)'0', (byte)'1' };
        private byte[] m_Int8ASCII = new byte[] { (byte)'1', (byte)'2' };
        private byte[] m_Uint8ASCII = new byte[] { (byte)'F', (byte)'E' };
        private byte[] m_Int16ASCII = new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4' };
        private byte[] m_Uint16ASCII = new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C' };
        private byte[] m_Int32ASCII = new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8' };
        private byte[] m_Uint32ASCII = new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A', (byte)'0', (byte)'9' };
        private byte[] m_Int64ASCII = new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'0', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F' };
        private byte[] m_Uint64ASCII = new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A', (byte)'0', (byte)'9', (byte)'8', (byte)'7', (byte)'6', (byte)'5', (byte)'4', (byte)'3', (byte)'2', (byte)'1' };

        [Test]
        public void WhenConvertValueToASCII_ConvertSuccessfully()
        {
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(m_Boolean), m_BoolASCII);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(m_Int8), m_Int8ASCII);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(m_Uint8), m_Uint8ASCII);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(m_Int16), m_Int16ASCII);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(m_Uint16), m_Uint16ASCII);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(m_Int32), m_Int32ASCII);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(m_Uint32), m_Uint32ASCII);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(m_Int64), m_Int64ASCII);
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(m_Uint64), m_Uint64ASCII);
        }

        [Test]
        public void WhenConvertASCIIToValueData_ConvertSuccessfully()
        {
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(m_BoolASCII, typeof(bool)), m_Boolean);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(m_Int8ASCII, typeof(sbyte)), m_Int8);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(m_Uint8ASCII, typeof(byte)), m_Uint8);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(m_Int16ASCII, typeof(short)), m_Int16);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(m_Uint16ASCII, typeof(ushort)), m_Uint16);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(m_Int32ASCII, typeof(int)), m_Int32);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(m_Uint32ASCII, typeof(uint)), m_Uint32);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(m_Int64ASCII, typeof(long)), m_Int64);
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(m_Uint64ASCII, typeof(ulong)), m_Uint64);
        }
    }
}
