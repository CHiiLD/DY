using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public class NetErrorReceivedEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public object Clue { get; private set; }

        public NetErrorReceivedEventArgs(string message, object error_args)
        {
            Message = message;
            Clue = error_args;
        }
    }
}
