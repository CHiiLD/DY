using System;
using System.Collections;

namespace DY.NET.LSIS.XGT
{
    public static class B2W
    {
        /// <summary>
        /// bool[16] 데이터를 UInt16데이터로 변환한다.
        /// </summary>
        /// <param name="bool16">byte[16]</param>
        /// <returns>UInt16</returns>
        public static UInt16 ToUInt16(bool[] bool16)
        {
            if (bool16 == null)
                throw new ArgumentNullException();
            if (bool16.Length > sizeof(UInt16) * 8)
                throw new ArgumentException("bit data is over 16");

            BitArray bitArray = new BitArray(bool16);
            ushort[] ret = new ushort[1] { 0 };
            bitArray.CopyTo(ret, 0);
            return ret[0];
        }

        /// <summary>
        /// UInt16데이터를 byte[16]데이터로 변환한다.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool[] ToBools(UInt16 word)
        {
            bool[] bits = new bool[16];
            for (int i = 0; i < sizeof(UInt16) * 8; i++)
            {
                int v = word & ((UInt16)1 << i);
                bits[i] = v == 0 ? false : true;
            }
            return bits;
        }
    }
}
