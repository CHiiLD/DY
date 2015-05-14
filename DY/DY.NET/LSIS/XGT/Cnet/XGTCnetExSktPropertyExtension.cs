using System.IO.Ports;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// partial 기능을 사용하여 프로퍼티 리턴 함수 선언
    /// </summary>
    public partial class XGTCnetExclusiveSocket
    {
        public string GetPortName()
        {
            return _SerialPort.PortName;
        }

        public int GetBaudRate()
        {
            return _SerialPort.BaudRate;
        }

        public Parity GetParity()
        {
            return _SerialPort.Parity;
        }

        public int GetDataBits()
        {
            return _SerialPort.DataBits;
        }

        public StopBits GetStopBits()
        {
            return _SerialPort.StopBits;
        }
    }
}
