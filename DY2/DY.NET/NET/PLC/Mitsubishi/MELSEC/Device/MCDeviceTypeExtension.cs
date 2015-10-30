using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public static class MCDeviceTypeExtension
    {
        public static DeviceType GetDeviceType(this MCDevice device)
        {
            switch (device)
            {
                case MCDevice.SM: return DeviceType.BIT;
                case MCDevice.SD: return DeviceType.WORD;
                case MCDevice.X: return DeviceType.BIT;
                case MCDevice.Y: return DeviceType.BIT;
                case MCDevice.M: return DeviceType.BIT;
                case MCDevice.L: return DeviceType.BIT;
                case MCDevice.F: return DeviceType.BIT;
                case MCDevice.V: return DeviceType.BIT;
                case MCDevice.B: return DeviceType.BIT;
                case MCDevice.D: return DeviceType.WORD;
                case MCDevice.W: return DeviceType.WORD;
                case MCDevice.TS: return DeviceType.BIT;
                case MCDevice.TC: return DeviceType.BIT;
                case MCDevice.TN: return DeviceType.WORD;
                case MCDevice.SS: return DeviceType.BIT;
                case MCDevice.SC: return DeviceType.BIT;
                case MCDevice.SN: return DeviceType.WORD;
                case MCDevice.CS: return DeviceType.BIT;
                case MCDevice.CC: return DeviceType.BIT;
                case MCDevice.CN: return DeviceType.WORD;
                case MCDevice.SB: return DeviceType.BIT;
                case MCDevice.SW: return DeviceType.WORD;
                case MCDevice.S: return DeviceType.BIT;
                case MCDevice.DX: return DeviceType.BIT;
                case MCDevice.DY: return DeviceType.BIT;
                case MCDevice.Z: return DeviceType.WORD;
                case MCDevice.R: return DeviceType.WORD;
                case MCDevice.ZR: return DeviceType.WORD;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
