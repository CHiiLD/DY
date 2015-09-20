using System;
using System.Threading.Tasks;
using System.IO.Ports;
using NUnit.Framework;
using DY.NET.LSIS.XGT;
using Moq;

namespace DY.NET.TEST
{
    [TestFixture]
    public class XGTCnetStreamTest
    {
        [Test]
        public async void WhenFloatOnProtocolStream_SendSuccessfully()
        {
            //Mock<IProtocolStream> mock_stream = new Mock<IProtocolStream>();
            //mock_stream.Setup(m => m.OpenAsync()).Returns(new Task<bool>(() => { return true; }));
            //mock_stream.Setup(m => m.CanCommunicate()).Returns(new Task<bool>(() => { return true; }));
            //ushort localport = 20;
            //var cmd = XGTCnetCommand.W;
            //var addr = "%MW100";
            //ushort value = 0;
            //XGTCnetProtocol cnet = new XGTCnetProtocol(localport, cmd) { Items = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr, value) } };
            //mock_stream.Setup(m => m.SendAsync(cnet)).Returns(new Task<IProtocol>(() => 
            //{
            //    XGTCnetProtocol r = new XGTCnetProtocol(20, XGTCnetCommand.W);
            //    r.Header = XGTCnetHeader.ACK;
            //    r.Tail = XGTCnetHeader.ETX;
            //    return r;
            //}));
            //Mock<SerialPort> mock_stream = new Mock<SerialPort>();
            //mock_stream.SetupProperty(m => m.PortName, "COM3");
            //mock_stream.SetupProperty(m => m.IsOpen, true);
        }
    }
}
