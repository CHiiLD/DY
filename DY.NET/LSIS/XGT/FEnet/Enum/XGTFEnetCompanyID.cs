namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// LSIS XGT 표기
    /// </summary>
    public enum XGTFEnetCompanyID
    {
        LSIS_XGT
    }

    public static class XGTFEnetCompanyIDExtension
    {
        public static byte[] ToByteArray(this XGTFEnetCompanyID id)
        {
            byte[] ret = new byte[8];
            switch(id)
            {
                case XGTFEnetCompanyID.LSIS_XGT:
                    ret[0] = (byte)'L';
                    ret[1] = (byte)'S';
                    ret[2] = (byte)'I';
                    ret[3] = (byte)'S';
                    ret[4] = (byte)'-';
                    ret[5] = (byte)'X';
                    ret[6] = (byte)'G';
                    ret[7] = (byte)'T';
                    break;
            }
            return ret;
        }
    }
}