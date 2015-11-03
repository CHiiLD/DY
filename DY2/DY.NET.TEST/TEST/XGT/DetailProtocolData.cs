using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace DY.NET.TEST
{
    public class DetailProtocolData : INotifyProtocolDataChanged
    {
        public string Address { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public DetailProtocolData(string address, Type type)
        {
            Address = address;
            Type = type;
        }
    }
}