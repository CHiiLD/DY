
using System;
using System.Linq;
using NUnit.Framework;
using DY.NET.LSIS.XGT;
using System.Collections.Generic;

namespace DY.NET.TEST
{
    [TestFixture]
    public class XGTFEnetTranslatorTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenConvertValueToASCII_ExpectArgumentNullException()
        {
            XGTFEnetTranslator.ToASCII(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenConvertValueToASCII_ExpectArgumentNullException2()
        {
            XGTFEnetTranslator.ToASCII(null, typeof(int));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenConvertASCIIToValueData_ExpectArgumentNullException()
        {
            XGTFEnetTranslator.ToValue(null, typeof(bool));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [TestCase(typeof(double))]
        [TestCase(typeof(float))]
        public void WhenConvertASCIIToValueData_ExpectArgumentException(Type type)
        {
            XGTFEnetTranslator.ToValue(new byte[] { 0x30, 0x31 }, type);
        }

        //버그인 듯?
        [Test]
        [TestCase(true, new byte[] { 0x01 })]
        [TestCase((sbyte)0x12, new byte[] { 0x12 })]
        [TestCase((byte)0xFE, new byte[] { 0xFE })]
        [TestCase((short)0x1234, new byte[] { 0x34, 0x12, })]
        [TestCase((ushort)0xFEDC, new byte[] { 0xDC, 0xFE, })]
        [TestCase(0x12345678, new byte[] { 0x12, 0x34, 0x56, 0x78 })]
        [TestCase(0xFEDCBA09, new byte[] { 0xFE, 0xDC, 0xBA, 0x09 })]
        [TestCase(0x1234567890ABCDEFL, new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF })]
        [TestCase(0xFEDCBA0987654321L, new byte[] { 0xFE, 0xDC, 0xBA, 0x09, 0x87, 0x65, 0x43, 0x21 })]
        public void WhenConvertValueToASCII_ConvertSuccessfully(object value, byte[] expect)
        {
            Assert.AreEqual(XGTFEnetTranslator.ToASCII(value), expect);
        }

        [Test]
        [TestCase(true, new byte[] { 0x01 })]
        [TestCase((sbyte)0x12, new byte[] { 0x12 })]
        [TestCase((byte)0xFE, new byte[] { 0xFE })]
        [TestCase((short)0x1234, new byte[] { 0x34, 0x12, })]
        [TestCase((ushort)0xFEDC, new byte[] { 0xDC, 0xFE, })]
        [TestCase(0x12345678, new byte[] { 0x12, 0x34, 0x56, 0x78 })]
        [TestCase(0xFEDCBA09, new byte[] { 0xFE, 0xDC, 0xBA, 0x09 })]
        [TestCase(0x1234567890ABCDEFL, new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF })]
        [TestCase(0xFEDCBA0987654321L, new byte[] { 0xFE, 0xDC, 0xBA, 0x09, 0x87, 0x65, 0x43, 0x21 })]
        public void WhenConvertASCIIToValueData_ConvertSuccessfully(object expect, byte[] value)
        {
            Assert.AreEqual(XGTFEnetTranslator.ToValue(value, expect.GetType()), expect);
        }

        [Test]
        [TestCase((byte)0xFE, typeof(short), new byte[] { 0xFE, 0x00 })]
        [TestCase((ushort)0x00FE, typeof(short), new byte[] { 0xFE, 0x00 })]
        [TestCase(0x00FEU, typeof(short), new byte[] { 0xFE, 0x00 })]
        [TestCase((long)0x00FE, typeof(short), new byte[] { 0xFE, 0x00 })]
        [TestCase((ulong)0x00FE, typeof(short), new byte[] { 0xFE, 0x00 })]
        public void WhenConvertASCIIToValueData_ConvertSuccessfully2(object expect, Type type, byte[] value)
        {
            object result = XGTFEnetTranslator.ToValue(value, type);
            Assert.AreEqual(result, expect);
            Assert.AreEqual(result.GetType(), type);
        }
    }
}