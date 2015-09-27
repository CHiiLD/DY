using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.TEST
{
    [TestFixture]
    public class XGTFEnetRealDeviceTest
    {
        private XGTFEnetStream stream;

        [SetUp]
        [TestCase("192.168.10.150")]
        public async void Setup(string hostname)
        {
            stream = new XGTFEnetStream(hostname);
            await stream.OpenAsync();
        }

        [Test]
        public void ConnectionCheckingTest()
        {
            Assert.True(stream.IsOpend());
        }

        [TearDown]
        public void TearDown()
        {
            stream.Close();
        }

        [Test]
        public async void StreamCommunicationTest(string addr, object value)
        {
            Random r = new Random();
            Dictionary<string, object> d = new Dictionary<string, object>() 
            { 
                { "%MX100F", r.Next(0, 2) == 0 ? false : true },
                { "%MB2310", (byte)r.Next(0, byte.MaxValue) },
                { "%MW100", (ushort)r.Next(0, ushort.MaxValue) },
                { "%MD200", (uint)r.Next(0, int.MaxValue) },
                { "%ML400", (ulong)r.Next(0, int.MaxValue) },
            };

            foreach (var item in d)
            {
                XGTFEnetProtocol w_response = await stream.SendAsync(new XGTFEnetProtocol(XGTFEnetCommand.WRITE_REQT, item.Value.GetType().ToXGTFEnetDataType())
                {
                    Items = new List<IProtocolData>()
                    {
                        new ProtocolData(item.Key, item.Value)
                    }
                }) as XGTFEnetProtocol;
                Assert.AreEqual(w_response.GetErrorCode(), 0);

                XGTFEnetProtocol r_response = await stream.SendAsync(new XGTFEnetProtocol(XGTFEnetCommand.READ_REQT, item.Value.GetType().ToXGTFEnetDataType())
                {
                    Items = new List<IProtocolData>()
                    {
                        new ProtocolData(item.Key)
                    }
                }) as XGTFEnetProtocol;

                Assert.AreEqual(r_response.GetErrorCode(), 0);
                Assert.AreEqual(r_response.Items[0].Value , item.Value);
            }
        }
    }
}