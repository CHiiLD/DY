using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public class NumericTypeExtension
    {
        public static bool IsNumeric(object data)
        {
            Type dataType = data.GetType();
            if (dataType == null)
                return false;
            return 
                    (dataType == typeof(Byte)
                    || dataType == typeof(Int16)
                    || dataType == typeof(Int32)
                    || dataType == typeof(Int64)
                    || dataType == typeof(SByte)
                    || dataType == typeof(UInt16)
                    || dataType == typeof(UInt32)
                    || dataType == typeof(UInt64)
                   );
        }
    }
}
