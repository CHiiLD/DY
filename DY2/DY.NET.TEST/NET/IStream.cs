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
        Stream GetStream();
        bool IsOpend();
        Task OpenAsync();
        Task CloseAsync();
    }
}