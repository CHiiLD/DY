using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    /// <summary>
    /// 읽기타임아웃 예외 클래스
    /// </summary>
    public class ReadTimeoutException : TimeoutException
    {
        public ReadTimeoutException(string message)
            : base(message)
        {
        }

        public ReadTimeoutException(TimeoutException timeoutException)
            : base(timeoutException.Message, timeoutException.InnerException)
        {

        }

        public ReadTimeoutException()
            : base()
        {
        }
    }
}
