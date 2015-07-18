using System;

namespace DY.NET
{
    public interface IConnect : IDisposable
    {
        bool Connect();
        void Close();
        bool IsConnected();
        EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged { get; set;}
    }
}