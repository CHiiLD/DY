namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet - 프레임 구조 'LSIS-XGT'
    /// 사용설명서_XGT FEnet_국문_V2.0(8.1.2 프레임 구조)
    /// </summary>
    public enum XGTFEnetCompanyID
    {
        NONE = 0,
        LSIS_XGT
    }

    public static class XGTFEnetCompanyIDExtension
    {
        /// <summary>
        /// XGTFEnetCompanyID 변수에 해당하는 ASCII CODE를 반환한다.
        /// </summary>
        /// <param name="comID">XGTFEnetCompanyID</param>
        /// <returns>ASCII CODE</returns>
        public static byte[] ToBytes(this XGTFEnetCompanyID comID)
        {
            byte[] code = new byte[8];
            switch(comID)
            {
                case XGTFEnetCompanyID.LSIS_XGT:
                    code[0] = (byte)'L';
                    code[1] = (byte)'S';
                    code[2] = (byte)'I';
                    code[3] = (byte)'S';
                    code[4] = (byte)'-';
                    code[5] = (byte)'X';
                    code[6] = (byte)'G';
                    code[7] = (byte)'T';
                    break;
            }
            return code;
        }
    }
}