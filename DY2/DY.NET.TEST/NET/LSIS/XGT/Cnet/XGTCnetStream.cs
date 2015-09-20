using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetStream : SerialPort, IProtocolStream
    {
        public int ReceiveTimeout { get; set; }
        public int SendTimeout { get; set; }

        public Stream GetStream()
        {
            return null;
        }

        public XGTCnetStream(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {

        }

        public async Task<bool> OpenAsync()
        {
            return false;
        }

        public async Task CloseAsync()
        {
        }

        public bool IsOpend()
        {
            return false;
        }

        public async Task<IProtocol> SendAsync(IProtocol protocol)
        {
            return null;
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
