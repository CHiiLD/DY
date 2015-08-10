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
        //데이터 박스
        protected Dictionary<string, object> StorageDictionary = new Dictionary<string, object>();

        protected byte[] ProtocolData;
        // ASCII 데이터
        public byte[] ASCIIProtocol
        {
            get
            {
                if (ProtocolData == null) AssembleProtocol();
                return ProtocolData;
            }
            internal set { ProtocolData = value; }
        }

        // AProtocol 클래스가 응답클래스로 쓰인 경우 요청프로토콜 클래스를 담고 
        // AProtocol 클래스가 요청클래스로 쓰인 경우 응답프로토콜 클래스를 담는다.
        public IProtocol MirrorProtocol { get; set; }
        // 메모리 번지의 타입
        public Type TType { get; protected set; }

        public int Tag { get; set; }
        public object UserData { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// 프로토콜 요청을 성공적으로 전달되었을 시 통지
        /// </summary>
        public event EventHandler<ProtocolReceivedEventArgs> ProtocolRequested;
        /// <summary>
        /// 요청된 프로토콜에 따른 응답 프로토콜을 성공적으로 받았을 시 통지
        /// </summary>
        public event EventHandler<ProtocolReceivedEventArgs> ProtocolReceived;
        
        protected AProtocol() 
        { 
        }
        
        public AProtocol(AProtocol that)
        {
            this.Tag = that.Tag;
            this.Description = that.Description;
            this.UserData = that.UserData;

            this.ProtocolReceived = that.ProtocolReceived;
            this.ProtocolRequested = that.ProtocolRequested;

            this.MirrorProtocol = that.MirrorProtocol;
            if (that.ProtocolData != null)
                this.ProtocolData = (byte[])that.ProtocolData.Clone();
        }

        public Dictionary<string, object> GetStorage()
        {
            return new Dictionary<string, object>(StorageDictionary);
        }

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

      
        public abstract void AssembleProtocol();
        public abstract void AnalysisProtocol();
        public abstract void Print();
    }
}