using System;

namespace DY.NET.LSIS
{
    /// <summary>
    /// 그로파 변수를 위한 클래스
    /// </summary>
    public class Glopa
    {
        public const string ERROR_ANOTHER_TYPE = "IT IS NOT GLOPA TYPE'S NAME";

        /// <summary>
        /// 매겨변수의 문자열이 그로파 타입의 변수이름인지 판별한다.
        /// </summary>
        /// <param name="glopa_var_name"> 그로파 변수 이름 </param>
        /// <returns> 그로파 변수의 포맷이 맞다면 true, 아니면 false </returns>
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

        /// <summary>
        /// 그로파 변수의 두번 째 인자값을 보고 그로파 변수에 할당된 메모리의 자료형 타입을 판별한다.
        /// </summary>
        /// <param name="glopa_var_name"> 그로파 변수 이름 </param>
        /// <returns> DataType </returns>
        public static PLCVarType GetDataType(string glopa_var_name)
        {
            if (string.IsNullOrEmpty(glopa_var_name))
                throw new ArgumentNullException("argument is null or empty");
            else if (!IsGlopaVar(glopa_var_name))
                throw new ArgumentException(ERROR_ANOTHER_TYPE);
            else if (glopa_var_name.Length < 4)
                throw new ArgumentException("it's not understandable glopa variable name");

            char device = glopa_var_name[1];
            char type = glopa_var_name[2];

            PLCVarType ret = PLCVarType.WORD;

            switch (type)
            {
                case 'X':
                    ret = PLCVarType.BIT;
                    break;
                case 'B':
                    ret = PLCVarType.BYTE;
                    break;
                case 'W':
                    ret = PLCVarType.WORD;
                    break;
                case 'D':
                    ret = PLCVarType.DWORD;
                    break;
                case 'L':
                    ret = PLCVarType.LWORD;
                    break;
                default:
                    throw new Exception("glopa variable name wrong");
            }
            return ret;
        }
    }
}
