using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using log4net;
using DY.NET.LSIS.XGT;
using DY.NET;
using System.IO.Ports;
using System.Threading;

namespace SampleNet
{
    class Program
    {
        protected static ILog Logger;

        public static void Output(object o, SocketDataReceivedEventArgs e)
        {
             XGTCnetExclusiveProtocol p = e.Protocol as XGTCnetExclusiveProtocol;
             p.PrintInfo();
        }

        static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();
            Logger =
                 LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            SerialPort serialPort = new SerialPort();
            serialPort = new System.IO.Ports.SerialPort("COM3", 19200);
            serialPort.Encoding = Encoding.ASCII;
            serialPort.Parity = System.IO.Ports.Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = System.IO.Ports.StopBits.One;
            serialPort.Open();

            XGTCnetExclusiveSocket socket = new XGTCnetExclusiveSocket(serialPort);

            try
            {
                //RSS
                var protocol = XGTCnetExclusiveProtocol.GetRSSProtocol(0, new ENQDataFormat("%PX00000"));
                protocol.DataReceivedEvent += Output;
                protocol.AssembleProtocol();
                //protocol.PrintInfo();
                socket.Send(protocol);
            }
            catch(Exception e)
            {
                Logger.Debug(e.ToString(), e);
            }

            try
            {
                //WSS
                var protocol = XGTCnetExclusiveProtocol.GetWSSProtocol(0, new ENQDataFormat("%PX00000", 1));
                protocol.DataReceivedEvent += Output;
                protocol.AssembleProtocol();
                socket.Send(protocol);
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString(), e);
            }
            Thread.Sleep(1000);

            try
            {
                //RSS
                var protocol = XGTCnetExclusiveProtocol.GetRSSProtocol(0, new ENQDataFormat("%PX00000"));
                protocol.DataReceivedEvent += Output;
                protocol.AssembleProtocol();
                socket.Send(protocol);
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString(), e);
            }

            Thread.Sleep(10000*100);
        }
    }
}
