using System;

namespace DY.NET
{
    /// <summary>
    /// PLC 클라이언트 소켓 인터페이스
    /// </summary>
    public interface ISocketCover : IConnect, IPostAsync
    {
    }
}