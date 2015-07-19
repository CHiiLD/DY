using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF
{
    /// <summary>
    /// 통신 종류
    /// </summary>
    //[Flags]
    public enum DYDeviceProtocolType
    {
        SERIAL = 1,
        ETHERNET = 2,
    }
}