using System.IO.Ports;

namespace DY.NET
{
    public abstract class ASerialPortBuilder
    {
        protected string _PortName;
        protected int _BaudRate = 9600;
        protected Parity _Parity = System.IO.Ports.Parity.None;
        protected int _DataBits = 8;
        protected StopBits _StopBits = System.IO.Ports.StopBits.One;

        protected ASerialPortBuilder(string name, int baud)
        {
            _PortName = name;
            _BaudRate = baud;
        }

        public ASerialPortBuilder Parity(Parity parity)
        {
            _Parity = parity;
            return this;
        }

        public ASerialPortBuilder DataBits(int databits)
        {
            _DataBits = databits;
            return this;
        }

        public ASerialPortBuilder StopBits(StopBits stopbits)
        {
            _StopBits = stopbits;
            return this;
        }

        public abstract object Build();
    }
}
