using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public class DetailProtocolData : IProtocolDataWithType
    {
        public string Address { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }

        public DetailProtocolData(string address, Type type)
        {
            Address = address;
            Type = type;
        }
    }
}