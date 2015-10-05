using System;
using System.Threading.Tasks;
using System.IO.Ports;
using NUnit.Framework;
using DY.NET.LSIS.XGT;
using Moq;

namespace DY.NET.Test
{
    [Ignore]
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
        public void TearDown()
        {
            var stream = m_StreamMock.Object;
            stream.Close();
        }

        [Test]
        public async void Open()
        {
            var stream = m_StreamMock.Object;
            await stream.OpenAsync();
            Assert.True(stream.IsOpend());
            Assert.DoesNotThrow(() => {  stream.Close(); });
        }

        [Test]
        public async void Sample_InterfaceMethodMock()
        {
            ushort localport = 20;
            var cmd = XGTCnetCommand.R;
            string addr = "%MW100";
            XGTCnetProtocol resquest = new XGTCnetProtocol(typeof(ushort), cmd);
            resquest.Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr) };
            m_StreamMock.Setup(m => m.SendAsync(resquest)).ReturnsAsync(new XGTCnetProtocol(typeof(ushort), cmd)
            {
                Header = ControlChar.ACK,
                Tail = ControlChar.ETX,
                Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(0) }
            });

            XGTCnetProtocol response = await m_StreamMock.Object.SendAsync(resquest) as XGTCnetProtocol;

            Assert.AreEqual(response.GetErrorCode(), 0);
            Assert.AreEqual(response.Header, ControlChar.ACK);
            Assert.AreEqual(response.Tail, ControlChar.ETX);
            Assert.AreEqual(response.CommandType, XGTCnetCommandType.SS);
            Assert.AreEqual(response.Command, cmd);
            Assert.AreEqual(response.LocalPort, localport);
            Assert.AreEqual(response.Data.Count, resquest.Data.Count);
        }
    }
}
