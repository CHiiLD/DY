using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public class WriteTimeoutException : TimeoutException
    {
        public WriteTimeoutException(string message)
            : base(message)
        {
        }

        public WriteTimeoutException()
            : base()
        {
        }
    }
}
