using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.WPF
{
    /// <summary>
    /// 데이터 표현 방식
    /// </summary>
    public enum PLCVariableType
    {
        BIT,
        BYTE,
        WORD,
        DWORD,
        LWORD,

        BOOL,
        SBYTE,
        SHORT,
        INT,
        LONG
    }
}