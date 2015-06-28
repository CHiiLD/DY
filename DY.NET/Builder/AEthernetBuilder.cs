using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace DY.NET
{
    public abstract class AEthernetBuilder
    {
        protected string _Host;
        protected ProtocolType _ProtocolType = System.Net.Sockets.ProtocolType.Tcp;
        protected AddressFamily _AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork;
        protected SocketType _SocketType = System.Net.Sockets.SocketType.Stream;
        protected int _Port;

        public AEthernetBuilder(string host, int port)
        {
            _Host = host;
            _Port = port;
        }

        public AEthernetBuilder ProtocolType(ProtocolType type)
        {
            _ProtocolType = type;
            return this;
        }

        public AEthernetBuilder AddressFamily(AddressFamily addr)
        {
            _AddressFamily = addr;
            return this;
        }

        public AEthernetBuilder Stream(SocketType skt_type)
        {
            _SocketType = skt_type;
            return this;
        }

        public abstract object Build();
    }
}
