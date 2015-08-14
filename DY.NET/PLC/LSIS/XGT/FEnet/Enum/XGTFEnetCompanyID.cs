namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet - 프레임 구조 'LSIS-XGT'
    /// 사용설명서_XGT FEnet_국문_V2.0(8.1.2 프레임 구조)
    /// </summary>
    public enum XGTFEnetCompanyID
    {
        NONE,
        LSIS_XGT
    }

    public static class XGTFEnetCompanyIDExtension
    {
        /// <summary>
        /// XGTFEnetCompanyID 변수에 해당하는 ASCII CODE를 반환한다.
        /// </summary>
        /// <param name="id">XGTFEnetCompanyID</param>
        /// <returns>ASCII CODE</returns>
        public static byte[] ToBytes(this XGTFEnetCompanyID id)
        {
            byte[] ascii_code = new byte[8];
            switch(id)
            {
                case XGTFEnetCompanyID.LSIS_XGT:
                    ascii_code[0] = (byte)'L';
                    ascii_code[1] = (byte)'S';
                    ascii_code[2] = (byte)'I';
                    ascii_code[3] = (byte)'S';
                    ascii_code[4] = (byte)'-';
                    ascii_code[5] = (byte)'X';
                    ascii_code[6] = (byte)'G';
                    ascii_code[7] = (byte)'T';
                    break;
            }
            return ascii_code;
        }
    }
}