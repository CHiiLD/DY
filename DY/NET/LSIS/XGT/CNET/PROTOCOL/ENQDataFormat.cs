using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class ENQDataFormat
    {
        public ushort Var_Len;  //2byte
        public string Var_Name; //?byte
        public object Data;     //?byte
    }
}
