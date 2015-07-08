﻿using System.Collections.Generic;
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
        public static Dictionary<char, ServiceableMode> GetCnetServiceableModeDictionary()
        {
            Dictionary<char, ServiceableMode> info = new Dictionary<char, ServiceableMode>();
            // XGK 
            info.Add('P', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            info.Add('M', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            info.Add('K', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            info.Add('T', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            info.Add('C', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            //info.Add('Z', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            //info.Add('S', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            info.Add('L', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            info.Add('N', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            info.Add('D', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            info.Add('R', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            info.Add('F', ServiceableMode.READ | ServiceableMode.MONITER);
            info.Add('W', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER); //ZR //XGK-CPUH 에서만 사용가능

            // XGI, XGR 이 라이브러리에서는 지원하지 않습니다
            //info.Add('I', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            //info.Add('Q', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER);
            //info.Add('U', ServiceableMode.MONITER);
            return info;
        }

        /// <summary>
        /// Cnet 통신에서 사용할 수 있는 디바이스 목록과 지원 크기(바이트 단위)
        /// </summary>
        /// <returns>key: device name, value: serviceable device memory size(word type)</returns>
        public static Dictionary<char, int> GetMemSizeDictionary(XGKCpuType cpu_type)
        {
            Dictionary<char, int> info = new Dictionary<char, int>();
            //바이트 단위로 계산
            info.Add('P', 2048);
            info.Add('M', 2048);
            info.Add('K', 2048);
            info.Add('F', 2048); //쓰기는 1025워드부터 가능
            info.Add('T', 2048);
            info.Add('C', 2048);
            info.Add('L', 11264);
            info.Add('N', 21504);
            //if (cpu_type == XGTFEnetCpuType.CPUE)
            if (cpu_type == XGKCpuType.XGK_CPUH || cpu_type == XGKCpuType.XGK_CPUA)
                info.Add('D', 32768); //단, CPUS 는 20000 까지 
            else
                info.Add('D', 20000); //단, CPUS 는 20000 까지 
            info.Add('R', 32768);
            info.Add('W', 65536);
            return info;
        }
    }
}