using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IProtocolItem
    {
        string Address { get; set; }
        object Value { get; set; }
    }
}