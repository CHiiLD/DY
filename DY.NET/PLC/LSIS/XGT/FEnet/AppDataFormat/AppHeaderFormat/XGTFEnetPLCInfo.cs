namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet - 헤더 구조 중 PLC Info의 데이터 구조체
    /// </summary>
    public class XGTFEnetPLCInfo
    {
        public XGTFEnetCpuType CpuType { get; private set; } //bit00-05 시피유 타입
        public XGTFEnetClass Class { get; private set; } // 7bit
        public XGTFEnetCpuState CpuState { get; private set; } //8bit 
        public XGTFEnetPLCSystemState PLCState { get; private set; } //bit08-12 시스템 상태

        /// <summary>
        /// 복사생성자
        /// </summary>
        /// <param name="that">복사 타겟</param>
        public XGTFEnetPLCInfo(XGTFEnetPLCInfo that)
        {
            CpuType = that.CpuType;
            Class = that.Class;
            CpuState = that.CpuState;
            PLCState = that.PLCState;
        }

        /// <summary>
        /// 생성자
        /// 응답 프로토콜
        /// </summary>
        /// <param name="data"></param>
        public XGTFEnetPLCInfo(byte[] data)
        {
            CpuType = (XGTFEnetCpuType)(0xFC & data[0]);
            Class = (XGTFEnetClass)(0x02 & data[0]);
            CpuState = (XGTFEnetCpuState)(0x01 & data[0]);
            PLCState = (XGTFEnetPLCSystemState)(0x1F & data[1]);
        }

        /// <summary>
        /// 생성자
        /// 요청 프로토콜 
        /// </summary>
        public XGTFEnetPLCInfo(XGTFEnetCpuType cpy_type, XGTFEnetClass clazz, XGTFEnetCpuState cpu_st, XGTFEnetPLCSystemState sys_st)
        {
            CpuType = cpy_type;
            Class = clazz;
            CpuState = cpu_st;
            PLCState = sys_st;
        }

        /// <summary>
        /// XGTFEnetPLCInfo 데이터를 byte[]로 변환한다.
        /// </summary>
        /// <returns>2byte data</returns>
        public byte[] ToByteArray()
        {
            byte[] ret = new byte[2];
            ret[0] = (byte)((byte)CpuType | (byte)Class | (byte)CpuState);
            ret[1] = (byte)PLCState;
            return ret;
        }
    }
}