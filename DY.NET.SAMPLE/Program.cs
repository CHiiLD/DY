
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.NET;
using DY.NET.LSIS.XGT;

namespace DY.NET.SAMPLE
{
    class Program
    {
        static XGTCnetSocket CnetSkt;
        static XGTFEnetHeader FEnetHeader;
        static XGTFEnetSocket FEnetSkt;

        static void OnError(object sender, DataReceivedEventArgs args)
        {
            System.Diagnostics.Debug.Assert(false);
        }

        static void OnReceived(object sender, DataReceivedEventArgs args)
        {
            XGTCnetProtocol cnet = args.Protocol as XGTCnetProtocol;
            Console.Write("--------------------------------------------------------------------------------");
            if (cnet != null)
            {
                Console.Write(cnet.Command.ToString(), cnet.CommandType.ToString());
                foreach (var d in cnet.ResponseDic)
                    Console.WriteLine("NAME: " + d.Key + " VALUE: " + d.Value);
            }
            XGTFEnetProtocol fenet = args.Protocol as XGTFEnetProtocol;
            if (fenet != null)
            {
                Console.Write(fenet.Command.ToString(), fenet.DataType.ToString());
                foreach (var d in fenet.ResponseDic)
                    Console.WriteLine("NAME: " + d.Key + " VALUE: " + d.Value);
            }
            Console.Write("--------------------------------------------------------------------------------");
        }

        static void ReadRSS(Type type, XGTModuleComm comm)
        {
            List<string> var_names = new List<string>();
            if (type == typeof(Boolean))
            {
                string[] rat = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", };
                for (int i = 0; i < 16; i++)
                    var_names.Add("D00000." + rat[i]);
            }
            else if (type == typeof(Byte) || type == typeof(SByte))
            {
                for (int i = 0; i < 16; i++)
                    var_names.Add("D0400" + (i / 2).ToString() + "." + ((i % 2 == 0) ? 0 : 8).ToString());
            }
            else if (type == typeof(Int16) || type == typeof(UInt16))
            {
                for (int i = 0; i < 16; i++)
                    var_names.Add("D043" + string.Format("{0:D2}", i));
            }
            else if (type == typeof(Int32) || type == typeof(UInt32))
            {
                for (int i = 0; i < 16; i++)
                    var_names.Add("D00" + string.Format("{0:D2}", (i * 2) + 4));
            }
            else if (type == typeof(Int64) || type == typeof(UInt64))
            {
                for (int i = 0; i < 16; i++)
                    var_names.Add("D00" + string.Format("{0:D2}", (i * 4) + 36));
            }
            List<PValue> p_datas = new List<PValue>();
            foreach (var b in var_names)
            {
                string glopa_name;
                if (BS2G.ToGlopaNotation(b, type, out glopa_name))
                {
                    var p_data = new PValue() { Name = glopa_name, Type = type };
                    p_datas.Add(p_data);
                }
            }
            IProtocol rss;
            switch (comm)
            {
                case XGTModuleComm.CNET:
                    rss = XGTCnetProtocol.NewRSSProtocol(00, p_datas);
                    rss.ErrorReceived += OnError;
                    CnetSkt.Send(rss);
                    break;
                case XGTModuleComm.FENET:
                    rss = XGTFEnetProtocol.NewRSSProtocol(FEnetHeader, p_datas);
                    rss.ErrorReceived += OnError;
                    FEnetSkt.Send(rss);
                    break;
            }
        }

