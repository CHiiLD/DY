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
            if (glopa_var_name == null)
                return false;
            else if (glopa_var_name.Length == 0)
                return false;

            foreach (char c in glopa_var_name)
            {
                if (c != '%' && (!('a' <= c && c <= 'z')) && (!('A' <= c && c <= 'Z')) && (!('0' <= c && c <= '9')) && c != '_')
                    return false;
            }
            return glopa_var_name[0] == '%';
        }

        public static DataType GetDataType(string glopa_var_name)
        {
            if (string.IsNullOrEmpty(glopa_var_name))
                throw new ArgumentNullException("argument is null or empty");
            else  if (!IsGlopaVar(glopa_var_name))
                throw new ArgumentException("it's not glopa variable name");
            else if(glopa_var_name.Length < 4)
                throw new ArgumentException("it's not understandable glopa variable name");

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
            }
            return ret;
        }
    }
}
