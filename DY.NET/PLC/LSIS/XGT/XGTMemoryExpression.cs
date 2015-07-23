using System;
using System.Collections.Generic;

namespace DY.NET.LSIS.XGT
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
        /// ____________MemoryExpression.BIT | MemoryExpression.WORD________________
        /// * 비트: 소수점만 제거
        /// * 바이트: 정수 부분을 2배로 올린 뒤, 끝 소수점이 8이면 +1
        /// * 워드: 그대로
        /// * 더블 워드: 정수 부분을 2로 나눔 단, 정수는 2의 배수어야 한다
        /// * 롱 워드: 정수 부분을 4로 나눔 단, 정수는 4의 배수이어야 한다
        /// </summary>
        /// <returns>key: device info, value: 비트, 워드 디바이스 영역 정보</returns>

        public static readonly Dictionary<char, MemoryExpression> MemExpDictionary = new Dictionary<char, MemoryExpression>
        {
            { 'P', MemoryExpression.BIT },
            { 'M', MemoryExpression.BIT },
            { 'L', MemoryExpression.BIT },
            { 'K', MemoryExpression.BIT },
            { 'F', MemoryExpression.BIT },
            { 'T', MemoryExpression.WORD },
            { 'C', MemoryExpression.WORD },
            { 'S', MemoryExpression.WORD },
            { 'D', MemoryExpression.BIT | MemoryExpression.WORD },
            { 'R', MemoryExpression.BIT | MemoryExpression.WORD },
            { 'U', MemoryExpression.BIT | MemoryExpression.WORD },
            { 'N', MemoryExpression.WORD },
            { 'Z', MemoryExpression.WORD },
        };

        public static bool IsDecimal(this MemoryExpression me)
        {
            return (me & MemoryExpression.WORD) != 0;
        }

        public static bool IsHexDecimal(this MemoryExpression me)
        {
            return (me & MemoryExpression.WORD) == 0;
        }
    }
}
