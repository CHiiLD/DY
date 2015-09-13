using System.IO.Ports;

namespace DY.NET
{
    /// <summary>
    /// 시리얼 빌더 클래스
    /// </summary>
    public abstract class ASerialPortBuilder : SerialPortElement
    {
        protected ASerialPortBuilder(string com, int baud)
        {
            PortName = com;
            BaudRate = baud;
            ParityBit = System.IO.Ports.Parity.None;
            DataBit = 8;
            StopBit = System.IO.Ports.StopBits.One;
        }

        public ASerialPortBuilder Parity(Parity parity)
        {
            ParityBit = parity;
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