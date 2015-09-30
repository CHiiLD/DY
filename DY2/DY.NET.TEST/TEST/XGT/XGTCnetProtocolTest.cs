using System;
using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.Test
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

            XGTCnetProtocol cnet = new XGTCnetProtocol(typeof(ushort), local, cmd);

            Assert.AreEqual(cnet.ItemType, typeof(ushort));
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
    }
}
