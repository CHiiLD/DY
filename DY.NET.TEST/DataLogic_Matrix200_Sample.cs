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

        public static async void Capture(Matrix200 m200)
        {
            Tuple<int, int> cap_size = await m200.CaptureAsync();
            if (cap_size != null)
                Console.WriteLine("capture size width: {0}, height: {1}", cap_size.Item1, cap_size.Item2);
        }

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
                Console.WriteLine("D -> Disconnect");
                Console.WriteLine("C -> Capture");
                Console.WriteLine("E -> Decoding");
                Console.WriteLine("*****************************");
                ConsoleKeyInfo str = Console.ReadKey();
                if ('Q' == str.KeyChar)
                {
                    m200.Close();
                    m200.Dispose();
                    m_Loop = false;
                }
                else if ('D' == str.KeyChar)
                {
                    Console.WriteLine("disconnect ..");
                    m200.Disconnect();
                }
                else if ('C' == str.KeyChar)
                {
                    Console.WriteLine("capture ..");
                    Capture(m200);
                }
                else if ('E' == str.KeyChar)
                {
                    Console.WriteLine("decoding ..");
                    await m200.DecodingAsync();
                }
            } while (m_Loop);
            System.Threading.Thread.Sleep(1000);
        }
    }
}
