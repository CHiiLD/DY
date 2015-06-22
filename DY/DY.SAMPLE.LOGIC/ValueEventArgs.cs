using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.SAMPLE.LOGIC
{
    public class ValueEventArgs : EventArgs
    {
        private object _Value;
        public object Value { get { return _Value; } }
        public ValueEventArgs(object value)
        {
            _Value = value;
        }
    }
}