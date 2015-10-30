using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    [Flags]
    public enum DeviceType
    {
        BIT = 1 << 0, 
        WORD = 1 << 1
    }
}