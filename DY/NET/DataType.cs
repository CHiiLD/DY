/*
작성자: CHILD	
기능: 바이너리 통신에 쓰이는 변수들의 데이터 타입의 열거 클래스
날짜: 2015-03-25
*/

namespace DY.NET
{
    public enum DataType : int
    {
        BIT = 0,
        BYTE = 1,  
        WORD = 2,
        DWORD = 4,
        LWORD = 8
    }

    public class DataTypeExtensions
    {
        public static int SizeOf(DataType type)
        {
            if (type == DataType.BIT)
                return 1;
            else
                return (int)type;
        }
    }
}