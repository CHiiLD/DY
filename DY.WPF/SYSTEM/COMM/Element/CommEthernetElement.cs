using System;
using System.Text;
using System.Net.Sockets;
using DY.NET;

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// 이더넷 통신 설정 옵션
    /// </summary>
    public class CommEthernetElement : EthernetElement, ISummary
    {
        public CommEthernetElement(string host, int port, ProtocolType type)
        {
            Host = host;
            Port = port;
            Type = type;
        }

        public string GetSummary()
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