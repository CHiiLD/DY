using System;
using System.Collections.Generic;
using System.Linq;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// 프로토콜 통신 횟수를 줄여 통신 최적화를 돕는 헬프 클래스
    /// </summary>
    public static class XGTProtocolHelper
    {
        public static readonly byte[] ServiceableDeviceTerritory = { (byte)'D', (byte)'M' };

        /// <summary>
        /// 메모리의 10진수 표기값 부분만을 Int32 정수로 반환한다.
        /// %MX1234F의 변수라면 10진수인 1234 만을 정수로 변환하여 반환
        /// </summary>
        /// <param name="data_type"></param>
        /// <param name="prog_var"></param>
        /// <returns></returns>
        private static int GetDecimalInteger(DataType data_type, string prog_var)
        {
            prog_var = prog_var.ToUpper().Replace(".", "");
            int demical_int;
            bool has_hex = !(data_type == DataType.BIT || data_type == DataType.BOOL || data_type == DataType.BYTE || data_type == DataType.SBYTE);
            int mem_size = XGTServiceableDevice.MemoryTerritorySize[prog_var[0]];
            MemoryExpression mem_exp = XGTMemoryExpression.MemExpDictionary[prog_var[0]];
            string demical_str = prog_var.Substring(1, prog_var.Length - 1 - (has_hex ? 0 : 1));

            //데이터 타입은 BIT나 BYTE를 선택했지만 정작 디바이스가 BIT, BYTE를 지원하지 않는 타입인가?
            if (((mem_exp & MemoryExpression.BIT) == 0) && has_hex)
                throw new ArgumentException("[TYPE1] Invalid memory variable string.");
            //10진수의 자리수 범위가 올바른가?
            if (!(1 <= demical_str.Length && demical_str.Length <= mem_size.ToString().Length))
                throw new ArgumentException("[TYPE2] Invalid memory variable string.");
            //10진수가 정수로 변환 가능한가? 
            if (!Int32.TryParse(demical_str, out demical_int))
                throw new ArgumentException("[TYPE3] Invalid memory variable string.");
            //디바이스의 범위를 초과하지 않았나?
            if (demical_int >= mem_size)
                throw new ArgumentException("Invalid range memory(D: 00000~19999, M: 0000~2023).");
            return demical_int;
        }

        /// <summary>
        /// 글로파변수의 디바이스가 XGT에서 사용가능한 디바이스영역 인지를 질의한다.
        /// 현재 M과 D영역만 사용가능하다.
        /// </summary>
        /// <param name="data_type"></param>
        /// <param name="glopa_var"></param>
        /// <returns></returns>
        public static bool IsServiceableDeviceTerritory(DataType data_type, string glopa_var)
        {
            if (!XGTMemoryExpression.MemExpDictionary.ContainsKey(glopa_var[1]))
                return false;
            IEnumerable<byte> allow_device = from a in ServiceableDeviceTerritory where a == (byte)glopa_var[1] select a;
            if (allow_device == null || allow_device.Count() == 0)
                return false;
            return true;
        }

        /// <summary>
        /// 프로그램 변수를 글로파 변수로 변환하여 반환한다.
        /// M0000 => %MW0000
        /// </summary>
        /// <param name="data_type"></param>
        /// <param name="mem_name"></param>
        /// <returns></returns>
        public static string ToGlopa(this DataType data_type, string mem_name)
        {
            if (String.IsNullOrEmpty(mem_name))
                throw new ArgumentException("Memory variable name is empty.");
            string result = null;
            string type_char = null;
            string addr = mem_name.ToUpper().Replace(".", "");
            int demical_int = GetDecimalInteger(data_type, addr);

            switch (data_type)
            {
                case DataType.BIT:
                case DataType.BOOL:
                    result = addr;
                    type_char = "X";
                    break;
                case DataType.BYTE:
                case DataType.SBYTE:
                    if (!(addr.Last() == '0' || addr.Last() == '8'))
                        throw new ArgumentException("Invalid memory variable string. the byte/sbyte addresses must end in 0 or 8");
                    int b = addr.Last() == '0' ? 0 : 1;
                    result = addr[0] + ((demical_int * 2) + b).ToString();
                    type_char = "B";
                    break;
                case DataType.SHORT:
                case DataType.WORD:
                    result = addr;
                    type_char = "W";
                    break;
                case DataType.DWORD:
                case DataType.INT:
                case DataType.LWORD:
                case DataType.LONG:
                    type_char = (data_type == DataType.DWORD || data_type == DataType.INT) ? "D" : "L";
                    int j = (data_type == DataType.DWORD || data_type == DataType.INT) ? 2 : 4;
                    if (demical_int % j != 0)
                        throw new ArgumentException("Address of the memory must be a multiple of " + j + ".");
                    result = addr[0] + (demical_int / j).ToString();
                    break;
                default:
                    throw new NotImplementedException();
            }
            result = result.Insert(0, "%").Insert(2, type_char);
            return result;
        }

        /// <summary>
        /// PLC IO READ할 변수들 중 BIT데이터는 WORD메모리 번지 수로 부르도록 변경한 뒤에 겹치는 WORD 메모리번지수를 
        /// 하나로 호출함으로써 최적화를 유도한다. 
        /// 그 외에 BYTE, INT32, INT64는 영역겹침 문제와 메모리 꼬임 때문에 그대로 호출한다.
        /// </summary>
        /// <param name="items">ICommIOData 리스트 데이터</param>
        /// <returns></returns>
        public static Dictionary<string, DataType> Optimize(IList<ICommIOData> items)
        {
            if (items == null)
                throw new ArgumentNullException("items argument is null");

            Dictionary<string, DataType> tickets = new Dictionary<string, DataType>();
            int line_cnt = 0;
            foreach (var i in items)
            {
                ++line_cnt;
                DataType type = i.Type;
                try
                {
                    string glopa_var = type.ToGlopa(i.Address);
                    if (!IsServiceableDeviceTerritory(type, glopa_var))
                        throw new ArgumentException("Not supported [" + glopa_var[1] + "] device.");

                    if (type == DataType.BOOL || type == DataType.BIT)
                        glopa_var = glopa_var.Substring(0, glopa_var.Length - 1).Remove(2, 1).Insert(2, "W");
                    if (!tickets.ContainsKey(glopa_var))
                        tickets.Add(glopa_var, ((type == DataType.BIT) || (type == DataType.BOOL)) ? DataType.WORD : type);
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message + "(row" + line_cnt + ")");
                }
            }
            return tickets;
        }

        /// <summary>
        /// 프로토콜 통신으로 받은 데이터를 분석하여 ICommIOData 컬렉션 Value값을 추적하여 채운다
        /// </summary>
        /// <param name="recv_data">Protocol 데이터</param>
        /// <param name="items">ICommIOData 리스트 데이터</param>
        /// <returns></returns>
        public static void Fill(Dictionary<string, object> recv_data, IList<ICommIOData> items)
        {
            if (recv_data == null || items == null)
                throw new ArgumentNullException("items argument is null");
            Dictionary<string, bool[]> bit_storagy = new Dictionary<string, bool[]>();
            foreach (var i in items)
            {
                string addr = i.Address;
                DataType type = i.Type;
                string glopa_var = type.ToGlopa(addr);
                if (type == DataType.BOOL || type == DataType.BIT)
                    glopa_var = glopa_var.Substring(0, glopa_var.Length - 1).Remove(2, 1).Insert(2, "W");
                if (!recv_data.ContainsKey(glopa_var))
                    continue;

                object value = recv_data[glopa_var];
                switch (type)
                {
                    case DataType.BIT:
                    case DataType.BOOL:
                        int idx = Convert.ToInt32(addr.Last().ToString(), 16);
                        if (bit_storagy.ContainsKey(addr))
                        {
                            i.Output = bit_storagy[addr][idx];
                        }
                        else
                        {
                            bool[] bits = B2W.ToBools((ushort)value);
                            i.Output = bits[idx];
                            bit_storagy.Add(addr, bits);
                        }
                        break;
                    case DataType.BYTE:
                    case DataType.SBYTE:
                    case DataType.SHORT:
                    case DataType.WORD:
                    case DataType.DWORD:
                    case DataType.INT:
                    case DataType.LONG:
                    case DataType.LWORD:
                        i.Output = value;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}