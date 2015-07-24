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

    /// <summary>
    /// XGT 디바이스의 메모리 타입 정보 제공
    /// </summary>
    public static class XGTMemoryExpression
    {
        /// <summary>
        /// 디바이스 별 메모리 번지의 비트와 워드 지원 여부의 정보를 얻는다
        /// ____________MemoryExpression.BIT -> 16진수로 주소번지 표현________________
        /// * 비트: 10진수 번지 + A ~ F, PLC 표기 그래도 사용 
        /// * 바이트: 10진수 번지 + 0 or 8 번지 (그 외에는 정상적으로 읽지 못함)
        /// * 워드: 그대로
        /// * 더블 워드: 정수 부분을 2로 나눔 단, 정수는 2의 배수어야 한다
        /// * 롱 워드: 정수 부분을 4로 나눔 단, 정수는 4의 배수이어야 한다
        /// ____________MemoryExpression.WORD -> 10진수로 주소번지 표현____________
        /// * 워드: 그대로
        /// * 더블 워드: 정수 부분을 2로 나눔 단, 정수는 2의 배수어야 한다
        /// * 롱 워드: 정수 부분을 4로 나눔 단, 정수는 4의 배수이어야 한다
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
    }
}
