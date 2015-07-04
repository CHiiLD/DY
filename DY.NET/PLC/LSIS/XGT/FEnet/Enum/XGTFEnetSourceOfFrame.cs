namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// PC -> PLC 통신 / PLC -> PC 통신
    /// </summary>
    public enum XGTFEnetSourceOfFrame : byte
    {
        PC2PLC = 0x33,
        PLC2PC = 0x11
    }

    public static class XGTFEnetSourceOfFrameExtension
    {
        public static byte ToByte(this XGTFEnetSourceOfFrame sf)
        {
            byte ret = 0;
            switch (sf)
            {
                case XGTFEnetSourceOfFrame.PC2PLC:
                    ret = (byte)XGTFEnetSourceOfFrame.PC2PLC;
                    break;
                case XGTFEnetSourceOfFrame.PLC2PC:
                    ret = (byte)XGTFEnetSourceOfFrame.PLC2PC;
                    break;
            }
            return ret;
        }
    }
}