using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    [Flags]
    public enum ServiceableMode
    {
        READ = 0x1,
        WRITE = 0x2,
        MONITER = 0x4,
    }
}
