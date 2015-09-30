using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IProtocol
    {
        IList<IProtocolItem> Items { get; set; }
        Type ItemType { get; set; }

        void Initialize();
        int GetErrorCode();
    }
}