        static void ReadWSS(Type type, XGTModuleComm comm)
        {
            List<string> var_names = new List<string>();
            object value = null;
            if (type == typeof(Boolean))
            {
                value = true;
                string[] rat = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", };
                for (int i = 0; i < 16; i++)
                    var_names.Add("D00000." + rat[i]);
            }
            else if (type == typeof(Byte) || type == typeof(SByte))
            {
                value = 0xFF;
                for (int i = 0; i < 16; i++)
                    var_names.Add("D0400" + (i / 2).ToString() + "." + ((i % 2 == 0) ? 0 : 8).ToString());
            }
            else if (type == typeof(Int16) || type == typeof(UInt16))
            {
                value = 0xFFFF;
                for (int i = 0; i < 16; i++)
                    var_names.Add("D043" + string.Format("{0:D2}", i));
            }
            else if (type == typeof(Int32) || type == typeof(UInt32))
            {
                value = 0xFFFFFFFF;
                for (int i = 0; i < 16; i++)
                    var_names.Add("D00" + string.Format("{0:D2}", (i * 2) + 4));
            }
            else if (type == typeof(Int64) || type == typeof(UInt64))
            {
                value = 0xFFFFFFFFFFFFFFFF;
                for (int i = 0; i < 16; i++)
                    var_names.Add("D00" + string.Format("{0:D2}", (i * 4) + 36));
            }
            List<PValue> p_datas = new List<PValue>();
            foreach (var b in var_names)
            {
                string glopa_name;
                if (BS2G.ToGlopaNotation(b, type, out glopa_name))
                {
                    var p_data = new PValue() { Name = glopa_name, Type = type, Value = value };
                    p_datas.Add(p_data);
                }
            }

            IProtocol wss;
            switch (comm)
            {
                case XGTModuleComm.CNET:
                    wss = XGTCnetProtocol.NewWSSProtocol(00, p_datas);
                    wss.ErrorReceived += OnError;
                    CnetSkt.Send(wss);
                    break;
                case XGTModuleComm.FENET:
                    wss = XGTFEnetProtocol.NewWSSProtocol(FEnetHeader, p_datas);
                    wss.ErrorReceived += OnError;
                    FEnetSkt.Send(wss);
                    break;
            }
        }

        static void ReadRSB(Type type, XGTModuleComm comm)
        {
            string var = "";
            if (type == typeof(Byte) || type == typeof(SByte))
                var = "%DB8000";
            else if (type == typeof(Int16) || type == typeof(UInt16))
                var = "%DW04300";
            else if (type == typeof(Int32) || type == typeof(UInt32))
                var = "%DD2";
            else if (type == typeof(Int64) || type == typeof(UInt64))
                var = "%DL9";

            IProtocol rsb;
            switch (comm)
            {
                case XGTModuleComm.CNET:
                    rsb = XGTCnetProtocol.NewRSBProtocol(00, new PValue() { Name = var, Type = type }, 15);
                    rsb.ErrorReceived += OnError;
                    CnetSkt.Send(rsb);
                    break;
                case XGTModuleComm.FENET:
                    rsb = XGTFEnetProtocol.NewRSBProtocol(FEnetHeader, new PValue() { Name = var, Type = type }, 16);
                    rsb.ErrorReceived += OnError;
                    FEnetSkt.Send(rsb);
                    break;
            }
        }

        static void ReadWSB(Type type, XGTModuleComm comm)
        {
            List<string> var_names = new List<string>();
            object value = null;
            if (type == typeof(Boolean))
            {
                value = true;
                string[] rat = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", };
                for (int i = 0; i < 16; i++)
                    var_names.Add("D00000." + rat[i]);
            }
            else if (type == typeof(Byte) || type == typeof(SByte))
            {
                value = 0xFF;
                for (int i = 0; i < 16; i++)
                    var_names.Add("D0400" + (i / 2).ToString() + "." + ((i % 2 == 0) ? 0 : 8).ToString());
            }
            else if (type == typeof(Int16) || type == typeof(UInt16))
            {
                value = 0xFFFF;
                for (int i = 0; i < 16; i++)
                    var_names.Add("D043" + string.Format("{0:D2}", i));
            }
            else if (type == typeof(Int32) || type == typeof(UInt32))
            {
                value = 0xFFFFFFFF;
                for (int i = 0; i < 16; i++)
                    var_names.Add("D00" + string.Format("{0:D2}", (i * 2) + 4));
            }
            else if (type == typeof(Int64) || type == typeof(UInt64))
            {
                value = 0xFFFFFFFFFFFFFFFF;
                for (int i = 0; i < 16; i++)
                    var_names.Add("D00" + string.Format("{0:D2}", (i * 4) + 36));
            }
            List<PValue> p_datas = new List<PValue>();
            foreach (var b in var_names)
            {
                string glopa_name;
                if (BS2G.ToGlopaNotation(b, type, out glopa_name))
                {
                    var p_data = new PValue() { Name = glopa_name, Type = type, Value = value };
                    p_datas.Add(p_data);
                }
            }
           
            IProtocol wsb;
            switch (comm)
            {
                case XGTModuleComm.CNET:
                    wsb = XGTCnetProtocol.NewWSBProtocol(00, p_datas);
                    wsb.ErrorReceived += OnError;
                    CnetSkt.Send(wsb);
                    break;
                case XGTModuleComm.FENET:
                    wsb = XGTFEnetProtocol.NewWSBProtocol(FEnetHeader, p_datas);
                    wsb.ErrorReceived += OnError;
                    FEnetSkt.Send(wsb);
                    break;
            }
        }

