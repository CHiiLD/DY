/*
 * 작성자: CHILD	
 * 기능: 제네릭 정수를 byte[]로, byte[]를 원하는 제니릭 타입 정수로 변환하는 클래스
 * 날짜: 2015-03-26
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class TransASC<T>
    {
        //제네릭 정수값을 원하는 byte의 수만큼 asc값으로 변환합니다.
        public static byte[] ToASC(T integer, int asc_byte_size)
        {
            string hex_str = string.Format("{0:X}", integer);
            byte[] asc = new byte[asc_byte_size];
            for (int i = 0; i < asc.Length; i++)
                asc[i] = (byte)'0';
            int asc_idx = asc_byte_size - 1;
            for (int i = hex_str.Length - 1; i >= 0; i--)
                asc[asc_idx--] = (byte)hex_str[i];
            return asc;
        }

        //아스키 byte 배열을 제네릭 타입에 맞추어 정수타입으로 변환시킵니다.
        public static T ToInt(byte[] asc)
        {
            StringBuilder sb = new StringBuilder();
            foreach(byte b in asc)
            {
                char c = (char)b;
                string s = string.Format("{0:X}", c);
                sb.Append(s);
            }
            
            ulong hex = Convert.ToUInt64(sb.ToString(), 16);
            object ret = Convert.ChangeType(hex, typeof(T));
            return (T)ret;
        }
    }
}
