using System;
using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.Test
{
    [TestFixture]
    public class XGTCnetCompressorTest
    {
        [Test]
        public void WSS2ASCII()
        {
            byte[] expectResult = new byte[] { 0x05, 0x32, 0x30, 0x57, 0x53, 0x53, 0x30, 0x31, 0x30, 0x36, 0x25, 0x4D, 0x57, 0x31, 0x30, 0x30, 0x30, 0x30, 0x45, 0x32, 0x04 };
            byte localport = 20;
            var cmd = XGTCnetCommand.W;
            string addr = "%MW100";
            ushort value = 0x00E2;
            XGTCnetProtocol cnet = new XGTCnetProtocol(value.GetType(), cmd) { LocalPort = localport };
            cnet.Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr, value) };

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(cnet);

            Assert.AreEqual(ascii_code, expectResult);
        }

        [Test]
        public void RSS2ASCII()
        {
            byte[] expectResult = new byte[] { 0x05, 0x30, 0x30, 0x52, 0x53, 0x53, 0x30, 0x31, 0x30, 0x36, 0x25, 0x4D, 0x57, 0x31, 0x30, 0x30, 0x04 };
            var cmd = XGTCnetCommand.R;
            string addr = "%MW100";
            XGTCnetProtocol cnet = new XGTCnetProtocol(typeof(ushort), cmd) { LocalPort = 0 };
            cnet.Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr) };

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(cnet);

            Assert.AreEqual(ascii_code, expectResult);
        }

        [Test]
        public void ASCII2WSS()
        {
            byte[] ack_code = new byte[] { 0x06, 0x30, 0x30, 0x57, 0x53, 0x53, 0x03 };
            ushort localport = 00;

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            XGTCnetProtocol cnet = cnet_comp.Decode(ack_code, null) as XGTCnetProtocol;

            Assert.AreEqual(cnet.Header, ControlChar.ACK);
            Assert.AreEqual(cnet.Tail, ControlChar.ETX);
            Assert.AreEqual(cnet.LocalPort, localport);
            Assert.AreEqual(cnet.Command, XGTCnetCommand.W);
            Assert.AreEqual(cnet.CommandType, XGTCnetCommandType.SS);
        }

        [Test]
        public void ASCII2RSS()
        {
            byte[] ack_code = new byte[] { 0x06, 0x30, 0x30, 0x52, 0x53, 0x53, 0x30, 0x31, 0x30, 0x32, 0x41, 0x39, 0x46, 0x33, 0x03 };
            byte localport = 00;

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            XGTCnetProtocol cnet = cnet_comp.Decode(ack_code, typeof(ushort)) as XGTCnetProtocol;

            Assert.AreEqual(cnet.Header, ControlChar.ACK);
            Assert.AreEqual(cnet.Tail, ControlChar.ETX);
            Assert.AreEqual(cnet.LocalPort, localport);
            Assert.AreEqual(cnet.Command, XGTCnetCommand.R);
            Assert.AreEqual(cnet.CommandType, XGTCnetCommandType.SS);
            Assert.AreEqual(cnet.Data.Count, 1);
            Assert.AreEqual(cnet.Data[0].Value, 0xA9F3);
        }

        [Test]
        public void ASCII2Nak()
        {
            byte[] nak_code = new byte[] { 0x15, 0x30, 0x30, 0x57, 0x53, 0x53, 0x31, 0x31, 0x33, 0x32, 0x03 };
            ushort localport = 00;

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor() ;
            XGTCnetProtocol cnet = cnet_comp.Decode(nak_code, null) as XGTCnetProtocol;

            Assert.AreEqual(cnet.Header, ControlChar.NAK);
            Assert.AreEqual(cnet.Tail, ControlChar.ETX);
            Assert.AreEqual(cnet.LocalPort, localport);
            Assert.AreEqual(cnet.Command, XGTCnetCommand.W);
            Assert.AreEqual(cnet.CommandType, XGTCnetCommandType.SS);
            Assert.AreEqual(cnet.Error, XGTCnetError.DEVICE_MEMORY);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        [TestCase(new byte[] { 0x32, 0x30, 0x52, 0x53, 0x53, 0x30, 0x31, 0x30, 0x32, 0x41, 0x39, 0x46, 0x33, 0x03 })]
        [TestCase(new byte[] { 0x06, 0x32, 0x30, 0x52, 0x53, 0x53, 0x30, 0x31, 0x30, 0x32, 0x41, 0x39, 0x46, 0x33 })]
        public void WhenDecodeInvalidASCII_ExpectException(byte[] ack_code)
        {
            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            XGTCnetProtocol cnet = cnet_comp.Decode(ack_code, null) as XGTCnetProtocol;
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void WhenProtocolDataAddItemToo_ExpectException()
        {
            ushort localport = 20;
            var cmd = XGTCnetCommand.W;
            string addr = "%MW100";
            ushort value = 0x00E2;
            XGTCnetProtocol cnet = new XGTCnetProtocol(value.GetType(), cmd);
            cnet.Data = new System.Collections.Generic.List<IProtocolData>();
            for (int i = 0; i < 17; i++)
                cnet.Data.Add(new ProtocolData(addr, value));

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(cnet);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void WhenAddressLengthLongToo_ExpectException()
        {
            ushort localport = 20;
            var cmd = XGTCnetCommand.W;
            string addr = "%MW45678921311023";
            ushort value = 0x00E2;
            XGTCnetProtocol cnet = new XGTCnetProtocol(value.GetType(), cmd);
            cnet.Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr, value) };

            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(cnet);
        }
    }
}
