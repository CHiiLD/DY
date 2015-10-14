using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace DY.NET
{
    public interface INotifyProtocolDataChanged : IProtocolData, INotifyCollectionChanged
    {
        Type Type { get; set; }
    }
}