using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet Protocol Structure Information
    /// </summary>
    public class XGTCnetProtocol : IProtocol
    {
        public Type Type { get; set; }
        //FRAME INFORMATION
        public XGTCnetHeader Header { set; get; }
        public ushort LocalPort { set; get; }
        public XGTCnetCommand Command { set; get; }
        public XGTCnetCommandType CommandType { protected set; get; }
        public XGTCnetHeader Tail { set; get; }
        public XGTCnetError Error { get; set; }
        //DATA INFORMATION
        public IList<IProtocolData> Items { get; set; }

        public XGTCnetProtocol()
        {
            Initialize();
        }

        public XGTCnetProtocol(ushort localPort, XGTCnetCommand cmd)
            : this()
        {
            LocalPort = localPort;
            Command = cmd;
            CommandType = XGTCnetCommandType.SS;
        }

        public static XGTCnetProtocol CreateRequestRSS(Type type, ushort localPort, IList<IProtocolData> items)
        {
            return new XGTCnetProtocol(localPort, XGTCnetCommand.R) { Type = type, Items = items };
        }

        public static XGTCnetProtocol CreateRequestWSS(ushort localPort, IList<IProtocolData> items)
        {
            return new XGTCnetProtocol(localPort, XGTCnetCommand.W) { Items = items };
        }

        /// 프로토콜 에러코드를 반환한다. 
        /// </summary>
        /// <returns>0을 제외한 값은 에러코드</returns>
        public virtual int GetErrorCode()
        {
            return (int)Error;
        }

        /// <summary>
        /// 프로퍼티 초기화
        /// </summary>
        public virtual void Initialize()
        {
            Header = XGTCnetHeader.NONE;
            LocalPort = ushort.MaxValue;
            Command = XGTCnetCommand.NONE;
            CommandType = XGTCnetCommandType.NONE;
            Tail = XGTCnetHeader.NONE;
            Error = XGTCnetError.OK;
            Items = null;
        }
    }
}