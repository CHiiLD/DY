using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    interface ISocketCoverAsync
    {
        Task<bool> ConnectAsync();
        Task<object> SendAsync(IProtocol protocol);
        void Close();
        bool IsConnected();
    }
}