        static void LSIS_XGT_SerialPortCommunication()
        {
            CnetSkt = new XGTCnetSocket.Builder("COM3", 9600).Build() as XGTCnetSocket;
            CnetSkt.ReceivedSuccessfully += OnReceived;
            if (CnetSkt.Connect())
            {
                ReadRSS(typeof(Boolean), XGTModuleComm.CNET);
                ReadRSS(typeof(Byte), XGTModuleComm.CNET);
                ReadRSS(typeof(UInt16), XGTModuleComm.CNET);
                ReadRSS(typeof(UInt32), XGTModuleComm.CNET);
                ReadRSS(typeof(UInt64), XGTModuleComm.CNET);

                ReadWSS(typeof(Boolean), XGTModuleComm.CNET);
                ReadWSS(typeof(Byte), XGTModuleComm.CNET);
                ReadWSS(typeof(UInt16), XGTModuleComm.CNET);
                ReadWSS(typeof(UInt32), XGTModuleComm.CNET);
                ReadWSS(typeof(UInt64), XGTModuleComm.CNET);

                ReadRSB(typeof(Byte), XGTModuleComm.CNET);
                ReadRSB(typeof(UInt16), XGTModuleComm.CNET);
                ReadRSB(typeof(UInt32), XGTModuleComm.CNET);
                ReadRSB(typeof(UInt64), XGTModuleComm.CNET);

                ReadWSB(typeof(Byte), XGTModuleComm.CNET);
                ReadWSB(typeof(UInt16), XGTModuleComm.CNET);
                ReadWSB(typeof(UInt32), XGTModuleComm.CNET);
                ReadWSB(typeof(UInt64), XGTModuleComm.CNET);
            }
            else
                Console.WriteLine("CnetSkt open error!");
            Console.ReadLine();
        }

        static void LSIS_XGT_TcpIP_Communication()
        {
            XGTFEnetPLCInfo plc_info = new XGTFEnetPLCInfo();
            plc_info.Set(XGTFEnetCpuType.XGK_CPUH, XGTFEnetClass.SLAVE, XGTFEnetCpuState.CPU_NOR, XGTFEnetPLCSystemState.RUN);
            FEnetHeader = XGTFEnetHeader.CreateXGTFEnetHeader(plc_info, XGTFEnetCpuInfo.XGK, 00, 02, 00);
            try
            {
                FEnetSkt = (XGTFEnetSocket)new XGTFEnetSocket.Builder("192.168.10.150", (int)XGTFEnetPort.TCP).Build();
                FEnetSkt.ReceivedSuccessfully += OnReceived;
                if (FEnetSkt.Connect())
                {
                    ReadRSS(typeof(Boolean), XGTModuleComm.FENET);
                    //ReadRSS(typeof(Byte), XGTModuleComm.FENET);
                    //ReadRSS(typeof(UInt16), XGTModuleComm.FENET);
                    //ReadRSS(typeof(UInt32), XGTModuleComm.FENET);
                    //ReadRSS(typeof(UInt64), XGTModuleComm.FENET);

                    //ReadWSS(typeof(Boolean), XGTModuleComm.FENET);
                    //ReadWSS(typeof(Byte), XGTModuleComm.FENET);
                    //ReadWSS(typeof(UInt16), XGTModuleComm.FENET);
                    //ReadWSS(typeof(UInt32), XGTModuleComm.FENET);
                    //ReadWSS(typeof(UInt64), XGTModuleComm.FENET);

                    //ReadRSB(typeof(Byte), XGTModuleComm.FENET);
                    //ReadRSB(typeof(UInt16), XGTModuleComm.FENET);
                    //ReadRSB(typeof(UInt32), XGTModuleComm.FENET);
                    //ReadRSB(typeof(UInt64), XGTModuleComm.FENET);

                    //ReadWSB(typeof(Byte), XGTModuleComm.FENET);
                    //ReadWSB(typeof(UInt16), XGTModuleComm.FENET);
                    //ReadWSB(typeof(UInt32), XGTModuleComm.FENET);
                    //ReadWSB(typeof(UInt64), XGTModuleComm.FENET);
                }
                else
                    Console.WriteLine("CnetSkt open error!");
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.Assert(false);
            }
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            //LSIS_XGT_SerialPortCommunication();
            LSIS_XGT_TcpIP_Communication();
        }
    }
}
