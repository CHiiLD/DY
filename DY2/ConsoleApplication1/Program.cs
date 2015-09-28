using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.NET;
using DY.NET.LSIS.XGT;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            test();
        }

        static async void test()
        {
            string hostname = "192.168.10.150";
            XGTFEnetStream stream = new XGTFEnetStream(hostname);
            await stream.OpenAsync();

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
                try
                {
                    XGTFEnetProtocol w_response = await stream.SendAsync(new XGTFEnetProtocol(XGTFEnetCommand.WRITE_REQT, item.Value.GetType())
                    {
                        Items = new List<IProtocolData>()
                    {
                        new ProtocolData(item.Key, item.Value)
                    }
                    }) as XGTFEnetProtocol;

                    XGTFEnetProtocol r_response = await stream.SendAsync(new XGTFEnetProtocol(XGTFEnetCommand.READ_REQT, item.Value.GetType())
                    {
                        Items = new List<IProtocolData>()
                    {
                        new ProtocolData(item.Key)
                    }
                    }) as XGTFEnetProtocol;
                }
                catch (Exception e)
                {

                }
            }
            stream.Close();
        }
    }
}
