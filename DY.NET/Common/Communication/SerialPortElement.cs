using System.IO.Ports;

namespace DY.NET
{
    public class SerialPortElement
    {
        public string PortName { get; protected set; }
        public int BaudRate { get; protected set; }
        public Parity ParityBit { get; protected set; }
        public int DataBit { get; protected set; }
        public StopBits StopBit { get; protected set; }
    }
}