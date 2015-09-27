namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet - 통신 흐름 
    /// </summary>
    public enum XGTFEnetStreamDirection : byte
    {
        NONE = 0x00,
        PC2PLC = 0x33,
        PLC2PC = 0x11
    }

    public static class XGTFEnetStreamDirectionExtension
    {
        /// <summary>
        /// XGTFEnetStreamDirection 객체를 byte로 변환한다.
        /// </summary>
        public static byte ToByte(this XGTFEnetStreamDirection direction)
        {
            byte stream = 0;
            switch (direction)
            {
                case XGTFEnetStreamDirection.PC2PLC:
                    stream = (byte)XGTFEnetStreamDirection.PC2PLC;
                    break;
                case XGTFEnetStreamDirection.PLC2PC:
                    stream = (byte)XGTFEnetStreamDirection.PLC2PC;
                    break;
            }
            return stream;
        }
    }
}