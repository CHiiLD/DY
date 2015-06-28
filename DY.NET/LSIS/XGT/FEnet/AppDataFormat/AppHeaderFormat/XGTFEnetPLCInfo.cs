namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// FEnet 헤더 구조 중 PLC Info의 데이터 구조체
    /// </summary>
    public struct XGTFEnetPLCInfo
    {
        public XGTFEnetCpuType CpuType; //bit00-05 시피유 타입
        public XGTFEnetClass Class; // 7bit
        public XGTFEnetCpuState CpuState; //8bit 
        public XGTFEnetPLCSystemState PLCState; //bit08-12 시스템 상태
        
        /// <summary>
        /// byte array로 초기화
        /// </summary>
        /// <param name="data"></param>
        public void Init(byte[] data)
        {
            CpuType = (XGTFEnetCpuType)(0xFC & data[0]);
            Class = (XGTFEnetClass)(0x02 & data[0]); ;
            CpuState = (XGTFEnetCpuState)(0x01 & data[0]);
            PLCState = (XGTFEnetPLCSystemState)(0xF8 & data[1]);
        }

        /// <summary>
        /// 셋팅
        /// </summary>
        public void Set(XGTFEnetCpuType cpy_type, XGTFEnetClass clazz, XGTFEnetCpuState cpu_st, XGTFEnetPLCSystemState sys_st)
        {
            CpuType = cpy_type;
            Class = clazz;
            CpuState = cpu_st;
            PLCState = sys_st;
        }

        /// <summary>
        /// byte array로 변환
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)((byte)CpuType | (byte)Class | (byte)CpuState);
            ret[1] = (byte) PLCState;
            return ret;
        }
    }
}
