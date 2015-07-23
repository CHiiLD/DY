using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// RSB, WSB 프로토콜에서 사용 가능한 디바이스 목록 관리
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class XGTFEnetProtocol
    {
        public static readonly List<char> PossibleRSBList = new List<char>
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

        public static readonly List<char> PossibleWSBList = new List<char>
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