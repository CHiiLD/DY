using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetStream : IProtocolStream
    {
        public Stream Stream { get; set; }
        public SerialPort SerialPort { get; protected set; }

        public XGTCnetStream(SerialPort serialPort)
        {
            SerialPort = serialPort;
        }

        public async Task<bool> OpenAsync()
        {
            return false;
        }

        public async Task CloseAsync()
        {
        }

        public async Task<bool> IsConnected()
        {
            return false;
        }

        public async Task<bool> CanCommunicate()
        {
            return false;
        }

        public async Task<IProtocol> SendAsync(IProtocol protocol)
        {
            return null;
        }

        public void Dispose()
        {

        }
    }
}
