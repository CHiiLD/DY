using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DY.NET;
using DY.NET.LSIS.XGT;
using System.IO.Ports;
using System.Diagnostics;

namespace DYNET_Lib_Test
{
    class Program
    {
        static readonly object _sync = new object();
        static XGTCnetExclusiveSocket _Socket;
        static XGTCnetExclusiveSocket Socket
        {
            get
            {
                lock (_sync)
                    return _Socket;
            }
            set
            {
                lock (_sync)
                    _Socket = value;
            }
        }
        static string MoniterVar = "%MX10";
        static string EventVar = "%DW100";
        static ushort Result;

        static void CheckM10()
        {
            var protocol = XGTCnetExclusiveProtocol.GetRSSProtocol(0, new ENQDataFormat(MoniterVar));
            protocol.DataReceivedEvent += (object socket, SocketDataReceivedEventArgs e) => 
            {
                XGTCnetExclusiveProtocol p = e.Protocol as XGTCnetExclusiveProtocol;
                byte data = (Byte)p.ACKDatas[0].Data;
                if (data == 0)
                {
                    System.Threading.Thread.Sleep(100);
                    Socket.Send(p.ReqtProtocol);
                }
                else if (data == 1)
                {
                    XGTCnetExclusiveProtocol DeviceRead = XGTCnetExclusiveProtocol.GetRSSProtocol(0, new ENQDataFormat(EventVar));
                    DeviceRead.DataReceivedEvent += ReadD100;
                    Task.Factory.StartNew(() => { System.Threading.Thread.Sleep(100);  Socket.Send(DeviceRead); });
                }
                else
                    Debug.Assert(false);
            };
            Socket.Send(protocol);
        }

        static void ReadD100(object socket, SocketDataReceivedEventArgs args)
        {
            XGTCnetExclusiveProtocol DeviceRead = XGTCnetExclusiveProtocol.GetRSSProtocol(0, new ENQDataFormat(EventVar));
            DeviceRead.DataReceivedEvent += (object skt, SocketDataReceivedEventArgs ev) =>
            {
                XGTCnetExclusiveProtocol p = ev.Protocol as XGTCnetExclusiveProtocol;
                ushort ret = (ushort)p.ACKDatas[0].Data;
                Result = ret;
                Console.WriteLine(string.Format("Read By {0}, Value : {1}", EventVar, Result));
                WriteM10ToOff();
            };
            Socket.Send(DeviceRead);
        }

        static void WriteM10ToOff()
        {
            var protocol = XGTCnetExclusiveProtocol.GetWSSProtocol(0, new ENQDataFormat(MoniterVar, (byte)0));
            protocol.DataReceivedEvent += (object socket, SocketDataReceivedEventArgs e) =>
            {
                Task.Factory.StartNew(() => { System.Threading.Thread.Sleep(100);  CheckM10(); });
            };
            Socket.Send(protocol);
        }

        static void Main(string[] args)
        {
            // 모니터할 변수 M10 (bit) %XM10
            // 리드할 변수   D100 %WD100
            DYSerialPort serialPort = new DYSerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            serialPort.Open();
            Socket = new XGTCnetExclusiveSocket(serialPort);
            CheckM10();
            System.Threading.Thread.Sleep(10000 * 1000);
        }
    }
}
