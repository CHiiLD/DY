/*
 * 작성자: CHILD	
 * 기능: 바이너리 통신에 쓰이는 변수들의 데이터 타입의 열거 클래스
 * 날짜: 2015-03-25
 */

using System;

namespace DY.NET
{
    public enum PLCVarType : int
    {
        BIT = 0,
        BYTE = 1,
        WORD = 2,
        DWORD = 4,
        LWORD = 8
    }

    public static class PLCVarTypeExtension
    {
        public static int ToSize(this PLCVarType type)
        {
            if (type == PLCVarType.BIT)
                return 1;
            else
                return (int)type;
        }

        public static Type ToType(this PLCVarType type)
        {
            Type t = null;
            switch (type)
            {
                case PLCVarType.BIT:
                    t = typeof(byte);
                    break;
                case PLCVarType.BYTE:
                    t = typeof(byte);
                    break;
                case PLCVarType.WORD:
                    t = typeof(short);
                    break;
                case PLCVarType.DWORD:
                    t = typeof(int);
                    break;
                case PLCVarType.LWORD:
                    t = typeof(long);
                    break;
            }
            return t;
        }
    }
}