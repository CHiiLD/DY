using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface ITimeout
    {
        int ReceiveTimeout { get; set; }
        int SendTimeout { get; set; }
        int OpenTimeout { get; set; }
    }
}