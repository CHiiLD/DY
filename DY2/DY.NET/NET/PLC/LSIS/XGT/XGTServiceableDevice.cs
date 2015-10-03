using System.Collections.Generic;
using System;

namespace DY.NET.LSIS.XGT
{
    public static class XGTServiceableDevice
    {
        /// <summary>
        /// Cnet 통신에서 사용할 수 있는 디바이스 목록과 지원 크기{바이트 단위}
        /// </summary>
        /// <returns>key: device name, value: serviceable device memory size{word type}</returns>
        public static Dictionary<char, int> MemoryTerritorySize = new Dictionary<char, int>
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
            { 'D', 32768 }, //단, CPUS 는 20000 까지 
            //{ 'D', 20000 }, //단, CPUS 는 20000 까지 
            { 'R', 32768 },
            { 'W', 65536 },
        };
    }
}