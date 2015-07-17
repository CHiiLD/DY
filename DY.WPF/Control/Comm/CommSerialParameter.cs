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
    public class CommSerialParameter : ISummaryParameter
    {
        public string Com { get; set; }
        public int Bandrate { get; set; }
        public Parity Parity { get; set; }
        public int DataBit { get; set; }
        public StopBits StopBit { get; set; }

        public string GetParameterSummaryString()
        {
            StringBuilder sb = new StringBuilder(Com);
            sb.Append(' ');
            sb.Append(Bandrate.ToString());
            sb.Append('-');
            sb.Append(DataBit.ToString());
            sb.Append('-');
            sb.Append(Parity.ToString()[0]);
            sb.Append('-');
            sb.Append(((int)StopBit).ToString());
            return sb.ToString();
        }
    }
}