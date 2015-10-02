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

namespace DY.NET.Test
{
    [Ignore]
    [TestFixture]
    public class XGTFEnetRealDeviceTest
    {
        private string hostname = "192.168.10.150";

        [Ignore]
        [Test]
        public async void OpenCloseOpneClose()
        {
            var stream = new XGTFEnetStream(hostname);
            await stream.OpenAsync();
            stream.Close();
            await stream.OpenAsync();
            stream.Close();
            await stream.OpenAsync();
            stream.Close();
            await stream.OpenAsync();
            stream.Close();
            await stream.OpenAsync();
            stream.Close();
        }

        [Test]
        public async void WhenCastSigned2UnSigned_CheckInvalidCasting()
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
                    var write_protocol = new XGTFEnetProtocol(value.GetType(), XGTFEnetCommand.WRITE_REQT);
                    write_protocol.Data = new List<IProtocolData>() { new ProtocolData(addr, value) };
                    XGTFEnetProtocol w_response = await stream.SendAsync(write_protocol) as XGTFEnetProtocol;

                    Assert.AreEqual(w_response.GetErrorCode(), 0);

                    var read_protocol = new XGTFEnetProtocol(value.GetType(), XGTFEnetCommand.READ_REQT);
                    read_protocol.Data = new List<IProtocolData>() { new ProtocolData(addr, value) };
                    XGTFEnetProtocol r_response = await stream.SendAsync(read_protocol) as XGTFEnetProtocol;

                    Assert.AreEqual(r_response.GetErrorCode(), 0);
                    Assert.AreEqual(r_response.Data[0].Value, value);
                }
            }
        }

        [Test]
        public async void WriteRead()
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
                    var write_protocol = new XGTFEnetProtocol(item.Value.GetType(), XGTFEnetCommand.WRITE_REQT);
                    write_protocol.Data = new List<IProtocolData>() { new ProtocolData(item.Key, item.Value) };
                    XGTFEnetProtocol w_response = await stream.SendAsync(write_protocol) as XGTFEnetProtocol;

                    Assert.AreEqual(w_response.GetErrorCode(), 0);

                    var read_protocol = new XGTFEnetProtocol(item.Value.GetType(), XGTFEnetCommand.READ_REQT);
                    read_protocol.Data = new List<IProtocolData>() { new ProtocolData(item.Key, item.Value) };
                    XGTFEnetProtocol r_response = await stream.SendAsync(read_protocol) as XGTFEnetProtocol;

                    Assert.AreEqual(r_response.GetErrorCode(), 0);
                    Assert.AreEqual(r_response.Data[0].Value, item.Value);
                }
            }
        }
    }
}