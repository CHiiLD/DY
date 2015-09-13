using System.Net.Sockets;

namespace DY.NET
{
    public class EthernetElement
    {
        public string Host { get; protected set; }
        public int Port { get; protected set; }
        public ProtocolType Type { get; protected set; }
    }
}