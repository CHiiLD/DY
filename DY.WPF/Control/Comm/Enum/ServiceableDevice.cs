using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF
{
    /// <summary>
    /// 디바이스 별 통신 방법 정의
    /// </summary>
    public static class ServiceableDevice
    {
        public static readonly Dictionary<CommDevice, CommType> List = new Dictionary<CommDevice, CommType>
        {
            { CommDevice.LSIS_XGT,                  CommType.SERIAL | CommType.ETHERNET },
            { CommDevice.HONEYWELL_VUQUEST3310G,    CommType.SERIAL },
            { CommDevice.DATALOGIC_MATRIX200,       CommType.SERIAL },
        };
    }
}