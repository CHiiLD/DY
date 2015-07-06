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
        private static bool m_Swtich = true;

        //private static void DataLogic_Matrix200_Loop(Matrix200HostMode hostmode)
        //{
        //    do
        //    {
        //        Console.WriteLine("*** 종료 Q");
        //        Console.WriteLine("*** One Shot-> 1");
        //        string ret = Console.ReadLine();
        //        if (ret == "Q")
        //            break;
        //        if (ret == "1")
        //            hostmode.ReadBarcode();
        //        else
        //            Console.WriteLine("잘못된 입력");
        //    } while (true);
        //}

        //private static void DataLogic_Matrix200_EnterHostMode(object sender, EventArgs args)
        //{
        //    Matrix200HostMode hostmode = sender as Matrix200HostMode;
        //    if (hostmode.State != Matrix200HostModeState.OK)
        //    {
        //        Console.WriteLine("호스트 프로그래밍 모드 접속 실패: " + hostmode.State.ToString());
        //        return;
        //    }
        //    Console.WriteLine("호스트 프로그래밍 모드 접속 성공: " + hostmode.State.ToString());
        //    DataLogic_Matrix200_Loop(hostmode);
        //    hostmode.TryExitHostProgrammingMode(DataLogic_Matrix200_ExitHostMode);
        //}

        //private static void DataLogic_Matrix200_ExitHostMode(object sender, EventArgs args)
        //{
        //    Matrix200HostMode hostmode = sender as Matrix200HostMode;
        //    if (hostmode.State != Matrix200HostModeState.OK)
        //    {
        //        Console.WriteLine("호스트 프로그래밍 모드 종료 실패: " + hostmode.State.ToString());
        //        return;
        //    }
        //    Console.WriteLine("호스트 프로그래밍 모드 종료 성공: " + hostmode.State.ToString());
        //    Console.ReadLine();
        //    m_Swtich = false;
        //}

        private static void DataLogic_Matrix200_Exit(object obj, Matrix200EventArgs args)
        {
            Console.Write(args.State.ToString() + " " + args.Error.ToString() + "\n");
            Console.WriteLine("**************************");
            m_Swtich = false;
        }

        private static void DataLogic_Matrix200_Read(object obj, Matrix200EventArgs args)
        {
            Console.Write(args.State.ToString() + " " + args.Error.ToString() + "\n");
            Console.WriteLine("**************************");
        }

        private static void DataLogic_Matrix200_Enter(object obj, Matrix200EventArgs args)
        {
            if (args != null)
                Console.Write(args.State.ToString() + " " + args.Error.ToString() + "\n");
            Console.WriteLine("**************************");
            Matrix200HostMode hostmode = obj as Matrix200HostMode;
            do
            {
                Console.WriteLine("***종료 Q");
                Console.WriteLine("***리딩 R");
                string key = Console.ReadLine();
                if(key == "Q")
                {
                    hostmode.ExitProgrammingMode((o, e) => 
                    {
                        hostmode.ExitHostMode(DataLogic_Matrix200_Exit);    
                    });
                    break;
                }
                else if (key == "R")
                {
                    //hostmode.ReadBarcode(DataLogic_Matrix200_Read);
                    byte[] bb = new byte[] { 0x1b, (byte)'A', (byte)'A', (byte)'0' };
                    hostmode.Serial.Write(bb, 0, bb.Length);
                    hostmode.Serial.Write(hostmode.CMD_END_SINGLE_PARAMETER_SEQ, 0, hostmode.CMD_END_SINGLE_PARAMETER_SEQ.Length);
                }
                else
                {

                }
            } while (true);
        }

        public static void DataLogic_Matrix200_Test()
        {
            Matrix200HostMode hostmode = Matrix200HostMode.CreateMaxtrix200HostMode("COM6", 115200);
            if (!hostmode.Connect())
            {
                Console.WriteLine("시리얼포트 연결 실패");
                return;
            }
            hostmode.EnterHostMode((o, e) =>
            {
                if(e.Error == Matrix200HostModeError.OK)
                {
                    Console.WriteLine("프로그래밍 모드 진입 중 ..");
                    hostmode.EnterProgrammingMode(DataLogic_Matrix200_Enter);
                }
                else
                {
                    Console.WriteLine("프로그래밍 모드 진입 실패");
                    m_Swtich = false;
                }
            });
            while (m_Swtich) { System.Threading.Thread.Sleep(1000); }
            Console.ReadKey();
        }
    }
}
