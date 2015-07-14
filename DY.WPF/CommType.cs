using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF
{
    [Flags]
    public enum CommType
    {
        SERIAL = 1,
        ETHERNET = 2,
    }
}