/*
 * 작성자: CHILD	
 * 기능: byte array(asc data) to string for debug
 * 날짜: 2015-03-31
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public class Bytes2HexString
    {
        public static string Change(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach(byte b in data)
            {
                sb.Append(string.Format("{0:X2}", b));
                sb.Append(',');
                sb.Append(' ');
            }
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
    }
}
