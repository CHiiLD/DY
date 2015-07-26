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
        public static readonly byte[] AllowDevice = { (byte)'D', (byte)'M' };

        private static string Optimize(int line_cnt, string mem_name, DataType type)
        {
            string target = null;
            string type_str = "W";
            string addr = mem_name;
            int key_int;
            string row_str = " (row " + line_cnt + ")";
            if (String.IsNullOrEmpty(addr))
                throw new ArgumentException("Memory variable name is empty." + row_str);
            addr = addr.ToUpper();
            addr.Replace(".", "");

            bool is_1byte_over = !(type == DataType.BIT || type == DataType.BOOL || type == DataType.BYTE || type == DataType.SBYTE);
            if (!XGTMemoryExpression.MemExpDictionary.ContainsKey(addr[0]))
                throw new ArgumentException("Not supported [" + addr[0] + "] device." + row_str);

            var allow_device = AllowDevice.Select(x => x == addr[0]);
            if (allow_device == null)
                throw new ArgumentException("Not supported [" + addr[0] + "] device." + row_str);

            MemoryExpression mem_exp = XGTMemoryExpression.MemExpDictionary[addr[0]];
            if (((mem_exp & MemoryExpression.BIT) != 0) && is_1byte_over)
                throw new ArgumentException("Invalid memory variable string.");

            if (!Int32.TryParse(addr.Substring(1, addr.Length - 1 - (is_1byte_over ? 0 : 1)), out key_int))
                throw new ArgumentException("Invalid memory variable string." + row_str);

            switch (type)
            {
                case DataType.BIT:
                case DataType.BOOL:
                    target = addr.Substring(0, addr.Length - 1);
                    break;

                case DataType.BYTE:
                case DataType.SBYTE:

                    if (!(addr.Last() == '0' || addr.Last() == '8'))
                        throw new ArgumentException("Invalid memory variable string. the byte/sbyte addresses must end in 0 ~ F." + row_str);
                    if ((mem_exp & MemoryExpression.WORD) != 0)
                    {
                        if (!Int32.TryParse(addr.Substring(1, addr.Length - 1), out key_int))
                            throw new ArgumentException("Invalid memory variable string." + row_str);
                        int b = addr.Last() == '0' ? 0 : 1;
                        target = addr[0] + ((key_int * 2) + b).ToString();
                    }
                    else
                    {
                        target = addr;
                    }
                    type_str = "B";
                    break;

                case DataType.SHORT:
                case DataType.WORD:
                    target = addr;
                    break;

                case DataType.DWORD:
                case DataType.INT:
                case DataType.LWORD:
                case DataType.LONG:
                    if (!Int32.TryParse(addr.Substring(1, addr.Length - 1), out key_int))
                        throw new ArgumentException("Invalid memory variable string." + row_str);
                    type_str = (type == DataType.DWORD || type == DataType.INT) ? "D" : "L";
                    int j = (type == DataType.DWORD || type == DataType.INT) ? 2 : 4;
                    if (key_int % j != 0)
                        throw new ArgumentException("Address of the memory must be a multiple of " + j + "." + row_str);
                    target = addr[0] + (key_int / j).ToString();
                    break;

                default:
                    throw new NotImplementedException();
            }

            target = target.Insert(0, "%");
            target = target.Insert(2, type_str);
            return target;
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

            Dictionary<string, DataType> ret = new Dictionary<string, DataType>();
            int line_cnt = 1;
            foreach (var i in items)
            {
                var type = i.GetDataType();
                string target = Optimize(line_cnt++, i.GetAddress(), i.GetDataType());
                if (!ret.ContainsKey(target))
                    ret.Add(target, ((type == DataType.BIT) || (type == DataType.BOOL)) ? DataType.WORD : type);
            }
            return ret;
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
            Dictionary<string, bool[]> forSaveMem = new Dictionary<string, bool[]>();
            int output;
            int line_cnt = 1;
            foreach (var i in items)
            {
                string addr = i.GetAddress();
                DataType type = i.GetDataType();
                string glopa_addr = Optimize(line_cnt++, addr, type);
                if (!recv_data.ContainsKey(glopa_addr))
                    continue;
                object value = recv_data[glopa_addr];
                switch (type)
                {
                    case DataType.BIT:
                    case DataType.BOOL:
                        if (Int32.TryParse("0x" + addr.Last(), out output))
                        {
                            if (forSaveMem.ContainsKey(addr))
                                i.SetValue(forSaveMem[addr][output]);
                            else
                            {
                                bool[] bits = B2W.ToBits((ushort)value);
                                i.SetValue(bits[output]);
                                forSaveMem.Add(addr, bits);
                            }
                        }
                        else
                            throw new IndexOutOfRangeException("Invalid address string.(address string must end with a 0-F)");
                        break;
                    case DataType.BYTE:
                    case DataType.SBYTE:
                    case DataType.SHORT:
                    case DataType.WORD:
                    case DataType.DWORD:
                    case DataType.INT:
                    case DataType.LONG:
                    case DataType.LWORD:
                        i.SetValue(value);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}