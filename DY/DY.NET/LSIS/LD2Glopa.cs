using System;

namespace DY.NET.LSIS
{
    public class LD2Glopa
    {
        public static string VarConvert(string ld, PLCVarType type)
        {
            string ret = ld;
            string type_s = null ;
            switch (type)
            {
                case PLCVarType.BIT:
                    type_s = "X";
                    break;
                case PLCVarType.BYTE:
                    type_s = "B";
                    break;
                case PLCVarType.WORD:
                    type_s = "W";
                    break;
                case PLCVarType.DWORD:
                    type_s = "D";
                    break;
                case PLCVarType.LWORD:
                    type_s = "L";
                    break;
            }
            ret.Insert(0, "%");
            ret.Insert(2, type_s);
            return ret;
        }
    }
}
