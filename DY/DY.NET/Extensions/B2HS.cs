/*
 * 작성자: CHILD	
 * 기능: BYTE ARRAY(ASC DATA) TO STRING FOR DEBUG
 * 날짜: 2015-03-31
 */

using System;
using System.Text;

namespace DY.NET
{
    /// <summary>
    /// 디버깅용 byte 배열을 확인하기 위한 클래스입니다.
    /// </summary>
    public class B2HS
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
