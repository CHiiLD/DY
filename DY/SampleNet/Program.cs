using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using log4net;
using DY.NET.LSIS.XGT;

namespace SampleNet
{
    class Program
    {
        protected static readonly ILog Logger =
                 LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
#if false
            int integer = 0x12;
            int asc_size = 4;
            string hex_str = string.Format("{0:X}", integer);
            byte[] asc = new byte[asc_size];
            for (int i = 0; i < asc.Length; i++)
                asc[i] = (byte)'0';
            int asc_idx = asc_size - 1;
            for (int i = hex_str.Length - 1; i >= 0; i--)
               asc[asc_idx--] = (byte)hex_str[i];
            Logger.Debug(asc.ToString());
#endif

            byte[] v = { 0x41, 0x39, 0x46, 0x33 };
            ushort value =  TransASC<ushort>.ToInt(v);
            Logger.Debug(value);

            int a = -31;
            long b = a;
            Logger.Debug("");
        }
    }
}
