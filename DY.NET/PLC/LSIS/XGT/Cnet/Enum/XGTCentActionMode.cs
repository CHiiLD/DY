using System;

namespace DY.NET.LSIS.XGT
{
    [Flags]
    public enum XGTCentActionMode
    {
        READ = 0x1,
        WRITE = 0x2,
        MONITER = 0x4,
    }
}
