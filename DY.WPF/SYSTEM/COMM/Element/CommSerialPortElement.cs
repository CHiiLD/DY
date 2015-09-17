using System.Text;
using System.IO.Ports;
using DY.NET;

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// SerialPort 설정 옵션
    /// </summary>
    public class CommSerialPortElement : SerialPortElement, ISummary
    {
        public ushort LocalPort { get; set; }

        public CommSerialPortElement(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            SetSerialPortElement(portName, baudRate, parity, dataBits, stopBits);
        }

        public CommSerialPortElement(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, ushort localPort)
        {
            SetSerialPortElement(portName, baudRate, parity, dataBits, stopBits);
            LocalPort = localPort;
        }

        public CommSerialPortElement(SerialPortElement element)
        {
            SetSerialPortElement(element.PortName, element.BaudRate, element.ParityBit, element.DataBit, element.StopBit);
        }

        public CommSerialPortElement(SerialPortElement element, ushort localPort)
        {
            SetSerialPortElement(element.PortName, element.BaudRate, element.ParityBit, element.DataBit, element.StopBit);
            LocalPort = localPort;
        }

        private void SetSerialPortElement(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            PortName = portName;
            BaudRate = baudRate;
            ParityBit = parity;
            DataBit = dataBits;
            StopBit = stopBits;
        }

        public string GetSummary()
        {
            StringBuilder sb = new StringBuilder(PortName);
            sb.Append(' ');
            sb.Append(BaudRate.ToString());
            sb.Append('-');
            sb.Append(DataBit.ToString());
            sb.Append('-');
            sb.Append(ParityBit.ToString()[0]); 
            sb.Append('-');
            sb.Append(((int)StopBit).ToString());
            return sb.ToString();
        }
    }
}