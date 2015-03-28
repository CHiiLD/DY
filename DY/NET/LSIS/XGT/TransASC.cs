/*
 * 작성자: CHILD	
 * 기능: 정수를 byte[]로, byte[]를 원하는 제니릭 타입 정수로 변환하는 클래스
 * 날짜: 2015-03-26
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class TransASC
    {
        // 정수를 원하는 바이트의 크기만큼 배열로 치환
        public static byte[] ToASC(object integer, DataType data_type)
        {
            if (typeof(object) == typeof(string))
                return ToASC((string)integer, (int)data_type);

            int data_byte_size = DataTypeExtensions.SizeOf(data_type);
            string hex_str = string.Format("{0:X}", integer);
            byte[] asc = new byte[data_byte_size];
            for (int i = 0; i < asc.Length; i++)
                asc[i] = (byte)'0';
            int asc_idx = data_byte_size - 1;
            for (int i = hex_str.Length - 1; i >= 0; i--)
                asc[asc_idx--] = (byte)hex_str[i];
            return asc;
        }

        //byte 배열을 제네릭 타입에 맞추어 헥사정수타입으로 변환시킵니다. {0x32, 0x30} -> 0x20
        public static object ToHex(byte[] asc, Type type)
        {
            StringBuilder sb = new StringBuilder();
            foreach(byte b in asc)
                sb.Append(Convert.ToChar(b));
            ulong hex = Convert.ToUInt64(sb.ToString(), 16);
            object ret = Convert.ChangeType(hex, type);
            return ret;
        }

        //아스키 byte 배열 값을 문자열로 변환합니다. {0x25, 0x4D} -> "%M"
        public static string ToString(byte[] asc)
        {
            string str = BitConverter.ToString(asc).Replace("-", string.Empty);
            return str;
        }

        //ASC단위의 바이트 배열로 변환합니다. "SS", 1 -> {0x53, 0x53}
        public static byte[] ToASC(string hex_str, int char_read_cnt)
        {
            byte[] asc = new byte[hex_str.Length / char_read_cnt];
            int cnt = 0;
            for (int i = 0; i < hex_str.Length / char_read_cnt; i = i + 2)
            {
                string query = hex_str.Substring(i, char_read_cnt);
                byte b = Convert.ToByte(query, 16);
                asc[cnt++] = b;
            }
            return asc;
        }
    }
}
