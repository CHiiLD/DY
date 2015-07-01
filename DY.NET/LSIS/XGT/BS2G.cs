
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// business standard notation to glopa notation
    /// 업계 표준 WORD 기준의 메모리 표기법을 XGT타입의 글로파 변수로 변환합니다.
    /// 비트: 소수점만 제거
    /// 바이트: 정수 부분을 2배로 올린 뒤 끝 소수점이 8이면 +1 을 합니다
    /// 워드: 그대로 쓰면 됩니다
    /// 더블 워드: 정수 부분을 2로 나눕니다 단, 정수는 2의 배수어야 합니다
    /// 롱 워드: 정수 부분을 4로 나눕니다. 단, 정수는 4의 배수이어야 합니다
    /// </summary>
    public static class BS2G
    {
        /// <summary>
        /// business standard notation to glopa notation
        /// 반드시 DWORD 사용시 변수 메모리 번지는 2의 배수이어야 한다
        /// 반드시 LWORD 사용시 변수 메모리 번지는 4의 배수이어야 한다
        /// </summary>
        /// <param name="std_name"> 업계 표준 표기법 </param>
        /// <param name="type"> 해당 변수의 타입 </param>
        /// <param name="target"> 타겟 글로파변수 </param>
        /// <returns> 변환 성공 여부 </returns>
        public static bool ToGlopaNotation(string std_name, Type type, out string target)
        {
            if (!NumericType.IsNumeric(type))
                throw new ArgumentException("type is not integer.");
            if (string.IsNullOrEmpty(std_name))
                throw new ArgumentNullException("std_name is null or empty.");
            if (Glopa.IsGlopaType(std_name))
                throw new ArgumentException("already glopa type.");
            char device_char = std_name[0];
            if (!XGTServiceableDevice.GetCnetServiceableModeDictionary().ContainsKey(device_char))
                throw new ArgumentException("invalid plc variable name");
            bool isConvert = false;
            StringBuilder sb = new StringBuilder("%");
            sb.Append(device_char);
            do
            {
                uint integer = 0;
                string str_num = std_name;
                str_num = str_num.Replace(".", "");
                str_num = str_num.Substring(1, str_num.Length - 1);
                if (type == typeof(Boolean))
                {
                    sb.Append('X');
                    sb.Append(str_num);
                }
                else if (type == typeof(SByte) || type == typeof(Byte))
                {
                    string str_int = str_num.Substring(0, str_num.Length - 1); //정수  
                    int single_digit = str_num.Last() == '0' ? 0 : 1;          //소수점
                    if (!UInt32.TryParse(str_int.ToString(), out integer))
                        break;
                    sb.Append('B');
                    sb.Append((integer * 2 + single_digit).ToString());
                }
                else if (type == typeof(Int16) || type == typeof(UInt16))
                {
                    sb.Append('W');
                    sb.Append(str_num);
                }
                else if (type == typeof(Int32) || type == typeof(UInt32))
                {
                    if (!UInt32.TryParse(str_num.ToString(), out integer))
                        break;
                    if (integer % 2 != 0)
                        break;
                    sb.Append('D');
                    sb.Append((integer / 2).ToString());
                }
                else if (type == typeof(Int64) || type == typeof(UInt64))
                {
                    if (!UInt32.TryParse(str_num.ToString(), out integer))
                        break;
                    if (integer % 4 != 0)
                        break;
                    sb.Append('L');
                    sb.Append((integer / 4).ToString());
                }
#if DEBUG
                else
                    System.Diagnostics.Debug.Assert(false);
#endif
                isConvert = true;
            } while (false);
            target = sb.ToString();
            return isConvert;
        }
    }
}
