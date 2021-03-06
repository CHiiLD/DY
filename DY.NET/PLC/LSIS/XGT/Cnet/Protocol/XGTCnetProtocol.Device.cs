﻿using System.Collections.Generic;

namespace DY.NET.LSIS.XGT
{
    public partial class XGTCnetProtocol
    {
        /// <summary>
        /// XGT 디바이스의 XGTCentActionMode(읽기, 쓰기, 모니터) 지원 정보를 얻는다.
        /// </summary>
        /// <returns>key: Device name, value: XGTCentActionMode[Flag]</returns>
        public static Dictionary<char, XGTCentActionMode> CnetServiceableModeDictionary = new Dictionary<char, XGTCentActionMode>
        {
            // XGK
            { 'P', XGTCentActionMode.READ | XGTCentActionMode.WRITE | XGTCentActionMode.MONITER },
            { 'M', XGTCentActionMode.READ | XGTCentActionMode.WRITE | XGTCentActionMode.MONITER },
            { 'K', XGTCentActionMode.READ | XGTCentActionMode.WRITE | XGTCentActionMode.MONITER },
            { 'T', XGTCentActionMode.READ | XGTCentActionMode.WRITE | XGTCentActionMode.MONITER },
            { 'C', XGTCentActionMode.READ | XGTCentActionMode.WRITE | XGTCentActionMode.MONITER },
#if false   
            {'Z', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER},
            {'S', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER},
#endif
            { 'L', XGTCentActionMode.READ | XGTCentActionMode.WRITE | XGTCentActionMode.MONITER },
            { 'N', XGTCentActionMode.READ | XGTCentActionMode.WRITE | XGTCentActionMode.MONITER },
            { 'D', XGTCentActionMode.READ | XGTCentActionMode.WRITE | XGTCentActionMode.MONITER },
            { 'R', XGTCentActionMode.READ | XGTCentActionMode.WRITE | XGTCentActionMode.MONITER },
            { 'F', XGTCentActionMode.READ | XGTCentActionMode.MONITER},
            { 'W', XGTCentActionMode.READ | XGTCentActionMode.WRITE | XGTCentActionMode.MONITER }, //ZR //XGK-CPUH 에서만 사용가능

#if false   // XGI, XGR 이 라이브러리에서는 지원하지 않습니다
            
            { {'I', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER}},
            { {'Q', ServiceableMode.READ | ServiceableMode.WRITE | ServiceableMode.MONITER}},
            { {'U', ServiceableMode.MONITER}},
#endif
        };
    }
}
