using System.IO.Ports;

namespace DY.NET
{
    /// <summary>
    /// 시리얼 빌더 클래스
    /// </summary>
    public abstract class ASerialPortBuilder
    {
        protected string PortName;
        protected int BaudRate;
        protected Parity Parity_ = System.IO.Ports.Parity.None;
        protected int DataBit = 8;
        protected StopBits StopBit = System.IO.Ports.StopBits.One;

        protected ASerialPortBuilder(string name, int baud)
        {
            PortName = name;
            BaudRate = baud;
        }

        public ASerialPortBuilder Parity(Parity parity)
        {
            Parity_ = parity;
            return this;
        }

        public ASerialPortBuilder DataBits(int databits)
        {
            DataBit = databits;
            return this;
        }

        public ASerialPortBuilder StopBits(StopBits stopbits)
        {
            StopBit = stopbits;
            return this;
        }

        public abstract object Build();
    }
}
