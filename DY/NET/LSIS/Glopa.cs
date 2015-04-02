using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS
{
    public class Glopa
    {
        public static bool IsGlopaVar(string glopa_var_name)
        {
            return glopa_var_name[0] == '%';
        }

        public static DataType GetDataType(string glopa_var_name)
        {
            if (!IsGlopaVar(glopa_var_name))
                throw new ArgumentException("it's not glopa variable name");

            char device = glopa_var_name[1];
            char type = glopa_var_name[2];

            DataType ret = DataType.WORD;

            switch(type)
            {
                case 'X':
                    ret = DataType.BIT;
                    break;
                case 'B':
                    ret = DataType.BYTE;
                    break;
                case 'W':
                    ret = DataType.WORD;
                    break;
                case 'D':
                    ret = DataType.DWORD;
                    break;
                case 'L':
                    ret = DataType.LWORD;
                    break;
                default:
                    throw new Exception("glopa variable name wrong");
                    break;
            }
            return ret;
        }
    }
}
