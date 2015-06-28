
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

        static void OnError(object sender, DataReceivedEventArgs args)
        {
            var p = args.Protocol as XGTCnetProtocol;
            System.Diagnostics.Debug.Assert(false, p.Error.ToString());
        }

        static void OnReceived(object sender, DataReceivedEventArgs args)
        {
            var p = args.Protocol as XGTCnetProtocol;
            Console.Write(p.Command + p.CommandType.ToString() + "-----------------------------------------------------------------------------");
            foreach (var d in p.ResponseDic)
                Console.WriteLine("NAME: " + d.Key + " VALUE: " + d.Value);
            Console.Write("--------------------------------------------------------------------------------");
        }

        static void ReadRSS1(Type type)
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
            var rss = XGTCnetProtocol.NewRSSProtocol(00, p_datas);
            rss.ErrorReceived += OnError;
            CnetSkt.Send(rss);
        }

        static void ReadWSS1(Type type)
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
            var rss = XGTCnetProtocol.NewWSSProtocol(00, p_datas);
            rss.ErrorReceived += OnError;
            CnetSkt.Send(rss);
        }

        static void ReadRSS2()
        {
            List<PValue> p_datas = new List<PValue>();
            //p_datas.Add( new PValue() { Name= "%DX000000", Type = typeof(Boolean) });
            //p_datas.Add( new PValue() { Name= "%DX000001", Type = typeof(Boolean) });
            //p_datas.Add( new PValue() { Name= "%DX000002", Type = typeof(Boolean) });
            //p_datas.Add( new PValue() { Name= "%DX000003", Type = typeof(Boolean) });

            //p_datas.Add( new PValue() { Name= "%DB8000", Type = typeof(Byte) });
            //p_datas.Add( new PValue() { Name= "%DB8001", Type = typeof(Byte) });
            //p_datas.Add( new PValue() { Name= "%DB8002", Type = typeof(Byte) });
            //p_datas.Add( new PValue() { Name= "%DB8003", Type = typeof(Byte) });

            //p_datas.Add( new PValue() { Name= "%DD2", Type = typeof(UInt32) });
            //p_datas.Add( new PValue() { Name= "%DD3", Type = typeof(UInt32) });
            //p_datas.Add( new PValue() { Name= "%DD4", Type = typeof(UInt32) });
            //p_datas.Add( new PValue() { Name= "%DD5", Type = typeof(UInt32) });

            //p_datas.Add( new PValue() { Name= "%DL9", Type = typeof(UInt64) });
            //p_datas.Add( new PValue() { Name= "%DL10", Type = typeof(UInt64) });
            //p_datas.Add( new PValue() { Name= "%DL11", Type = typeof(UInt64) });
            //p_datas.Add( new PValue() { Name = "%DL12", Type = typeof(UInt64) });
            var rss = XGTCnetProtocol.NewRSSProtocol(00, p_datas);
            rss.ErrorReceived += OnError;
            CnetSkt.Send(rss);
        }

        static void ReadRSB(Type type)
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
            var rsb = XGTCnetProtocol.NewRSBProtocol(00, new PValue() { Name = var, Type = type }, 15);
            rsb.ErrorReceived += OnError;
            CnetSkt.Send(rsb);
        }

        static void ReadWSB(Type type)
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
            var rsb = XGTCnetProtocol.NewRSBProtocol(00, new PValue() { Name = var, Type = type }, 15);
            rsb.ErrorReceived += OnError;
            CnetSkt.Send(rsb);
        }

        static void Main(string[] args)
        {
            CnetSkt = new XGTCnetSocket.Builder("COM3", 9600).Build() as XGTCnetSocket;
            CnetSkt.ReceivedSuccessfully += OnReceived;
            if (CnetSkt.Connect())
            {
                //ReadRSS1(typeof(Boolean));
                //ReadRSS1(typeof(Byte));
                //ReadRSS1(typeof(UInt16));
                //ReadRSS1(typeof(UInt32));
                //ReadRSS1(typeof(UInt64));
                //ReadRSS2();
                //ReadWSS1(typeof(Boolean));
                //ReadWSS1(typeof(Byte));
                //ReadWSS1(typeof(UInt16));
                //ReadWSS1(typeof(UInt32));
                //ReadWSS1(typeof(UInt64));
                //ReadRSB1(typeof(Byte));
                //ReadRSB1(typeof(UInt16));
                //ReadRSB1(typeof(UInt32));
                ReadRSB(typeof(UInt64));
            }
            else
                Console.WriteLine("CnetSkt open error!");
            Console.ReadLine();
        }
    }
}
