using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DY.NET
{
    public class Delivery
    {
        public DeliveryError Error { get; set; }
        public object Package { get; set; }
        public Stopwatch DelivaryTime { get; private set; }

        public Delivery()
        {
            DelivaryTime = Stopwatch.StartNew();
        }

        public Delivery Packing()
        {
            DelivaryTime.Stop();
            return this;
        }
    }
}
