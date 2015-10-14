using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class ProtocolData : IProtocolData
    {
        public string Address { get; set; }
        public object Value { get; set; }

        public ProtocolData(string addr)
        {
            Address = addr;
        }

        public ProtocolData(object value)
        {
            Value = value;
        }

        public ProtocolData(string addr, object value)
        {
            Address = addr;
            Value = value;
        }
    }
}