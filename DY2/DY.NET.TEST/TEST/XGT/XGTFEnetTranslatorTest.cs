
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
        public void WhenConvertNullToASCII_ExpectArgumentNullException()
        {
            XGTFEnetTranslator.ToCode(typeof(int), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenConvertNullToValue_ExpectArgumentNullException()
        {
            XGTFEnetTranslator.ToValue(typeof(bool), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [TestCase(typeof(double))]
        [TestCase(typeof(float))]
        public void WhenUseUnsupportType_ExpectArgumentException(Type type)
        {
            XGTFEnetTranslator.ToValue(type, new byte[] { 0x30, 0x31 });
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
        public void Value2ASCII(object value, byte[] expect)
        {
            Assert.AreEqual(XGTFEnetTranslator.ToCode(value.GetType(), value), expect);
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
        public void ASCII2Value(object expect, byte[] value)
        {
            Assert.AreEqual(XGTFEnetTranslator.ToValue(expect.GetType(), value), expect);
        }

        [Test]
        [TestCase((byte)0xFE, typeof(short), new byte[] { 0xFE, 0x00 })]
        [TestCase((ushort)0x00FE, typeof(short), new byte[] { 0xFE, 0x00 })]
        [TestCase(0x00FEU, typeof(short), new byte[] { 0xFE, 0x00 })]
        [TestCase((long)0x00FE, typeof(short), new byte[] { 0xFE, 0x00 })]
        [TestCase((ulong)0x00FE, typeof(short), new byte[] { 0xFE, 0x00 })]
        public void ASCII2Value(object expect, Type type, byte[] value)
        {
            object result = XGTFEnetTranslator.ToValue(type, value);
            Assert.AreEqual(result, expect);
            Assert.AreEqual(result.GetType(), type);
        }
    }
}