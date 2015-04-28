using System;

namespace DY.NET.LSIS
{
    public class LD2Glopa
    {
        public static string VarConvert(string ld, DataType type)
        {
            string ret = ld;
            string type_s = null ;
            switch (type)
            {
                case DataType.BIT:
                    type_s = "X";
                    break;
                case DataType.BYTE:
                    type_s = "B";
                    break;
                case DataType.WORD:
                    type_s = "W";
                    break;
                case DataType.DWORD:
                    type_s = "D";
                    break;
                case DataType.LWORD:
                    type_s = "L";
                    break;
            }
            ret.Insert(0, "%");
            ret.Insert(2, type_s);
            return ret;
        }
    }
}
