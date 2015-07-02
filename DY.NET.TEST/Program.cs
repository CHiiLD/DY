using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.NET;
using DY.NET.LSIS.XGT;

namespace DY.NET.TEST
{
    class Program
    {
        static void Main(string[] args)
        {
            string OUT_B = "%PB0004";
            var dic = new List<string>();
            dic.Add(OUT_B);
            XGTFEnetProtocol<byte> protocol = XGTFEnetProtocol<byte>.NewRSSProtocol(00, dic);
            XGTFEnetSocket socket = new XGTFEnetSocket("192.168.10.150", XGTFEnetPort.TCP);
            if(socket.Connect())
            {
                socket.ReceivedProtocolSuccessfully += (o, e) => 
                {
                    e.Protocol.Print();
                };
                socket.Send(protocol);
                Console.ReadLine();
            }
        }
    }
}
