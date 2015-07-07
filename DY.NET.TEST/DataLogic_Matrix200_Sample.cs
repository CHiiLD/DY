using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.NET.DATALOGIC.MATRIX;

namespace DY.NET.TEST
{
    public static class DataLogic_Matrix200_Sample
    {
        private static bool m_Loop = true;

        public static async void DataLogic_Matrix200_Test()
        {
            Matrix200 m200 = Matrix200.CreateMaxtrix200HostMode("COM3", 115200);
            if (!m200.Connect())
                Console.WriteLine("Serial port connect fail!");
            await m200.PrepareAsync();
            do
            {
                Console.WriteLine("*****************************");
                Console.WriteLine("Q -> Exit");
                Console.WriteLine("DISC -> Disconnect");
                Console.WriteLine("CAP -> Capture");
                Console.WriteLine("DEC -> Decoding");
                Console.WriteLine("*****************************");
                string str = Console.ReadLine();
                if ("Q" == str)
                {
                    //m200.Dispose();
                    m_Loop = false;
                }
                else if ("DISC" == str)
                {
                    Console.WriteLine("disconnect ..");
                    m200.Disconnect();
                }
                else if ("CAP" == str)
                {
                    Console.WriteLine("capture ..");
                    m200.Capture();
                }
                else if ("DEC" == str)
                {
                    Console.WriteLine("decoding ..");
                    m200.Decoding();
                }
            } while (m_Loop);
            System.Threading.Thread.Sleep(1000);
        }
    }
}
