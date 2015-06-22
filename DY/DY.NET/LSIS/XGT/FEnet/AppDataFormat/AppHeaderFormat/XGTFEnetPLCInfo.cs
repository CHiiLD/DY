using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// FEnet 헤더 구조 중 PLC Info의 데이터 구조체
    /// </summary>
    public struct XGTFEnetPLCInfo
    {
        public CpuType Cpu_Type; //bit00-05 시피유 타입
        public bool IsSlave; // bit06 -> 1인 경우 슬레이브, 0인 경우 마스터 or 단독
        public bool IsError; // bit07 -> 1인 경우 동작에러, 0인 경우 동작정상
        public XGTFEnetPLCSystemState State; //bit08-12 시스템 상태

        internal void Init(byte[] data)
        {
            byte flag_slave = 0x2; // 0000010
            byte flag_error = 0x1; // 0000001

            IsSlave = (data[0] & flag_slave) == flag_slave;
            IsError = (data[0] & flag_error) == flag_error;

            Cpu_Type = (CpuType)(data[0] >> 2);
            State = (XGTFEnetPLCSystemState)(data[1] >> 3);
        }
    }
}
