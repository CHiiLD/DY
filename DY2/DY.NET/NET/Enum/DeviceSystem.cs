using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    [Flags]
    public enum DeviceSystem
    {
        BIN = 1 << 0, //2
        OCT = 1 << 1, //8
        DEC = 1 << 2, //10
        DUO = 1 << 3, //12
        HEX = 1 << 4//16
    }
}