using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.SAMPLE.LOGIC
{
    public class IntegerReceivedToPlcEventArgs : EventArgs
    {
        private object _Value;
        public object Value { get { return _Value; } }
        public IntegerReceivedToPlcEventArgs(object value)
        {
            _Value = value;
        }
    }
}
