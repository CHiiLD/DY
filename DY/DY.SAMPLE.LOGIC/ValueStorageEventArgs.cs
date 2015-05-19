using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.SAMPLE.LOGIC
{
    public class ValueStorageEventArgs : EventArgs
    {
        private object _Value;
        public object Value { get { return _Value; } }
        public ValueStorageEventArgs(object value)
        {
            _Value = value;
        }
    }
}
