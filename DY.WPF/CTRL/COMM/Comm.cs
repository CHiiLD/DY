using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF.CTRL.COMM
{
    public enum Comm
    {
        SERIAL,
        ETHERNET
    }

    public static class CommExtension
    {
        public static string ToString(this Comm comm)
        {
            string ret = "";
            switch (comm)
            {
                case Comm.SERIAL:
                    ret = "Serial Port";
                    break;
                case Comm.ETHERNET:
                    ret = "Ethernet";
                    break;
            }
            return ret;
        }
    }
}