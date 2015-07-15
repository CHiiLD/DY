using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace DY.WPF
{
    /// <summary>
    /// SerialPort 설정 옵션
    /// </summary>
    public struct CommSerialStruct
    {
        public string Com { get; set; }
        public int Bandrate { get; set; }
        public Parity Parity { get; set; }
        public int DataBit { get; set; }
        public StopBits StopBit { get; set; }
    }
}