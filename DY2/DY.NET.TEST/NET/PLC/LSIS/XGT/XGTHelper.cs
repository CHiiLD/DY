using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public static class XGTHelper
    {
        private static readonly XGTOptimizationTool m_Opt = new XGTOptimizationTool();

        public static IList<IProtocolItemWithType>[] Classify(IList<IProtocolItemWithType> list)
        {
            var newList = new List<IProtocolItemWithType>(list);
            //글로파 변수 문자열로 변환
            m_Opt.ConvertGlopaVariableName(newList);
            //Type에 맞게 한 상자 16개 팩으로 포장
            IList<IProtocolItemWithType>[] result = m_Opt.PackProtocolData(newList);
            return result;
        }

        public static void Match(IList<IProtocolItemWithType> list, IList<IProtocolItem> receivedData)
        {
            List<IProtocolItem> recvList = receivedData.ToList();
            foreach (var item in list)
            {
                string glopa = item.Address;
                Type type = item.Type;
                if (type == typeof(bool))
                    glopa = m_Opt.ConvertBitAddrToWordAddr(item.Address); //M0000F -> M0000
                glopa = m_Opt.ToGlopaVariableName(glopa, type == typeof(bool) ? typeof(ushort) : item.Type); // M0000-> %MW0000
                IProtocolItem data = recvList.Find(x => x.Address == glopa);
                object value = data.Value;
                if (type == typeof(bool))
                    value = m_Opt.SearchBooleanFromUInt16(item.Address, (ushort)value);
                item.Value = value;
            }
        }
    }
}
