using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF.CTRL.COMM
{
    public enum CommDeviceType
    {
        XGT,
        MATRIX200,
        VUQUEST3310G
    }

    public static class CommDeviceTypeExtension
    {
        public static string ToString(this CommDeviceType type)
        {
            string ret = "";
            switch (type)
            {
                case CommDeviceType.XGT:
                    ret = "LSIS XGT";
                    break;
                case CommDeviceType.MATRIX200:
                    ret = "Datalogic Matrix200";
                    break;
                case CommDeviceType.VUQUEST3310G:
                    ret = "Honeywell Vuquest3310g";
                    break;
            }
            return ret;
        }
    }
}