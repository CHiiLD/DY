using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTOptimization
    {
        public IList<IProtocolDataWithType>[] Package(IList<IProtocolDataWithType> list)
        {
            var newList = new List<IProtocolDataWithType>(list);
            //글로파 변수 문자열로 변환
            ConvertGlopaVariableName(newList);
            //Type에 맞게 한 상자 16개 팩으로 포장
            IList<IProtocolDataWithType>[] result = PackProtocolData(newList);
            return result;
        }

        public void Match(IList<IProtocolDataWithType> list, IList<IProtocolData> receivedData)
        {
            List<IProtocolData> recvList = receivedData.ToList();
            foreach (var item in list)
            {
                string glopa = item.Address;
                Type type = item.Type;
                if (type == typeof(bool))
                    glopa = ConvertBitAddrToWordAddr(item.Address); //M0000F -> M0000
                glopa = ToGlopaVariableName(glopa, item.Type); // M0000-> %MW0000
                IProtocolData search_ret = recvList.Find(x => x.Address == glopa);
                object value = search_ret.Value;
                if (type == typeof(bool))
                    value = SearchBooleanFromUInt16(item.Address, (ushort)value);
                item.Value = value;
            }
        }

        public IList<IProtocolDataWithType>[] PackProtocolData(IList<IProtocolDataWithType> newList)
        {
            ILookup<int, IProtocolDataWithType> look = SeparateProtocol(newList);
            List<List<IProtocolDataWithType>> package = new List<List<IProtocolDataWithType>>();
            foreach (IGrouping<int, IProtocolDataWithType> groups in look)
            {
                List<IProtocolDataWithType> pack = new List<IProtocolDataWithType>();
                int cnt = 0;
                foreach (IProtocolDataWithType group in groups)
                {
                    if (cnt % 16 == 0 && cnt != 0)
                    {
                        package.Add(pack);
                        pack = new List<IProtocolDataWithType>();
                        cnt = 0;
                    }
                    if (pack.Find(x => x.Address == group.Address) == default(IProtocolDataWithType)) //중복 방지
                    {
                        pack.Add(new DetailProtocolData(group.Address, group.Type));
                        cnt++;
                    }
                }
                package.Add(pack);
            }
            List<IProtocolDataWithType>[] result = package.ToArray();
            return result;
        }

        public void ConvertGlopaVariableName(IList<IProtocolDataWithType> newList)
        {
            foreach (var item in newList)
            {
                if (item.Type == typeof(bool))
                {
                    item.Address = ConvertBitAddrToWordAddr(item.Address); //M0000F -> M0000
                    item.Value = typeof(ushort);
                }
                item.Address = ToGlopaVariableName(item.Address, item.Type); // M0000-> %MW0000
            }
        }

        public ILookup<int, IProtocolDataWithType> SeparateProtocol(IList<IProtocolDataWithType> list)
        {
            return null;
        }

        public string ConvertBitAddrToWordAddr(string bitAddr)
        {
            return null;
        }

        public bool SearchBooleanFromUInt16(string bitAddr, ushort value)
        {
            return false;
        }

        public string ToGlopaVariableName(string normalAddr, Type type)
        {
            return null;
        }
    }
}
