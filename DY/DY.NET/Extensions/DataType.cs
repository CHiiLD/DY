/*
작성자: CHILD	
기능: 바이너리 통신에 쓰이는 변수들의 데이터 타입의 열거 클래스
날짜: 2015-03-25
*/

using System;

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

    public static class DataTypeExtensions
    {
        public static int SizeOf(this DataType type)
        {
            if (type == DataType.BIT)
                return 1;
            else
                return (int)type;
        }

        public static Type TypeOf(this DataType type)
        {
            Type t = null;
            switch (type)
            {
                case DataType.BIT:
                    t = typeof(byte);
                    break;
                case DataType.BYTE:
                    t = typeof(byte);
                    break;
                case DataType.WORD:
                    t = typeof(ushort);
                    break;
                case DataType.DWORD:
                    t = typeof(uint);
                    break;
                case DataType.LWORD:
                    t = typeof(ulong);
                    break;
            }
            return t;
        }
    }
}