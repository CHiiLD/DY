using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DY.NET
{
    public interface IPostOffice : IDisposable
    {
        Stream Stream { get; set; }
        Task<bool> ConnectAsync();
        Task DisconnectAsync();
        bool IsConnected();
    }
}