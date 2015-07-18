using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// 이더넷 통신 설정 옵션
    /// </summary>
    public class CommEthernetParameter : ISummaryParameter
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public ProtocolType Type { get; set; }

        public string GetParameterSummaryString()
        {
            StringBuilder sb = new StringBuilder(Type.ToString());
            sb.Append(' ');
            sb.Append(Host);
            sb.Append(':');
            sb.Append(Port.ToString());
            return sb.ToString();
        }
    }
}