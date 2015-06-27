using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// Type 확장 클래스
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Type 객체의 해당 바이트 크기를 리턴
        /// </summary>
        /// <param name="type">Type 객체</param>
        /// <returns>바이트 크기</returns>
        public static int ToSize(this Type type)
        {
            int size = 0;
            if (type == typeof(Boolean))
                size = 1;
            else if (type == typeof(SByte) || type == typeof(Byte))
                size = 1;
            else if (type == typeof(Int16) || type == typeof(UInt16))
                size = 2;
            else if (type == typeof(Int32) || type == typeof(UInt32))
                size = 4;
            else if (type == typeof(Int64) || type == typeof(UInt64))
                size = 8;
#if DEBUG
            else
                System.Diagnostics.Debug.Assert(false);
#endif
            return size;
        }
    }
}
