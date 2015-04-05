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

            DYSerialPort serialPort = new DYSerialPort("COM3", 19200, Parity.None, 8, StopBits.One);
            serialPort.Open();

            Logger.Debug("debug");
            XGTCnetExclusiveSocket socket = new XGTCnetExclusiveSocket(serialPort);
#if false    //   RSS&WSS TEST
            //RSS
            try
            {
                var protocol = XGTCnetExclusiveProtocol.GetRSSProtocol(0, new ENQDataFormat("%PX00000"));
                protocol.DataReceivedEvent += Output;
                //protocol.PrintInfo();
                socket.Send(protocol);
            }
            catch(Exception e)
            {
                Logger.Debug(e.ToString(), e);
            }

            //WSS
            try
            {
                var protocol = XGTCnetExclusiveProtocol.GetWSSProtocol(0, new ENQDataFormat("%PX00000", 1));
                protocol.DataReceivedEvent += Output;
                socket.Send(protocol);
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString(), e);
            }

            //RSS
            try
            {
                var protocol = XGTCnetExclusiveProtocol.GetRSSProtocol(0, new ENQDataFormat("%PX00000"));
                protocol.DataReceivedEvent += Output;
                socket.Send(protocol);
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString(), e);
            }
#endif


            string varName = "%DW4000";
#if false
            
            //RSB
            var protocol = XGTCnetExclusiveProtocol.GetRSBProtocol(0, varName, 2);
            protocol.DataReceivedEvent += Output;
            socket.Send(protocol);
            protocol.PrintInfo();
            
#endif

#if true
            List<ENQDataFormat> enq = new List<ENQDataFormat>();
            enq.Add(new ENQDataFormat(varName, (ushort)0003));
            enq.Add(new ENQDataFormat(varName, (ushort)0004));

            //WSB
            var protocol = XGTCnetExclusiveProtocol.GetWSBProtocol(0, enq);
            protocol.DataReceivedEvent += Output;
            socket.Send(protocol);
            protocol.PrintInfo();

            protocol = XGTCnetExclusiveProtocol.GetRSBProtocol(0, varName, 2);
            protocol.DataReceivedEvent += Output;
            socket.Send(protocol);
            protocol.PrintInfo();
#endif
            Thread.Sleep(10000 * 100);
        }
    }
}
