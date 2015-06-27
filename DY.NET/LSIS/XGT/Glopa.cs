using System;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 그로파 변수를 위한 클래스
    /// </summary> 
    public static class Glopa
    {
        public const string ERROR_ANOTHER_TYPE = "IT IS NOT GLOPA TYPE'S NAME";

        /// <summary>
        /// 매겨변수의 문자열이 그로파 타입의 변수이름인지 판별한다.
        /// </summary>
        /// <param name="name"> 그로파 변수 이름 </param>
        /// <returns> 그로파 변수의 포맷이 맞다면 true, 아니면 false </returns>
        public static bool IsGlopaType(string name)
        {
            bool ret = false;
            do
            {
                if (string.IsNullOrEmpty(name))
                    break;
                foreach (char c in name)
                    if (c != '%' && (!('a' <= c && c <= 'z')) && (!('A' <= c && c <= 'Z')) && (!('0' <= c && c <= '9')) && c != '_')
                        break;
                if (name[0] != '%')
                    break;
                //디바이스 정보가 올바르지 않은 경우를 검사
                if (!ServiceableDevice.GetDeviceInfo().ContainsKey(name[1]))
                    break;
                //메모리 번지의 타입이 올바르지 않은 경우를 검사 
                char type = name[2];
                if (type == 'X' || type == 'B' || type == 'W' || type == 'D' || type == 'L')
                    ret = true;
            } while (false);
            return ret;
        }

        /// <summary>
        /// 그로파 변수의 두번 째 인자값을 보고 그로파 변수에 할당된 메모리의 자료형 타입을 판별한다.
        /// </summary>
        /// <param name="glopa_var_name"> 그로파 변수 이름 </param>
        /// <returns> DataType </returns>
        public static Type GetDataType(string glopa_var_name)
        {
            if (string.IsNullOrEmpty(glopa_var_name))
                throw new ArgumentNullException("argument is null or empty");
            else if (!IsGlopaType(glopa_var_name))
                throw new ArgumentException(ERROR_ANOTHER_TYPE);
            else if (glopa_var_name.Length < 4)
                throw new ArgumentException("it's not understandable glopa variable name");
            Type type;
            switch (glopa_var_name[2])
            {
                case 'X':
                    type = typeof(Boolean);
                    break;
                case 'B':
                    type = typeof(Byte);
                    break;
                case 'W':
                    type = typeof(UInt16);
                    break;
                case 'D':
                    type = typeof(UInt32);
                    break;
                case 'L':
                    type = typeof(UInt64);
                    break;
                default:
                    throw new Exception("glopa variable name wrong");
            }
            return type;
        }
    }
}
