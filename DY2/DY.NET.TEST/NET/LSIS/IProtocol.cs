using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IProtocol
    {
        int GetErrorCode();
        IList<IProtocolData> Items { get; set; }
        void Initialize();
    }
}