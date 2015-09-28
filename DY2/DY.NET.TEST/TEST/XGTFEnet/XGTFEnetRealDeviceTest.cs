using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DY.NET.LSIS.XGT;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace DY.NET.TEST
{
    //[Ignore]
    [TestFixture]
    public class XGTFEnetRealDeviceTest
    {
        [Test]
        public async void StreamCommunicationTest()
        {
            using (var stream = new XGTFEnetStream("192.168.10.150"))
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
                    var write_protocol = new XGTFEnetProtocol(XGTFEnetCommand.WRITE_REQT, item.Value.GetType());
                    write_protocol.Items = new List<IProtocolData>() { new ProtocolData(item.Key, item.Value) };
                    XGTFEnetProtocol w_response = await stream.SendAsync(write_protocol) as XGTFEnetProtocol;

                    Assert.AreEqual(w_response.GetErrorCode(), 0);

                    var read_protocol = new XGTFEnetProtocol(XGTFEnetCommand.READ_REQT, item.Value.GetType());
                    read_protocol.Items = new List<IProtocolData>() { new ProtocolData(item.Key, item.Value) };
                    XGTFEnetProtocol r_response = await stream.SendAsync(read_protocol) as XGTFEnetProtocol;

                    Assert.AreEqual(r_response.GetErrorCode(), 0);
                    Assert.AreEqual(r_response.Items[0].Value, item.Value);
                }
            }
        }
    }
}