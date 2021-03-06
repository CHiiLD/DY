﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public static class XGTHelper
    {
        private static readonly XGTOptimizationTool m_Opt = new XGTOptimizationTool();

        public static IList<IProtocolData>[] Classify(IList<INotifyProtocolDataChanged> list)
        {
            var newList = new List<INotifyProtocolDataChanged>(list);
            //글로파 변수 문자열로 변환
            m_Opt.ChangeNonGlopa2Glopa(newList);
            //Type에 맞게 한 상자 16개 팩으로 포장
            IList<IProtocolData>[] result = m_Opt.Pack(newList);
            return result;
        }

        public static void Match(IList<INotifyProtocolDataChanged> list, IList<IProtocolData> receivedData)
        {
            List<IProtocolData> recvList = receivedData.ToList();
            foreach (var item in list)
            {
                string glopa = item.Address;
                Type type = item.Type;
                if (type == typeof(bool))
                    glopa = m_Opt.GetWordNonGlopaFormBitNonGlopa(item.Address); //M0000F -> M0000
                glopa = m_Opt.ToGlopaAddress(glopa, type == typeof(bool) ? typeof(ushort) : item.Type); // M0000-> %MW0000
                IProtocolData data = recvList.Find(x => x.Address == glopa);
                object value = data.Value;
                if (type == typeof(bool))
                    value = m_Opt.GetBooleanFormWordTypeValue(item.Address, (ushort)value);
                item.Value = value;
            }
        }
    }
}