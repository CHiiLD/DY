using System;

namespace DY.NET
{
    public interface ISocketCover : ITag, IDisposable
    {
        bool Connect();
        void Close();
        void Send(IProtocol protocolFrame);
        bool IsOpen();
    }
}
