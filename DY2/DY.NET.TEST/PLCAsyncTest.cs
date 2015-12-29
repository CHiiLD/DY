using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DY.NET.TEST
{
    [TestFixture]
    public class PLCAsyncTest
    {
        [Test]
        public async Task Sample()
        {
            var type = ProtocolType.FOR_READ;
            Mock<IPLCAsync> plcMock = new Mock<IPLCAsync>();
            plcMock.Setup(x => x.OpenAsync()).Returns(true);
            var plc = plcMock.Object;

            IProtocolInfo protocol = null;

            plc.OpenTimeout = 1000;
            plc.ReadTimeout = 1000;
            plc.WriteTimeout = 1000;

            bool isOpend = await plc.OpenAsync();
            if (isOpend == true && await plc.CanTalkAsync())
            {
                protocol.ProtocolType = type;
                IProtocolBase resp = await plc.TalkAsync(protocol);
                Assert.AreEqual(0, resp.GetErrorCode());
                Assert.AreEqual(type, resp);
            }
        }
    }
}