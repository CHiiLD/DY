using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class ENQDataFormat
    {
        public string GlopaVarName; //?byte
        public object Data;     //?byte

        public ENQDataFormat(string glopaVarName)
        {
            if (!Glopa.IsGlopaVar(glopaVarName))
                throw new ArgumentException("glopaVarName is not glopa type's name");
            else if (glopaVarName.Length > 16)
                throw new ArgumentException("glopa var over limited string's length");
            GlopaVarName = glopaVarName;
        }

        public ENQDataFormat(string glopaVarName, object data)
        {
            if (!NumericTypeExtension.IsNumeric(data))
                throw new ArgumentException("data is not numeric type.");
            else if (!Glopa.IsGlopaVar(glopaVarName))
                throw new ArgumentException("glopaVarName is not glopa type's name");
            else if (glopaVarName.Length > 16)
                throw new ArgumentException("glopa var over limited string's length");

            GlopaVarName = glopaVarName;
            if (Glopa.GetDataType(glopaVarName) == DataType.BIT)
                Data = Convert.ToUInt16(data);        //BIT데이터는 2BYTE로 표현합니다 ture => {0x00, 0x01}
            else
                Data = data;
        }
    }
}