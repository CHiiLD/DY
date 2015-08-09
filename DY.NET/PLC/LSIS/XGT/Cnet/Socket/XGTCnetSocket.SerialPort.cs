using System;
using System.IO.Ports;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// partial 기능을 사용하여 프로퍼티 리턴 함수 선언
    /// </summary>
    public sealed partial class XGTCnetSocket
    {
        public class Builder : ASerialPortBuilder
        {
            public Builder(string name, int baud)
                : base(name, baud)
            {
            }

            public override object Build()
            {
                var skt = new XGTCnetSocket() { m_SerialPort = new SerialPort(PortName, BaudRate, Parity_, DataBit, StopBit) };
                skt.m_SerialPort.DataReceived += skt.OnDataRecieve;
                skt.m_SerialPort.ErrorReceived += skt.OnSerialErrorReceived;
                skt.m_SerialPort.PinChanged += skt.OnSerialPinChanged;
                return skt;
            }
        }

        public string GetPortName()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIAL_IS_NULL);
            return m_SerialPort.PortName;
        }

        public int GetBaudRate()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIAL_IS_NULL);
            return m_SerialPort.BaudRate;
        }

        public Parity GetParity()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIAL_IS_NULL);
            return m_SerialPort.Parity;
        }

        public int GetDataBits()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIAL_IS_NULL);
            return m_SerialPort.DataBits;
        }

        public StopBits GetStopBits()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIAL_IS_NULL);
            return m_SerialPort.StopBits;
        }
    }
}
