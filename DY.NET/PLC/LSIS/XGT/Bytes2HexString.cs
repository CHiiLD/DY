using System.Text;

namespace DY.NET.LSIS.XGT
{
    public class Bytes2HexString
    {
        /// <summary>
        /// ASCII 데이터를 1byte 씩 HEX값으로 계산하여 문자열을 만들어 반환한다.
        /// </summary>
        /// <param name="ASCII">ASCII데이터</param>
        /// <returns>HEX 문자열</returns>
        public static string Change(byte[] ASCII)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ASCII)
            {
                sb.Append(string.Format("{0:X2}", b));
                sb.Append(' ');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}