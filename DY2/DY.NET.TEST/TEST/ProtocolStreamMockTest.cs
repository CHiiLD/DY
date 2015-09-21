using System;
using System.Threading.Tasks;
using System.IO.Ports;
using NUnit.Framework;
using DY.NET.LSIS.XGT;
using Moq;

namespace DY.NET.TEST
{
    [TestFixture]
    public class IProtocolStreamMockTest
    {
        private Mock<IProtocolStream> m_StreamMock;

        [TestFixtureSetUp]
        public void SetUp()
        {
            m_StreamMock = new Mock<IProtocolStream>();
            m_StreamMock.Setup(m => m.IsOpend()).Returns(true);
        }

        [TestFixtureTearDown]
        public async void TearDown()
        {
            var stream = m_StreamMock.Object;
            await stream.CloseAsync();
        }

        [Test]
        public async void WhenStreamOpend_CanCommunicate()
        {
            var stream = m_StreamMock.Object;
            await stream.OpenAsync();
            Assert.True(stream.IsOpend());
            Assert.DoesNotThrow(async () => { await stream.CloseAsync(); });
        }

        [Test]
        public async void WhenFloatOnStream_SendSuccessfully()
        {
            ushort localport = 20;
            var cmd = XGTCnetCommand.R;
            string addr = "%MW100";
            XGTCnetProtocol resquest = new XGTCnetProtocol(localport, cmd);
            resquest.Items = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr) };
            m_StreamMock.Setup(m => m.SendAsync(resquest)).ReturnsAsync(new XGTCnetProtocol(localport, cmd)
            {
                Header = XGTCnetHeader.ACK,
                Tail = XGTCnetHeader.ETX,
                Items = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(0) }
            });

            XGTCnetProtocol response = await m_StreamMock.Object.SendAsync(resquest) as XGTCnetProtocol;

            Assert.AreEqual(response.GetErrorCode(), 0);
            Assert.AreEqual(response.Header, XGTCnetHeader.ACK);
            Assert.AreEqual(response.Tail, XGTCnetHeader.ETX);
            Assert.AreEqual(response.CommandType, XGTCnetCommandType.SS);
            Assert.AreEqual(response.Command, cmd);
            Assert.AreEqual(response.LocalPort, localport);
            Assert.AreEqual(response.Items.Count, resquest.Items.Count);
        }
    }
}
