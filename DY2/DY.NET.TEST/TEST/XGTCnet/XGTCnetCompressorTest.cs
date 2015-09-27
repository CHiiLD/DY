using System;
using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.TEST
{
    [TestFixture]
    public class XGTCnetCompressorTest
    {
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
        [TestCase(new byte[] { 0x32, 0x30, 0x52, 0x53, 0x53, 0x30, 0x31, 0x30, 0x32, 0x41, 0x39, 0x46, 0x33, 0x03 })]
        [TestCase(new byte[] { 0x06, 0x32, 0x30, 0x52, 0x53, 0x53, 0x30, 0x31, 0x30, 0x32, 0x41, 0x39, 0x46, 0x33 })]
        public void WhenProtocolDecoded_InvalidAckCodeError(byte[] ack_code)
        {
            XGTCnetCompressor cnet_comp = new XGTCnetCompressor();
            XGTCnetProtocol cnet = cnet_comp.Decode(ack_code) as XGTCnetProtocol;
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
    }
}
