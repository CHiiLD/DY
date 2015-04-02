using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class ENQDataFormat
    {
        public string Var_Name;
        public object Data;     //?byte

        public ENQDataFormat(string varName)
        {
            Var_Name = varName;
        }

        public ENQDataFormat(string varName, object data)
        {
            Var_Name = varName;
            if(Glopa.GetDataType(varName) == DataType.BIT)
                Data = Convert.ToUInt16(data);        //BIT데이터는 2BYTE로 표현합니다 ture => {0x00, 0x01}
            else
                Data = data;
        }
    }
}
