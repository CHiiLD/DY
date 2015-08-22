using System;

namespace DY.NET
{
    public class DeliveryArrivalEventArgs : EventArgs
    {
        public Delivery Delivery { get; private set; }

        public DeliveryArrivalEventArgs(Delivery delivery)
        {
            this.Delivery = delivery;
        }
    }
}