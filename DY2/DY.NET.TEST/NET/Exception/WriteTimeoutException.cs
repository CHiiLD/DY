using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    /// <summary>
    /// 쓰기시간초과 예외 클래스
    /// </summary>
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