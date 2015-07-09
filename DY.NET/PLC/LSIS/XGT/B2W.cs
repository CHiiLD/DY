using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// Bit 16 -> Word
    /// Word -> Bit 16
    /// 변환 클래스
    /// </summary>
    public static class B2W
    {
        public static UInt16 ToUInt16(bool[] bit_datas)
        {
            if (bit_datas == null)
                throw new ArgumentNullException();
            if (bit_datas.Length > sizeof(UInt16) * 8)
                throw new ArgumentException("bit data is over 16");

            BitArray bitArray = new BitArray(bit_datas);
            ushort[] ret = new ushort[1] { 0 };
            bitArray.CopyTo(ret, 0);
            return ret[0];
        }

        public static bool[] ToBits(UInt16 word_data)
        {
            bool[] bits = new bool[16];
            for (int i = 0; i < sizeof(UInt16) * 8; i++)
            {
                int v = word_data & ((UInt16)1 << i);
                bits[i] = v == 0 ? false : true;
            }
            return bits;
        }
    }
}
