using System;
using System.Collections.Generic;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// PostBox 알고리즘
    /// </summary>
    public static class PostAlgorithm
    {
        /// <summary>
        /// 문자열을 정수로 변환
        /// </summary>
        /// <param name="glopa_var">그로파 변수 이름</param>
        /// <returns>메모리 번지 수</returns>
        private static int GetMemoryNumber(string glopa_var)
        {
            string num_str = glopa_var.Substring(3, glopa_var.Length - 3);
            int mem_ptr_num;
            Int32.TryParse(num_str, out mem_ptr_num);
            return mem_ptr_num;
        }

        public static List<Mail> LetterOpener(List<List<byte>> protocol_data)
        {
            List<Mail> ret = new List<Mail>();


            return ret;
        }

        /// <summary>
        /// 납봉
        /// </summary>
        /// <param name="mail">Mail 객체</param>
        /// <returns>List<Mail> 객체</returns>
        public static List<Mail> SealingWax(Mail mail)
        {
            string name = mail.Name;
            Type type = mail.VType;
            object value = mail.Value;
            List<Mail> ret = new List<Mail>();
            string var_name = "%" + name;

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("ARGUMENT NAME IS NULL OR EMPTY");
            if (type == null)
                throw new ArgumentNullException("ARGUMENT TYPE IS NULL");

            // BIT 처리
            if (type == typeof(Boolean))//D00000.0 -> %DX000000
            {
                var_name = name.Insert(2, "X");
                var_name = name.Replace(".", "");
                ret.Add(new Mail() { Name = var_name, Value = mail.Value, VType = mail.VType });
            }
            // BYTE 처리
            else if (type == typeof(SByte) || type == typeof(Byte))//D00010.8 -> %DB00020
            {
                var_name = name.Insert(2, "B");
                var_name = name.Replace(".", "");
                var_name = var_name.Substring(0, var_name.Length - 1);
                string num_str = (GetMemoryNumber(var_name) * 2).ToString(); //BYTE는 WORD기준으로 메모리를 2배 증가
                var_name = var_name.Substring(0, 3) + num_str;
                ret.Add(new Mail() { Name = var_name, Value = mail.Value, VType = mail.VType });
            }
            // WORD 처리
            else if (type == typeof(Int16) || type == typeof(UInt16))
            {
                var_name = name.Insert(2, "W");
                ret.Add(new Mail() { Name = var_name, Value = mail.Value, VType = mail.VType });
            }
            // DWORD 처리
            else if (type == typeof(Int32) || type == typeof(UInt32))
            {
                var_name = name.Insert(2, "D");
                if (GetMemoryNumber(var_name) % 2 == 0)
                {
                    string num_str = (GetMemoryNumber(var_name) / 2).ToString(); //DWORD는 WORD기준으로 메모리를 1/2배 증가
                    var_name = var_name.Substring(0, 3) + num_str;
                    ret.Add(new Mail() { Name = var_name, Value = mail.Value, VType = mail.VType });
                }
                else
                {
                    byte[] byte_data = CA2C.ToByte(value, value.GetType());
                    for (int i = 0; i < 2; i++)
                    {
                        ret.Add(new Mail()
                        {
                            Name = var_name.Substring(0, 2) + "W" + (GetMemoryNumber(var_name) + i).ToString(),
                            Value = CA2C.ToValue(new byte[2] { byte_data[3 - (i * 2)], byte_data[2 - (i * 2)] }, typeof(UInt16)),
                            VType = typeof(UInt16)
                        });
                    }
                }
            }
            // LWORD 처리
            else if (type == typeof(Int64) || type == typeof(UInt64))
            {
                var_name = name.Insert(2, "L");
                if (GetMemoryNumber(var_name) % 4 == 0)
                {
                    string num_str = (GetMemoryNumber(var_name) / 4).ToString(); //LWORD는 WORD기준으로 메모리를 1/4배 증가
                    var_name = var_name.Substring(0, 3) + num_str;
                    ret.Add(new Mail() { Name = var_name, Value = mail.Value, VType = mail.VType });
                }
                else
                {
                    byte[] byte_data = CA2C.ToByte(value, value.GetType());
                    for (int i = 0; i < 4; i++)
                    {
                        ret.Add(new Mail()
                        {
                            Name = var_name.Substring(0, 2) + "W" + (GetMemoryNumber(var_name) + i).ToString(),
                            Value = CA2C.ToValue(new byte[2] { byte_data[3 - (i * 2)], byte_data[2 - (i * 2)] }, typeof(UInt16)),
                            VType = typeof(UInt16)
                        });
                    }
                }
            }
#if DEBUG
            else
                System.Diagnostics.Debug.Assert(false);
#endif
            return ret;
        }
    }
}