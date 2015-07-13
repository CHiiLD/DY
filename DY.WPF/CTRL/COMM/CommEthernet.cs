using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace DY.WPF.CTRL.COMM
{
    public struct CommEthernet
    {
        ushort Port { get; set; }
        string IP { get; set; }
        ProtocolType Type { get; set; }
    }
}