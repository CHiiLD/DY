using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    /// <summary>
    /// 추상 프로토콜 클래스
    /// </summary>
    public abstract class AProtocol : IProtocol
    {
        protected byte[] ProtocolData;
        public byte[] ASCIIProtocol
        {
            get
            {
                if (ProtocolData == null)
                    AssembleProtocol();
                return ProtocolData;
            }
            internal set
            {
                ProtocolData = value;
            }
        }

        /// <summary>
        /// AProtocol 클래스가 응답클래스로 쓰인 경우 요청프로토콜 클래스를 담고 
        /// AProtocol 클래스가 요청클래스로 쓰인 경우 응답프로토콜 클래스를 담는다.
        /// </summary>
        public IProtocol MirrorProtocol { get; set; }
        
        protected AProtocol() { }

        public AProtocol(AProtocol that)
        {
            this.Tag = that.Tag;
            this.Description = that.Description;
            this.UserData = that.UserData;

            this.ProtocolReceived = that.ProtocolReceived;
            this.ErrorReceived = that.ErrorReceived;
            this.ProtocolRequested = that.ProtocolRequested;

            this.MirrorProtocol = that.MirrorProtocol;
            if (that.ProtocolData != null)
                this.ProtocolData = (byte[])that.ProtocolData.Clone();
        }

        protected Dictionary<string, object> DataStorageDictionary = new Dictionary<string, object>();
        public Dictionary<string, object> GetStorage()
        {
            return new Dictionary<string, object>(DataStorageDictionary);
        }

        public Type TargetType { get; protected set; }

        /// <summary>
        /// 통신 중 예외 또는 에러가 발생시 통지
        /// </summary>
        public event EventHandler<ProtocolReceivedEventArgs> ErrorReceived;
        /// <summary>
        /// 프로토콜 요청을 성공적으로 전달되었을 시 통지
        /// </summary>
        public event EventHandler<ProtocolReceivedEventArgs> ProtocolRequested;
        /// <summary>
        /// 요청된 프로토콜에 따른 응답 프로토콜을 성공적으로 받았을 시 통지
        /// </summary>
        public event EventHandler<ProtocolReceivedEventArgs> ProtocolReceived;

        public void ProtocolReceivedEvent(object obj, IProtocol protocol)
        {
            if (ProtocolReceived != null)
            {
                var cold_pt = System.Threading.Volatile.Read(ref protocol);
                ProtocolReceived(obj, new ProtocolReceivedEventArgs(cold_pt));
            }
        }

        public void ProtocolRequestedEvent(object obj, IProtocol protocol)
        {
            if (ProtocolRequested != null)
            {
                var cold_pt = System.Threading.Volatile.Read(ref protocol);
                ProtocolRequested(obj, new ProtocolReceivedEventArgs(cold_pt));
            }
        }

        public void ErrorReceivedEvent(object obj, IProtocol protocol)
        {
            if (ErrorReceived != null)
            {
                var cold_pt = System.Threading.Volatile.Read(ref protocol);
                ErrorReceived(obj, new ProtocolReceivedEventArgs(cold_pt));
            }
        }

        public int Tag { get; set; }
        public string Description { get; set; }
        public object UserData { get; set; }

        public abstract void AssembleProtocol();
        public abstract void AnalysisProtocol();
        public abstract void Print();
    }
}
