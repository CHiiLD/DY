using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface ITimeout
    {
        int ReceiveingTimeout { get; set; }
        int SendingTimeout { get; set; }
        int ConnectingTimeout { get; set; }
    }
}