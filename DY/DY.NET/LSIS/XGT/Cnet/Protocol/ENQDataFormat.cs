using System;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// call by value 를 위해 구조체로 작성
    /// </summary>
    public struct ENQDataFormat
    {
        public const int GROPA_VAR_LENGTH_LIMIT = 16;

        public string GlopaVarName; //?byte
        public object Data;         //?byte

        public ENQDataFormat(string glopaVarName)
        {
            if (!Glopa.IsGlopaVar(glopaVarName))
                throw new ArgumentException("glopaVarName is not glopa type's name");
            else if (glopaVarName.Length > GROPA_VAR_LENGTH_LIMIT)
                throw new ArgumentException("glopa var over limited string's length");
            GlopaVarName = glopaVarName;
            Data = null;
        }

        public ENQDataFormat(string glopaVarName, object data)
        {
            if (!NumericTypeExtension.IsNumeric(data))
                throw new ArgumentException("data is not numeric type.");
            else if (!Glopa.IsGlopaVar(glopaVarName))
                throw new ArgumentException("glopaVarName is not glopa type's name");
            else if (glopaVarName.Length > GROPA_VAR_LENGTH_LIMIT)
                throw new ArgumentException("glopa var over limited string's length");

            GlopaVarName = glopaVarName;
            if (Glopa.GetDataType(glopaVarName) == PLCVarType.BIT)
                Data = Convert.ToUInt16(data);        //BIT데이터는 2BYTE로 표현합니다 ture => {0x00, 0x01}
            else
                Data = data;
        }
    }
}