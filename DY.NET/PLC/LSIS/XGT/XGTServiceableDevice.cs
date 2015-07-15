using System.Collections.Generic;
using System;

namespace DY.NET.LSIS.XGT
{
    [Flags]
    public enum ServiceableMode
    {
        READ = 0x1,
        WRITE = 0x2,
        MONITER = 0x4,
    }

    /// <summary>
    /// 사용가능한 디바이스와 해당 디바이스의 사용가능한 모드의 정보를 처리
    /// </summary>
    public static class XGTServiceableDevice
    {
        /// <summary>
        /// 각 디바이스 별 읽기 쓰기 모니터 가능 정보를 얻는다
        /// </summary>
        /// <returns>key: Device name, value: serviceable device mode</returns>

        public static Dictionary<char, ServiceableMode> CnetServiceableModeDictionary = new Dictionary<char, ServiceableMode>
        {
            // XGK 
            { 'P', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER },
            { 'M', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER },
            { 'K', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER },
            { 'T', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER },
            { 'C', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER },
            //{ {'Z', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER}},
            //{ {'S', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER}},
            { 'L', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER },
            { 'N', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER },
            { 'D', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER },
            { 'R', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER },
            { 'F', ServiceableMode.READ | ServiceableMode.MONITER},
            { 'W', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER }, //ZR //XGK-CPUH 에서만 사용가능

            // XGI, XGR 이 라이브러리에서는 지원하지 않습니다
            //{ {'I', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER}},
            //{ {'Q', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER}},
            //{ {'U', ServiceableMode.MONITER}},
        };

        /// <summary>
        /// Cnet 통신에서 사용할 수 있는 디바이스 목록과 지원 크기{바이트 단위}
        /// </summary>
        /// <returns>key: device name, value: serviceable device memory size{word type}</returns>
        public static Dictionary<char, int> MemSizeDictionary = new Dictionary<char, int>
        {
            //바이트 단위로 계산
            { 'P', 2048 },
            { 'M', 2048 },
            { 'K', 2048 },
            { 'F', 2048 }, //쓰기는 1025워드부터 가능
            { 'T', 2048 },
            { 'C', 2048 },
            { 'L', 11264 },
            { 'N', 21504 },
            //{ {'D', 32768}}, //단, CPUS 는 20000 까지 
            { 'D', 20000 }, //단, CPUS 는 20000 까지 
            { 'R', 32768 },
            { 'W', 65536 },
        };
    }
}