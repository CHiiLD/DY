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
            Error = DeliveryError.SUCCESS;
            DelivaryTime = Stopwatch.StartNew();
        }

        public Delivery Packing()
        {
            DelivaryTime.Stop();
            return this;
        }

        public Delivery Packing(DeliveryError error)
        {
            Error = error;
            DelivaryTime.Stop();
            return this;
        }

        public Delivery Packing(object package, DeliveryError error)
        {
            Error = error;
            Package = package;
            DelivaryTime.Stop();
            return this;
        }
    }
}