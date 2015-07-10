using System;

namespace DY.NET
{
    public interface ISocketCover : ITag, IDisposable
    {
        bool Connect();
        void Close();
        void Send(IProtocol protocol);
        bool IsConnected();

        EventHandler<DataReceivedEventArgs> SendedProtocolSuccessfully { get; set; }
        EventHandler<DataReceivedEventArgs> ReceivedProtocolSuccessfully { get; set; }

        void SendedProtocolSuccessfullyEvent(IProtocol protocol);
        void ReceivedProtocolSuccessfullyEvent(IProtocol protocol);
    }
}