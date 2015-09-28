
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

        Dictionary<object, byte[]> TestItem = new Dictionary<object, byte[]>()
        {
            { true, new byte[] {0x01} },
            { (sbyte)0x12, new byte[] {0x12} },
            { (byte)0xFE, new byte[] {0xFE} },
            { (short)0x1234, new byte[] {0x34, 0x12, } },
            { (ushort)0xFEDC, new byte[] {0xDC, 0xFE, } },
            { 0x12345678, new byte[] {0x12, 0x34, 0x56, 0x78} },
            { 0xFEDCBA09, new byte[] {0xFE, 0xDC, 0xBA, 0x09} },
            { 0x1234567890ABCDEFL, new byte[] {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF} },
            { 0xFEDCBA0987654321L, new byte[] {0xFE, 0xDC, 0xBA, 0x09, 0x87, 0x65, 0x43, 0x21} },
        };

        //버그인 듯?
        [Test]
        [Ignore]
        public void WhenConvertValueToASCII_ConvertSuccessfully()
        {
            foreach(var item in TestItem)
                Assert.AreEqual(XGTFEnetTranslator.ToASCII(item.Key), item.Value);
        }

        [Test]
        public void WhenConvertASCIIToValueData_ConvertSuccessfully()
        {
            foreach (var item in TestItem)
                Assert.AreEqual(XGTFEnetTranslator.ToValue(item.Value, item.Key.GetType()), item.Key);
        }
    }
}