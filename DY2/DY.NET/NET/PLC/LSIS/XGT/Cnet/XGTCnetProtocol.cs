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
        //FRAME INFORMATION
        public ControlChar Header { set; get; }
        public ushort LocalPort { set; get; }
        public XGTCnetCommand Command { set; get; }
        public XGTCnetCommandType CommandType { protected set; get; }
        public ControlChar Tail { set; get; }
        public XGTCnetError Error { get; set; }
        //DATA INFORMATION
        public Type Type { get; set; }
        public IList<IProtocolData> Data { get; set; }

        internal XGTCnetProtocol()
        {
            Initialize();
        }

        public XGTCnetProtocol(Type type, ushort localPort, XGTCnetCommand cmd)
            : this()
        {
            Type = type;
            LocalPort = localPort;
            Command = cmd;
            CommandType = XGTCnetCommandType.SS;
        }

        public static XGTCnetProtocol CreateRequestRSS(Type type, ushort localPort, IList<IProtocolData> items)
        {
            return new XGTCnetProtocol(type, localPort, XGTCnetCommand.R) { Type = type, Data = items };
        }

        public static XGTCnetProtocol CreateRequestWSS(Type type, ushort localPort, IList<IProtocolData> items)
        {
            return new XGTCnetProtocol(type, localPort, XGTCnetCommand.W) { Data = items };
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
            Header = ControlChar.NONE;
            LocalPort = ushort.MaxValue;
            Command = XGTCnetCommand.NONE;
            CommandType = XGTCnetCommandType.NONE;
            Tail = ControlChar.NONE;
            Error = XGTCnetError.OK;

            Data = null;
            this.Type = null;
        }
    }
}