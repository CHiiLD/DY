using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public class ReadTimeoutException : TimeoutException
    {
        public ReadTimeoutException(string message)
            : base(message)
        {
        }

        public ReadTimeoutException()
            : base()
        {
        }
    }
}
