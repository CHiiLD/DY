using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace DY.WPF.CTRL.COMM
{
    public struct CommSerial
    {
        public string Port { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }
        //public CommSerial()
        //{
        //    Parity = System.IO.Ports.Parity.None;
        //    DataBits = 8;
        //    StopBits = System.IO.Ports.StopBits.One;
        //}
    }
}
