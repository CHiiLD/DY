using System;

namespace DY.NET
{
    public interface ISocketCover : ITag, IDisposable
    {
        bool Connect();
        void Close();
        void Send(IProtocol protocolFrame);
        bool IsOpen();

        EventHandler<DataReceivedEventArgs> SendedProtocolSuccessfully { get; set; }
        EventHandler<DataReceivedEventArgs> ReceivedProtocolSuccessfully { get; set; }

        void SendedProtocolSuccessfullyEvent(IProtocol iProtocol);
        void ReceivedProtocolSuccessfullyEvent(IProtocol iProtocol);
    }
}