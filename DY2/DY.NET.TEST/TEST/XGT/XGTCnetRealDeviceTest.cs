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
        private byte m_localport = 00;
        private string m_Com = "COM3";
        private int m_Bandrate = 9600;
        private System.IO.Ports.Parity m_Parity = System.IO.Ports.Parity.None;
        private int m_DataBit = 8;
        private System.IO.Ports.StopBits m_Stopbit = System.IO.Ports.StopBits.One;

        [Test]
        public async void OpenCloseOpneClose()
        {
            var stream = new XGTCnetStream(m_localport, m_Com, m_Bandrate, m_Parity, m_DataBit, m_Stopbit);
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
            using (var stream = new XGTCnetStream(m_localport, m_Com, m_Bandrate, m_Parity, m_DataBit, m_Stopbit))
            {
                await stream.OpenAsync();
                Assert.True(stream.IsOpend());
                Random r = new Random();
                string addr = "%MB100";
                int cnt = 5;
                for (int i = 0; i < cnt; i++)
                {
                    sbyte value = (sbyte)r.Next(sbyte.MinValue, 0);
                    XGTCnetProtocol write_protocol = new XGTCnetProtocol(typeof(sbyte), XGTCnetCommand.WRITE)
                    { 
                        Data = new List<IProtocolData>() { new ProtocolData(addr, value) }
                    };
                    XGTCnetProtocol w_response = await stream.SendAsync(write_protocol) as XGTCnetProtocol;

                    Assert.AreEqual(w_response.GetErrorCode(), 0);

                    XGTCnetProtocol read_protocol = new XGTCnetProtocol(typeof(sbyte), XGTCnetCommand.READ)
                    {
                        Data = new List<IProtocolData>() { new ProtocolData(addr, value) }
                    };
                    XGTCnetProtocol r_response = await stream.SendAsync(read_protocol) as XGTCnetProtocol;
                    
                    Assert.AreEqual(r_response.GetErrorCode(), 0);
                    Assert.AreEqual(r_response.Data[0].Value, value);
                }
            }
        }

        [Test]
        public async void WriteRead()
        {
            using (var stream = new XGTCnetStream(m_localport, m_Com, m_Bandrate, m_Parity, m_DataBit, m_Stopbit))
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
                    string addr = item.Key;
                    object value = item.Value;

                    XGTCnetProtocol write_protocol = new XGTCnetProtocol(value.GetType(), XGTCnetCommand.WRITE)
                    {
                        Data = new List<IProtocolData>() { new ProtocolData(addr, value) }
                    };
                    XGTCnetProtocol w_response = await stream.SendAsync(write_protocol) as XGTCnetProtocol;

                    Assert.AreEqual(w_response.GetErrorCode(), 0);

                    XGTCnetProtocol read_protocol = new XGTCnetProtocol(value.GetType(), XGTCnetCommand.READ)
                    {
                        Data = new List<IProtocolData>() { new ProtocolData(addr, value) }
                    };
                    XGTCnetProtocol r_response = await stream.SendAsync(read_protocol) as XGTCnetProtocol;

                    Assert.AreEqual(r_response.GetErrorCode(), 0);
                    Assert.AreEqual(r_response.Data[0].Value, value);
                }
            }
        }
    }
}
