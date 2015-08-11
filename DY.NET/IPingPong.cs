using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IPingPong : IStream
    {
        /// <summary>
        /// 서버와 통신하여 통신 속도를 측정
        /// </summary>
        /// <returns> 
        /// 0 >=: Milliseconds
        /// 0 <: DeliveryError
        /// </returns>
        Task<long> PingAsync();
    }
}