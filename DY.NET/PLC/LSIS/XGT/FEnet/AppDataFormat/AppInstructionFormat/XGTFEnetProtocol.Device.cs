using System.Collections.Generic;

namespace DY.NET.LSIS.XGT
{
    public partial class XGTFEnetProtocol
    {
        /// <summary>
        /// XGT FEnet 통신에서 RSB 통신을 할 때 사용할 수 있는 디바이스 목록
        /// </summary>
        public static readonly List<char> UsableRSBDeviceNameList = new List<char>
        {
            { 'P' }, 
            { 'M' }, 
            { 'L' }, 
            { 'F' }, 
            { 'C' }, 
            { 'D' }, 
            { 'T' }, 
            { 'N' }, 
            { 'R' }, 
        };

        /// <summary>
        /// XGT FEnet 통신에서 WSB 통신을 할 때 사용할 수 있는 디바이스 목록
        /// </summary>
        public static readonly List<char> UsableWSBDeviceNameList = new List<char>
        {
            { 'P' }, 
            { 'M' }, 
            { 'K' }, 
            { 'C' }, 
            { 'D' }, 
            { 'T' }, 
            { 'N' }, 
            { 'R' }, 
        };
    }
}