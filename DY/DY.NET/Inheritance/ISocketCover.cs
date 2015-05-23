using System;

namespace DY.NET
{
    public interface ISocketCover : ITag, IDisposable
    {
        bool Connect();
        bool Close();
        void Send(IProtocol protocolFrame);
        bool IsOpen();
    }
}
