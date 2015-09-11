using System;

namespace DY.NET
{
    /// <summary>
    /// IConnect.ConnectionStatusChanged 아규먼트
    /// </summary>
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; private set; }

        public ConnectionStatusChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}