using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// LSIS PLC CPU 종류
    /// </summary>
    public enum XGTFEnetCpuInfo : byte
    {
        XGK = 0xA0,
        XGI = 0xA4,
        XGR = 0xA8,
    }
}