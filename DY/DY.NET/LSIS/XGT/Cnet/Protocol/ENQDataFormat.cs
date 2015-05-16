using System;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// call by value 를 위해 구조체로 작성
    /// </summary>
    public struct ENQDataFormat
    {
        public const int GROPA_VAR_LENGTH_LIMIT = 16;
        public const string ERROR_GROPA_VAR_LENGTH_LIMIT = "GLOPA VAR OVER LIMITED STRING'S LENGTH";

        public string GlopaVarName; //?byte
        public object Data;         //?byte

        public ENQDataFormat(string glopaVarName)
        {
            if (!Glopa.IsGlopaVar(glopaVarName))
                throw new ArgumentException(Glopa.ERROR_ANOTHER_TYPE);
            else if (glopaVarName.Length > GROPA_VAR_LENGTH_LIMIT)
                throw new ArgumentException(ERROR_GROPA_VAR_LENGTH_LIMIT);
            GlopaVarName = glopaVarName;
            Data = null;
        }

        public ENQDataFormat(string glopaVarName, object data)
        {
            if (!NumericTypeExtension.IsNumeric(data))
                throw new ArgumentException(NumericTypeExtension.ERROR_NOT_NEMERIC_TYPE);
            else if (!Glopa.IsGlopaVar(glopaVarName))
                throw new ArgumentException(Glopa.ERROR_ANOTHER_TYPE);
            else if (glopaVarName.Length > GROPA_VAR_LENGTH_LIMIT)
                throw new ArgumentException(ERROR_GROPA_VAR_LENGTH_LIMIT);

            GlopaVarName = glopaVarName;
            if (Glopa.GetDataType(glopaVarName) == PLCVarType.BIT)
                Data = Convert.ToUInt16(data);        //BIT데이터는 2BYTE로 표현합니다 ture => {0x00, 0x01}
            else
                Data = data;
        }
    }
}