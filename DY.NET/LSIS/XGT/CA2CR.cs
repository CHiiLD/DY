/*
 * 작성자: CHILD	
 * 설명: ASC2 데이터를 Type형 변수로, Type형 변수를 ASC2데이터로 서로 변환하는 클래스
 * 날짜: 2015-03-31
 */

using System;
using System.Text;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 바이트 타입에 따른 문자 배열 데이터 <-> 문자열데이터, 정수형 데이터로의 변환 클래스입니다.
    /// CA2C와 기능은 같지만 리턴되는 byte를 역으로 리턴합니다
    /// </summary>
    public static class CA2CR
    {
        /// <summary>
        /// 정수형이나 문자열 데이터를 바이트 크기에 맞추어 문자 배열로 변환합니다.
        /// </summary>
        /// <param name="value"> 정수형, 문자열 데이터 </param>
        /// <param name="type"> 정수형, 문자열 타입 정보 </param>
        /// <returns></returns>
        public static byte[] ToASC(object value, Type type)
        {
            var byte_arr = CA2C.ToASC(value, type);
            Array.Reverse(byte_arr);
            return byte_arr;
        }

        /// <summary>
        /// 정수형이나 문자열 데이터를 자료형의 바이트 크기에 맞추어 문자 배열로 변환합니다.
        /// </summary>
        /// <param name="value"> 정수형, 문자열 데이터 </param>
        /// <returns></returns>
        public static byte[] ToASC(object value)
        {
            return ToASC(value, value.GetType());
        }
    }
}