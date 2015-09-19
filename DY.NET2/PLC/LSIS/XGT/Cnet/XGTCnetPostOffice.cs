using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetPostOffice : IPostOffice, IPoster
    {
        public Stream Stream { get; set; }

        protected SerialPort m_SerialPort;

        public XGTCnetPostOffice(SerialPort serialPort)
        {
            m_SerialPort = serialPort;
        }
        
        public virtual async Task<bool> ConnectAsync()
        {
            return false;
        }

        public virtual async Task DisconnectAsync()
        {
        }

        public virtual bool IsConnected()
        {
            return false;
        }

        public virtual async Task<IProtocol> PostAsync(IProtocol protocol)
        {
            return null;
        }
        
        public virtual void Dispose()
        {

        }
    }
}