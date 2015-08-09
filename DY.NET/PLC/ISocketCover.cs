using System;

namespace DY.NET
{
    /// <summary>
    /// PLC 클라이언트 소켓 인터페이스
    /// </summary>
    public interface ISocketCover : ITag, IConnect
    {
        void Send(IProtocol protocol);
        EventHandler<ProtocolReceivedEventArgs> SendedProtocolSuccessfully { get; set; }
        EventHandler<ProtocolReceivedEventArgs> ReceivedProtocolSuccessfully { get; set; }
        EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged { get; set; }
        
        void SendedProtocolSuccessfullyEvent(IProtocol protocol);
        void ReceivedProtocolSuccessfullyEvent(IProtocol protocol);
        void ConnectionStatusChangedEvent(bool isConnected);
    }
}