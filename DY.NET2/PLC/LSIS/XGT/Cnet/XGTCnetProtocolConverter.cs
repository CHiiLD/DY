using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetProtocolConverter : IProtocolConverter
    {
        public virtual byte[] Encode(IProtocol protocol)
        {
            //XGTCnetProtocolFrame의 데이터로 ASCII 데이터 만들어서 반환
            return null;
        }

        public virtual IProtocol Decode(byte[] ascii)
        {
            //ASCII데이터로 XGTCnetProtocolFrame를 반환 
            return null;
        }
    }
}
