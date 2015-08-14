using System;
using System.IO.Ports;

namespace DY.NET.LSIS.XGT
{
    public partial class XGTCnetSocket
    {
        public class Builder : ASerialPortBuilder
        {
            public Builder(string name, int baud)
                : base(name, baud)
            {
            }

            public override object Build()
            {
                var xgt_cnet_socket = new XGTCnetSocket()
                {
                    m_SerialPort = new SerialPort(PortName, BaudRate, Parity_, DataBit, StopBit),
                    Description = "LSIS XGT Cnet(" + PortName + ")"
                };
                return xgt_cnet_socket;
            }
        }
    }
}
