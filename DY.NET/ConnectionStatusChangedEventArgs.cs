using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; private set; }
        public ConnectionStatusChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}