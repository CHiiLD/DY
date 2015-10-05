
using System;
using NUnit.Framework;
using DY.NET.LSIS.XGT;
using System.Collections.Generic;

namespace DY.NET.Test
{
#if false
    [TestFixture]
    public class XGTCnetTranslatorTest
    {
        [Test]
        public void UInt16ToInfoData()
        {
            ushort value = 0x12;
            byte[] expectResult = new byte[] { (byte)'1', (byte)'2' };

            byte[] result = XGTCnetTranslator.BlockDataToASCII(value);

            Assert.AreEqual(result, expectResult);
        }

        [Test]
        public void InfoDataToUInt16()
        {
            byte[] bytes = new byte[] { (byte)'1', (byte)'2' };
            ushort aim = 0x12;

            ushort value = XGTCnetTranslator.ASCIIToBlockData(bytes);

            Assert.AreEqual(value, aim);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenConvertNullToASCII_ExpectArgumentNullException()
        {
            XGTCnetTranslator.ValueDataToASCII(null, typeof(int));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenConvertNullToValueData_ExpectArgumentNullException()
        {
            XGTCnetTranslator.ASCIIToValueData(null, typeof(bool));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [TestCase(typeof(double))]
        [TestCase(typeof(float))]
        public void WhenUseUnsupportType_ExpectArgumentException(Type type)
        {
            XGTCnetTranslator.ASCIIToValueData(new byte[] { 0x30, 0x31 }, type);
        }

        [Test]
        public void Address2ASCII()
        {
            string addr = "%MW100";
            byte[] expectResult = new byte[] { (byte)'%', (byte)'M', (byte)'W', (byte)'1', (byte)'0', (byte)'0' };

            byte[] result = XGTCnetTranslator.AddressDataToASCII(addr);

            Assert.AreEqual(result, expectResult);
        }

        [Test]
        [TestCase(true, new byte[] { (byte)'0', (byte)'1' })]
        [TestCase((sbyte)0x12, new byte[] { (byte)'1', (byte)'2' })]
        [TestCase((byte)0xFE, new byte[] { (byte)'F', (byte)'E' })]
        [TestCase((short)0x1234, new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4' })]
        [TestCase((ushort)0xFEDC, new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C' })]
        [TestCase(0x12345678, new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8' })]
        [TestCase(0xFEDCBA09, new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A', (byte)'0', (byte)'9' })]
        [TestCase(0x1234567890ABCDEFL, new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'0', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F' })]
        [TestCase(0xFEDCBA0987654321L, new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A', (byte)'0', (byte)'9', (byte)'8', (byte)'7', (byte)'6', (byte)'5', (byte)'4', (byte)'3', (byte)'2', (byte)'1' })]
        public void Value2ASCII(object value, byte[] expect)
        {
            Assert.AreEqual(XGTCnetTranslator.ValueDataToASCII(value, value.GetType()), expect);
        }

        [Test]
        [TestCase(true, new byte[] { (byte)'0', (byte)'1' })]
        [TestCase((sbyte)0x12, new byte[] { (byte)'1', (byte)'2' })]
        [TestCase((byte)0xFE, new byte[] { (byte)'F', (byte)'E' })]
        [TestCase((short)0x1234, new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4' })]
        [TestCase((ushort)0xFEDC, new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C' })]
        [TestCase(0x12345678, new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8' })]
        [TestCase(0xFEDCBA09, new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A', (byte)'0', (byte)'9' })]
        [TestCase(0x1234567890ABCDEFL, new byte[] { (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'0', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F' })]
        [TestCase(0xFEDCBA0987654321L, new byte[] { (byte)'F', (byte)'E', (byte)'D', (byte)'C', (byte)'B', (byte)'A', (byte)'0', (byte)'9', (byte)'8', (byte)'7', (byte)'6', (byte)'5', (byte)'4', (byte)'3', (byte)'2', (byte)'1' })]
        public void ASCII2Value(object expect, byte[] value)
        {
            Assert.AreEqual(XGTCnetTranslator.ASCIIToValueData(value, expect.GetType()), expect);
        }

        [Test]
        [TestCase((byte)0xFE, typeof(short), new byte[] { (byte)'0', (byte)'0', (byte)'F', (byte)'E' })]
        [TestCase((ushort)0x00FE, typeof(short), new byte[] { (byte)'0', (byte)'0', (byte)'F', (byte)'E' })]
        [TestCase(0x00FEU, typeof(short), new byte[] { (byte)'0', (byte)'0', (byte)'F', (byte)'E' })]
        [TestCase((long)0x00FE, typeof(short), new byte[] { (byte)'0', (byte)'0', (byte)'F', (byte)'E' })]
        [TestCase((ulong)0x00FE, typeof(short), new byte[] { (byte)'0', (byte)'0', (byte)'F', (byte)'E' })]
        public void WhenMismatchTypeOfValueAndUserType_Convert(object expect, Type type, byte[] value)
        {
            object result = XGTCnetTranslator.ASCIIToValueData(value, type);

            Assert.AreEqual(result, expect);
            Assert.AreEqual(result.GetType(), type);
        }
    }
#endif
}
