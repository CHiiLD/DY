using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public class MCProtocol : IProtocol
    {
        public IList<IProtocolData> Data { get; set; }
        public Type Type { get; set; }

        ControlChar Header { get; set; }
        ushort LocalPort { get; set; }
        ushort PCNumber { get; set; }
        MCCommand Command { get; set; }
        byte WaitTime { get; set; }
        ushort BCC { get; set; }
        ushort Error { get; set; }

        public void Initialize()
        {

        }

        public int GetErrorCode()
        {
            return -1;
        }
    }
}