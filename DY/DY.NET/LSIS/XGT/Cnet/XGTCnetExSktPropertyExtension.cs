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
            return Serial.PortName;
        }

        public int GetBaudRate()
        {
            return Serial.BaudRate;
        }

        public Parity GetParity()
        {
            return Serial.Parity;
        }

        public int GetDataBits()
        {
            return Serial.DataBits;
        }

        public StopBits GetStopBits()
        {
            return Serial.StopBits;
        }
    }
}
