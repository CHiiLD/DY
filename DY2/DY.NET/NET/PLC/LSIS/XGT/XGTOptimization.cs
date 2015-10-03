using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTOptimizationTool
    {
        public IList<IProtocolData>[] Pack(IList<IProtocolDataWithType> newList)
        {
            List<List<IProtocolData>> result = new List<List<IProtocolData>>();
            ILookup<Type, IProtocolDataWithType> look = SeparateProtocol(newList);
            
            foreach (IGrouping<Type, IProtocolDataWithType> groups in look)
            {
                List<IProtocolData> pack = new List<IProtocolData>();
                result.Add(pack);
                int cnt = 0;
                foreach (IProtocolDataWithType group in groups)
                {
                    if (cnt % 16 == 0 && cnt != 0)
                    {
                        pack = new List<IProtocolData>();
                        result.Add(pack);
                    }
                    bool alreadyExist = false;
                    foreach (var item in result)
                    {
                        if (item.Find(x => x.Address == group.Address) != null)
                        {
                            alreadyExist = true;
                            break;
                        }
                    }
                    if (!alreadyExist)
                    {
                        pack.Add(new ProtocolData(group.Address));
                        cnt++;
                    }
                }
            }
            return result.ToArray();
        }

        public void ConvertGlopaVariableName(IList<IProtocolDataWithType> newList)
        {
            foreach (var item in newList)
            {
                if (item.Type == typeof(bool))
                {
                    item.Address = ConvertBitAddrToWordAddr(item.Address); //M0000F -> M0000
                    item.Type = typeof(ushort);
                }
                item.Address = ToGlopaVariableName(item.Address, item.Type); // M0000-> %MW0000
            }
        }

        public ILookup<Type, IProtocolDataWithType> SeparateProtocol(IList<IProtocolDataWithType> list)
        {
            ILookup<Type, IProtocolDataWithType> look = list.ToLookup(x => x.Type, x => x);
            return look;
        }

        public string ConvertBitAddrToWordAddr(string bitAddr)
        {
            string wordAddr = bitAddr.Replace(".", "");
            wordAddr = wordAddr.Substring(0, wordAddr.Length - 1);
            return wordAddr;
        }

        public bool SearchBooleanFromUInt16(string addr, ushort value)
        {
            int idx = Convert.ToInt32(addr.Last().ToString(), 16);
            int boolean = value & ((UInt16)1 << idx);
            return boolean == 0 ? false : true;
        }

        public string ToGlopaVariableName(string normalAddr, Type type)
        {
            string result = null;
            string type_char = null;
            string addr = normalAddr.ToUpper().Replace(".", "");
            int demical_int;
            bool has_hex = !(type == typeof(bool) || type == typeof(byte) || type == typeof(sbyte));
            int mem_size = XGTServiceableDevice.MemoryTerritorySize[addr[0]];
            MemoryExpression mem_exp = XGTMemoryExpression.MemExpDictionary[addr[0]];
            string demical_str = addr.Substring(1, addr.Length - 1 - (has_hex ? 0 : 1));

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

            if (type == typeof(bool))
            {
                result = addr;
                type_char = "X";
            }
            else if (type == typeof(sbyte) || type == typeof(byte))
            {
                if (!(addr.Last() == '0' || addr.Last() == '8'))
                    throw new ArgumentException("Invalid memory variable string. the byte/sbyte addresses must end in 0 or 8");
                int b = addr.Last() == '0' ? 0 : 1;
                result = addr[0] + ((demical_int * 2) + b).ToString();
                type_char = "B";
            }
            else if (type == typeof(ushort) || type == typeof(short))
            {
                result = addr;
                type_char = "W";
            }
            else if (type == typeof(uint) || type == typeof(int) || type == typeof(ulong) || type == typeof(long))
            {
                type_char = (type == typeof(uint) || type == typeof(int)) ? "D" : "L";
                int j = type_char == "D" ? 2 : 4;
                if (demical_int % j != 0)
                    throw new ArgumentException("Address of the memory must be a multiple of " + j + ".");
                result = addr[0] + (demical_int / j).ToString();
            }
            else
            {
                throw new NotImplementedException();
            }
            result = result.Insert(0, "%").Insert(2, type_char);
            return result;
        }
    }
}