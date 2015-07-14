using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF
{
    public enum Device
    {
        LSIS_XGT,
        HONEYWELL_VUQUEST3310G,
        DATALOGIC_MATRIX200
    }
    
    public static class ServiceableDevice
    {
        public static readonly Dictionary<Device, CommType> List = new Dictionary<Device, CommType>
        {
            { Device.LSIS_XGT,                  CommType.SERIAL | CommType.ETHERNET },
            { Device.HONEYWELL_VUQUEST3310G,    CommType.SERIAL },
            { Device.DATALOGIC_MATRIX200,       CommType.SERIAL },
        };
    }
}