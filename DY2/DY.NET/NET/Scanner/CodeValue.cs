using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public class CodeValue : IValue
    {
        public object Value { get; set; }
        public CodeValue(byte[] code)
        {
            Value = code;
        }
    }
}