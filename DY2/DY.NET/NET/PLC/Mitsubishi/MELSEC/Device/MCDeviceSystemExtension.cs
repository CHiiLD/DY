using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public static class MCDeviceSystemExtension
    {
        public static DeviceSystem GetDeviceSystem(this MCDevice device)
        {
            switch (device)
            {
                case MCDevice.SM: return DeviceSystem.HEX;
                case MCDevice.SD: return DeviceSystem.HEX;
                case MCDevice.X: return DeviceSystem.HEX;
                case MCDevice.Y: return DeviceSystem.HEX;
                case MCDevice.M: return DeviceSystem.DEC;
                case MCDevice.L: return DeviceSystem.DEC;
                case MCDevice.F: return DeviceSystem.DEC;
                case MCDevice.V: return DeviceSystem.DEC;
                case MCDevice.B: return DeviceSystem.HEX;
                case MCDevice.D: return DeviceSystem.DEC;
                case MCDevice.W: return DeviceSystem.HEX;
                case MCDevice.TS: return DeviceSystem.DEC;
                case MCDevice.TC: return DeviceSystem.DEC;
                case MCDevice.TN: return DeviceSystem.DEC;
                case MCDevice.SS: return DeviceSystem.DEC;
                case MCDevice.SC: return DeviceSystem.DEC;
                case MCDevice.SN: return DeviceSystem.DEC;
                case MCDevice.CS: return DeviceSystem.DEC;
                case MCDevice.CC: return DeviceSystem.DEC;
                case MCDevice.CN: return DeviceSystem.DEC;
                case MCDevice.SB: return DeviceSystem.HEX;
                case MCDevice.SW: return DeviceSystem.HEX;
                case MCDevice.S: return DeviceSystem.DEC;
                case MCDevice.DX: return DeviceSystem.HEX;
                case MCDevice.DY: return DeviceSystem.HEX;
                case MCDevice.Z: return DeviceSystem.DEC;
                case MCDevice.R: return DeviceSystem.DEC;
                case MCDevice.ZR: return DeviceSystem.HEX;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
