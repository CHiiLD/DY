using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;
using DY.NET.LSIS.XGT;

namespace DY.NET.Mitsubishi.MELSEC
{
    public class MCSerialModuleStream : XGTCnetStream
    {
        public MCSerialModuleStream(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            Compressor = new MCCompressor();
        }
    }
}