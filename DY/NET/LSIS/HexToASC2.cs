/*
 * 작성자: CHILD	
 * 기능: 정수형 변수를 XGT 프레임에 호환하는 ASC2값으로 변환합니다.
 * 날짜: 2015-03-25
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS
{
    public class HexToASC2
    {
        //%MW123 -> {%, M, W, 1, 2, 3}
        public static byte[] Convert(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        //0xff11 -> {0xff, 0x11}
        public static byte[] Convert(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value); //BitConverter는 1023 -> FF-03 빅에디안으로 들어가기 때문에 스왑을 해줘야 한다.
            byte swap = bytes[0];
            bytes[0] = bytes[1];
            bytes[1] = swap;
            return bytes;
        }

        //0x34 -> {0x3 , 0x4}
        public static byte[] Convert(byte value)
        {
            string hex = value.ToString("X2"); //0x?? 16진수 2자리 string format으로 컨버트해주는 표준 숫자 서식 문자열, 참고 (https://msdn.microsoft.com/ko-kr/library/dwhawy9k(v=vs.110).aspx)
            byte[] bytes = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => System.Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            return bytes;
        }
    }
}
