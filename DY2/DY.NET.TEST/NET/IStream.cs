using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DY.NET
{
    public interface IStream : IDisposable
    {
        Stream Stream { get; set; }
        Task<bool> OpenAsync();
        Task CloseAsync();
        Task<bool> IsConnected();
        Task<bool> CanCommunicate();
    }
}