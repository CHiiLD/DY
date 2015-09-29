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
        private string hostname = "192.168.10.150";

        [Test]
        public async void NegativeNumberTest()
        {
            using (var stream = new XGTFEnetStream(hostname))
            {
                await stream.OpenAsync();
                Assert.True(stream.IsOpend());
                Random r = new Random();
                string addr = "%MB100";
                int cnt = 10;
                for (int i = 0; i < cnt; i++)
                {
                    sbyte value = (sbyte)r.Next(sbyte.MinValue, 0);
                    var write_protocol = new XGTFEnetProtocol(XGTFEnetCommand.WRITE_REQT, value.GetType());
                    write_protocol.Items = new List<IProtocolData>() { new ProtocolData(addr, value) };
                    XGTFEnetProtocol w_response = await stream.SendAsync(write_protocol) as XGTFEnetProtocol;

                    Assert.AreEqual(w_response.GetErrorCode(), 0);

                    var read_protocol = new XGTFEnetProtocol(XGTFEnetCommand.READ_REQT, value.GetType());
                    read_protocol.Items = new List<IProtocolData>() { new ProtocolData(addr, value) };
                    XGTFEnetProtocol r_response = await stream.SendAsync(read_protocol) as XGTFEnetProtocol;

                    Assert.AreEqual(r_response.GetErrorCode(), 0);
                    Assert.AreEqual(r_response.Items[0].Value, (byte)value);
                }
            }
        }

        [Test]
        public async void StreamCommunicationTest()
        {
            using (var stream = new XGTFEnetStream(hostname))
            {
                await stream.OpenAsync();
                Assert.True(stream.IsOpend());
                Random random = new Random();
                Dictionary<string, object> testList = new Dictionary<string, object>() 
                {
                    { "%MB0000", (byte)random.Next(0, byte.MaxValue) },
                    { "%MW100", (ushort)random.Next(0, ushort.MaxValue) },
                    { "%MD200", (uint) (random.NextDouble() * (double)uint.MaxValue) },
                    { "%ML400", (ulong)(random.NextDouble() * (double)ulong.MaxValue) },
                };

                foreach (var item in testList)
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

        [Test]
        public async void BitTypeWriteRequestTest()
        {
            using (var stream = new XGTFEnetStream(hostname))
            {
                await stream.OpenAsync();
                Assert.True(stream.IsOpend());
                Random r = new Random();
                int cnt = 20;
                for (int i = 0; i < cnt; i++)
                {
                    string addr = "%MX172C";
                    bool value = r.Next(0, 2) == 0 ? false : true;
                    Console.WriteLine(value);
                    var write_protocol = new XGTFEnetProtocol(XGTFEnetCommand.WRITE_REQT, value.GetType());
                    write_protocol.Items = new List<IProtocolData>() { new ProtocolData(addr, value) };
                    XGTFEnetProtocol w_response = await stream.SendAsync(write_protocol) as XGTFEnetProtocol;

                    Assert.AreEqual(w_response.GetErrorCode(), 0);

                    var read_protocol = new XGTFEnetProtocol(XGTFEnetCommand.READ_REQT, value.GetType());
                    read_protocol.Items = new List<IProtocolData>() { new ProtocolData(addr, value) };
                    XGTFEnetProtocol r_response = await stream.SendAsync(read_protocol) as XGTFEnetProtocol;

                    Assert.AreEqual(r_response.GetErrorCode(), 0);
                    Assert.AreEqual((byte)r_response.Items[0].Value == 0 ? false : true, value);
                }
            }
        }
    }
}