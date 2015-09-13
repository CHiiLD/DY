using System;
using System.IO.Ports;

namespace DY.NET.LSIS.XGT
{
    public partial class XGTCnetSocket
    {
        public class Builder : ASerialPortBuilder
        {
            private ushort m_Localport;

            public Builder(string com, int baud, ushort localport)
                : base(com, baud)
            {
                m_Localport = localport;
            }

            public override object Build()
            {
                var xgt_cnet_socket = new XGTCnetSocket()
                {
                    m_SerialPort = new SerialPort(PortName, BaudRate, ParityBit, DataBit, StopBit),
                    LocalPort = m_Localport,
                    Description = "LSIS XGT Cnet(" + PortName + ")"
                };
                return xgt_cnet_socket;
            }
        }   
    }
}
