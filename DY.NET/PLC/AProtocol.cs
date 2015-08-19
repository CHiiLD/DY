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
        protected Dictionary<string, object> Tickets = new Dictionary<string, object>();

        protected byte[] ASCIIData;
        // ASCII 데이터
        public byte[] ASCIIProtocol
        {
            get
            {
                if (ASCIIData == null) AssembleProtocol();
                return ASCIIData;
            }
            internal set 
            {
                ASCIIData = value;
                Dictionary<string, object> tickets = new Dictionary<string, object>();
                foreach (var t in Tickets)
                    tickets.Add(t.Key, null);
                Tickets = tickets;
            }
        }

        // AProtocol 클래스가 응답클래스로 쓰인 경우 요청프로토콜 클래스를 담고 
        // AProtocol 클래스가 요청클래스로 쓰인 경우 응답프로토콜 클래스를 담는다.
        public IProtocol MirrorProtocol { get; set; }
        // 메모리 번지의 타입
        public Type TType { get; protected set; }

        public int Tag { get; set; }
        public object UserData { get; set; }
        public string Description { get; set; }
        
        protected AProtocol() 
        { 
        }
        
        public AProtocol(AProtocol that)
        {
            this.Tag = that.Tag;
            this.Description = that.Description;
            this.UserData = that.UserData;
            this.TType = that.TType;
            this.MirrorProtocol = that.MirrorProtocol;

            if (that.ASCIIData != null)
                this.ASCIIData = (byte[])that.ASCIIData.Clone();
        }

        public Dictionary<string, object> DrawTickets()
        {
            return new Dictionary<string, object>(Tickets);
        }
        
        public abstract void AssembleProtocol();
        public abstract void AnalysisProtocol();
        public abstract void Print();
    }
}