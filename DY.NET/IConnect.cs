using System;

namespace DY.NET
{
    /// <summary>
    /// 클라이언트 통신을 위한 인터페이스
    /// </summary>
    public interface IConnect : IDisposable, ITag, IStream
    {
        bool Connect();
        void Close();
        bool IsConnected();
        EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged { get; set; }
    }
}