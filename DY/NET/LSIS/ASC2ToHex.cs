/*
 * 작성자: CHILD	
 * 기능: ACK응답에서 가져온 byte값들을 데이타 타입에 맞추어 int32로 변환
 * 날짜: 2015-03-25
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS
{
    public class ASC2ToHex
    {
        // {0x41, 0x39} -> 0xA9(169)
        public static int Convert(DataType dataType, byte[] data)
        {
            int datasize = (int) dataType;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("0x");
            for(int i = 0; i < data.Length; i ++)
            {
                byte d = data[i];
                sb.Append(d);
            }
            return System.Convert.ToInt32(sb.ToString());
        }
    }
}