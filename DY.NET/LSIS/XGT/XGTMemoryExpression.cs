using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.LSIS.XGT
{
    [Flags]
    public enum MemoryExpression
    {
        // 디바이스|워드번지(10진수)|메모리번지(16진수)
        BIT = 0x01,
        WORD = 0x02
    }

    public static class XGTMemoryExpression
    {
        /// <summary>
        /// 디바이스 별 메모리 번지의 비트와 워드 지원 여부의 정보를 얻는다
        /// MemoryExpression.BIT -> 16진수로 주소번지 표현
        /// MemoryExpression.WORD -> 10진수로 주소번지 표현
        /// MemoryExpression.BIT |MemoryExpression.WORD -> 10진수로 주소번지 표현, 비트는 .H 로 표현
        /// </summary>
        /// <returns>key: device info, value: 비트, 워드 디바이스 영역 정보</returns>
        public static Dictionary<char, MemoryExpression> GetMemExpDictionary()
        {
            Dictionary<char, MemoryExpression> dic = new Dictionary<char, MemoryExpression>();
            dic.Add('P', MemoryExpression.BIT);
            dic.Add('M', MemoryExpression.BIT);
            dic.Add('L', MemoryExpression.BIT);
            dic.Add('K', MemoryExpression.BIT);
            dic.Add('F', MemoryExpression.BIT);
            dic.Add('T', MemoryExpression.BIT | MemoryExpression.WORD);
            dic.Add('C', MemoryExpression.BIT | MemoryExpression.WORD);
            dic.Add('S', MemoryExpression.BIT);

            dic.Add('D', MemoryExpression.BIT | MemoryExpression.WORD);
            dic.Add('R', MemoryExpression.BIT | MemoryExpression.WORD);
            dic.Add('U', MemoryExpression.BIT | MemoryExpression.WORD);

            dic.Add('N', MemoryExpression.WORD);
            dic.Add('Z', MemoryExpression.WORD);
            return dic;
        }
    }
}
