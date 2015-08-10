using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface ITimeout
    {
        int WriteTimeout { get; set; }
        int ReadTimeout { get; set; }
    }
}