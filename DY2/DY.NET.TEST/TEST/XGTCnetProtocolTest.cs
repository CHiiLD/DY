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
