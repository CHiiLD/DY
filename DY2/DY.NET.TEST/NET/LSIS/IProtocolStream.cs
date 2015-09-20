using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IProtocolStream : IStream
    {
        Task<IProtocol> SendAsync(IProtocol protocol);
    }
}