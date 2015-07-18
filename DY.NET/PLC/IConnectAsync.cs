
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    /// <summary>
    /// 비동기 소켓 접속 추상화 인터페이스
    /// 주로 TCP통신 등에 쓰임
    /// </summary>
    public interface IConnectAsync
    {
        Task<bool> ConnectAsync();
    }
}
