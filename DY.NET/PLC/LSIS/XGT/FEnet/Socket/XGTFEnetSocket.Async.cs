using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace DY.NET.LSIS.XGT
{
    public sealed partial class XGTFEnetSocket
    {
        /// <summary>
        /// 서버와 통신하여 통신 속도를 측정
        /// </summary>
        /// <returns> 
        /// 0 >=: Milliseconds
        /// 0 <: DeliveryError
        /// </returns>
        public override async Task<long> PingAsync()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("%DW0", null);
            XGTFEnetProtocol cnet = XGTFEnetProtocol.NewRSSProtocol(typeof(ushort), 00, dictionary);
            Delivery delivery = await PostAsync(cnet);
            return delivery.Error == DeliveryError.SUCCESS ? delivery.DelivaryTime.ElapsedMilliseconds : -1;
        }
    }
}