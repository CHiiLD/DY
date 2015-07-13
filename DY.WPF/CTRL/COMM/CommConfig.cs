using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF.CTRL.COMM
{
    public class CommConfig
    {
        public CommState State { get; set; }
        public bool Enable { get; set; }
        public Comm Comm { get; set; }
        public CommDeviceType DeviceType { get; set; }
        public string Summary { get; set; }
        public string Note { get; set; }
        private bool On { get; set; }
        public object Setting { get; set; }
        public object Socket { get; set; }
    }
}