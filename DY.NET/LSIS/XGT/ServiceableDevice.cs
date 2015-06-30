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
        /// <summary>
        /// 각 디바이스 별 읽기 쓰기 모니터 가능 정보를 얻는다
        /// </summary>
        /// <returns></returns>
        public static Dictionary<char, DeviceMode> GetCnetDeviceMotionDictionary()
        {
            Dictionary<char, DeviceMode> info = new Dictionary<char, DeviceMode>();
            // XGK 
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

            // XGI, XGR

            info.Add('I', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('Q', DeviceMode.READ | DeviceMode.WRITE | DeviceMode.MONITER);
            info.Add('U', DeviceMode.MONITER);

            return info;
        }

        /// <summary>
        /// Cnet 통신에서 사용할 수 있는 디바이스 목록
        /// </summary>
        /// <returns></returns>
        public static Dictionary<char, int> GetCnetServiceableDeviceDictionary()
        {
            Dictionary<char, int> info = new Dictionary<char, int>();
            const int I = 2;
            //바이트 단위로 계산
            info.Add('P', 2048 * I);
            info.Add('M', 2048 * I);
            info.Add('K', 2048 * I);
            info.Add('F', 2048 * I);
            info.Add('T', 2048 * I);
            info.Add('C', 2048 * I);
            info.Add('Z', 128 * I);
            info.Add('S', 128 * I);
            info.Add('L', 11264 * I);
            info.Add('N', 21504 * I);
            info.Add('D', 32768 * I); //단, CPUS 는 20000만 까지 
            info.Add('R', 32768 * I);
            info.Add('W', 65536 * I);
            return info;
        }

        public static List<char> GetFEnetSBProtocolServiceableDeviceList()
        {
            List<char> list = new List<char>();
            list.Add('P');
            list.Add('N');
            list.Add('N');
            list.Add('K');
            list.Add('T');
            list.Add('C');
            list.Add('D');
            list.Add('N');
            list.Add('F');
            return list;
        }

#if false
        public static List<char> GetFEnetSSProtocolServiceableDeviceList(Type type)
        {
            List<char> list = new List<char>();
            list.Add('P');
            list.Add('N');
            list.Add('N');
            list.Add('K');
            list.Add('T');
            list.Add('C');
            list.Add('D');
            list.Add('N');
            list.Add('F');
            return list;
        }
#endif
    }
}
