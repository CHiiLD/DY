using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.SAMPLE.LEAK_TESTER
{
    public class SerialNumber
    {
        public int SerialNumberStart_L { get; set; }
        public int SerialNumberStart_R { get; set; }
        public int SerialNumber_L { get; set; }
        public int SerialNumber_R { get; set; }

        public SerialNumber()
        {
            SerialNumberStart_L = SerialNumber_L = 1;
            SerialNumberStart_R = SerialNumber_R = 5001;
        }

        public SerialNumber(SerialNumber that)
        {
            SerialNumberStart_L = that.SerialNumberStart_L;
            SerialNumberStart_R = that.SerialNumberStart_R;
            SerialNumber_L = that.SerialNumber_L;
            SerialNumber_R = that.SerialNumber_R;
        }

        public void SerialNumberInit()
        {
            SerialNumber_L = SerialNumberStart_L;
            SerialNumber_R = SerialNumberStart_R;
        }
    }
}
