using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.TEST
{
    [Ignore]
    [TestFixture]
    public class XGTCnetRealDeviceTest
    {
        [Test]
        public async void StreamCommunicationTest()
        {
            using (var stream = new XGTCnetStream("COM3", 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One))
            {
                await stream.OpenAsync();
                Assert.True(stream.IsOpend());
                Random r = new Random();
                Dictionary<string, object> d = new Dictionary<string, object>() 
                { 
                    { "%MB0000", (byte)r.Next(0, byte.MaxValue) },
                    { "%MW100", (ushort)r.Next(0, ushort.MaxValue) },
                    { "%MD200", (uint) (r.NextDouble() * (double)uint.MaxValue) },
                    { "%ML400", (ulong)(r.NextDouble() * (double)ulong.MaxValue) },
                };
                foreach (var item in d)
                {
                    XGTCnetProtocol write_protocol = new XGTCnetProtocol(0, XGTCnetCommand.W);
                    write_protocol.Items = new List<IProtocolData>() { new ProtocolData(item.Key, item.Value) };
                    XGTCnetProtocol w_response = await stream.SendAsync(write_protocol) as XGTCnetProtocol;

                    Assert.AreEqual(w_response.GetErrorCode(), 0);

                    XGTCnetProtocol read_protocol = new XGTCnetProtocol(0, XGTCnetCommand.R);
                    read_protocol.Items = new List<IProtocolData>() { new ProtocolData(item.Key, item.Value) };
                    XGTCnetProtocol r_response = await stream.SendAsync(read_protocol) as XGTCnetProtocol;

                    Assert.AreEqual(r_response.GetErrorCode(), 0);
                    Assert.AreEqual(r_response.Items[0].Value, item.Value);
                }
            }
        }
    }
}
