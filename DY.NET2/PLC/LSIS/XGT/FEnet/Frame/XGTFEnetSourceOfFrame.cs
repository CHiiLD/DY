namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet - 통신 흐름 
    /// </summary>
    public enum XGTFEnetSourceOfFrame : byte
    {
        NONE = 0x00,
        PC2PLC = 0x33,
        PLC2PC = 0x11
    }

    public static class XGTFEnetSourceOfFrameExtension
    {
        /// <summary>
        /// XGTFEnetSourceOfFrame 객체를 byte로 변환한다.
        /// </summary>
        /// <param name="srcOfFrame">XGTFEnetSourceOfFrame</param>
        /// <returns>byte</returns>
        public static byte ToByte(this XGTFEnetSourceOfFrame srcOfFrame)
        {
            byte stream = 0;
            switch (srcOfFrame)
            {
                case XGTFEnetSourceOfFrame.PC2PLC:
                    stream = (byte)XGTFEnetSourceOfFrame.PC2PLC;
                    break;
                case XGTFEnetSourceOfFrame.PLC2PC:
                    stream = (byte)XGTFEnetSourceOfFrame.PLC2PC;
                    break;
            }
            return stream;
        }
    }
}