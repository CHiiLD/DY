using System;
using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.TEST
{
    [TestFixture]
    public class XGTCnetProtocolTest
    {
        [Test]
        public void WhenInitialize_ExpectPropertyIsNone()
        {
            XGTCnetProtocol cnet = new XGTCnetProtocol(null, XGTCnetCommand.READ);
            cnet.Initialize();
            Assert.AreEqual(cnet.GetErrorCode(), 0);
            Assert.AreEqual(cnet.Header, ControlChar.NONE);
            Assert.AreEqual(cnet.Tail, ControlChar.NONE);
            Assert.AreEqual(cnet.Command, XGTCnetCommand.NONE);
            Assert.AreEqual(cnet.CommandType, XGTCnetCommandType.NONE);
            Assert.AreEqual(cnet.Error, XGTCnetError.OK);
            Assert.AreEqual(cnet.LocalPort, byte.MaxValue);
            Assert.AreEqual(cnet.Data, null);
        }

        [Test]
        public void Constructor()
        {
            XGTCnetCommand cmd = XGTCnetCommand.READ;
            Type type = typeof(ushort);

            XGTCnetProtocol cnet = new XGTCnetProtocol(type, cmd);

            Assert.AreEqual(type, typeof(ushort));
            Assert.AreEqual(cnet.Command, cmd);
        }
    }
}
