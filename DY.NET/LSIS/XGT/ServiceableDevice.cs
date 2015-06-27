using System.Collections.Generic;
using System;

namespace DY.NET.LSIS.XGT
{
    [Flags]
    public enum DeviceMode
    {
        READ = 0x1,
        WRITE = 0x2,
        MONITER = 0x4,
    }

    /// <summary>
    /// 사용가능한 디바이스와 해당 디바이스의 사용가능한 모드의 정보를 처리
    /// </summary>
    public static class ServiceableDevice
    {
        public static Dictionary<char, DeviceMode> GetDeviceInfo()
        {
            Dictionary<char, DeviceMode> info = new Dictionary<char, DeviceMode>();
            info.Add('P', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('M', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('K', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('T', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('C', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('Z', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('S', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('L', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('N', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('D', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER); //XGK-CPUH(0~32767) XGK-CPUS(0~19999)
            info.Add('R', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('F', DeviceMode.READ | DeviceMode.MONITER);
            info.Add('W', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER); //ZR //XGK-CPUH 에서만 사용가능
            return info;
        }
    }
}
