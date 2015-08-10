using System;

namespace DY.NET
{
    /// <summary>
    /// PLC 클라이언트 소켓 인터페이스
    /// </summary>
    public interface ISocketCover : IConnect
    {
        void Send(IProtocol protocol);
        EventHandler<ProtocolReceivedEventArgs> SendedProtocolSuccessfully { get; set; }
        EventHandler<ProtocolReceivedEventArgs> ReceivedProtocolSuccessfully { get; set; }
    }
}