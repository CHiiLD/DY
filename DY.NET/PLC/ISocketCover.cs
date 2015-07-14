using System;

namespace DY.NET
{
    public interface ISocketCover : ITag, IConnect
    {
        void Send(IProtocol protocol);
        EventHandler<ProtocolReceivedEventArgs> SendedProtocolSuccessfully { get; set; }
        EventHandler<ProtocolReceivedEventArgs> ReceivedProtocolSuccessfully { get; set; }

        void SendedProtocolSuccessfullyEvent(IProtocol protocol);
        void ReceivedProtocolSuccessfullyEvent(IProtocol protocol);
    }
}