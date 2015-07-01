using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public abstract class AXGTProtocol<T> : IProtocol
    {
        internal Dictionary<string, T> Datas { get; set; }

        #region INTENAL VARIABLE
        internal byte[] ASC2Protocol { get; set; } // 원시 프로토콜 데이터
        internal IProtocol OtherParty { get; set; } //응답 프로토콜일 경우 요청프로토콜 주소를 저장하는 변수
        //protected Type TypeParamiter { get; private set; }
        #endregion

        protected AXGTProtocol()
        {
            //TypeParamiter = typeof(T);
            Datas = new Dictionary<string, T>();
        }

        public AXGTProtocol(AXGTProtocol<T> that)
        {
            this.Tag = that.Tag;
            this.Description = that.Description;
            this.UserData = that.UserData;

            this.ProtocolReceived = that.ProtocolReceived;
            this.ErrorReceived = that.ErrorReceived;
            this.ProtocolRequested = that.ProtocolRequested;

            this.OtherParty = that.OtherParty;
            if (that.ASC2Protocol != null)
                this.ASC2Protocol = (byte[])that.ASC2Protocol.Clone();

            this.Datas = new Dictionary<string, T>(that.Datas);
        }

        public int Tag { get; set; }
        public string Description { get; set; }
        public object UserData { get; set; }

        /// <summary>
        /// 통신 중 예외 또는 에러가 발생시 통지
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> ErrorReceived;
        /// <summary>
        /// 프로토콜 요청을 성공적으로 전달되었을 시 통지
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> ProtocolRequested;
        /// <summary>
        /// 요청된 프로토콜에 따른 응답 프로토콜을 성공적으로 받았을 시 통지
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> ProtocolReceived;

        public void ProtocolReceivedEvent(object obj, IProtocol protocol)
        {
            if (ProtocolReceived != null)
            {
                var cold_pt = System.Threading.Volatile.Read(ref protocol);
                ProtocolReceived(obj, new DataReceivedEventArgs(cold_pt));
            }
        }

        public void ProtocolRequestedEvent(object obj, IProtocol protocol)
        {
            if (ProtocolRequested != null)
            {
                var cold_pt = System.Threading.Volatile.Read(ref protocol);
                ProtocolRequested(obj, new DataReceivedEventArgs(cold_pt));
            }
        }

        public void ErrorReceivedEvent(object obj, IProtocol protocol)
        {
            if (ErrorReceived != null)
            {
                var cold_pt = System.Threading.Volatile.Read(ref protocol);
                ErrorReceived(obj, new DataReceivedEventArgs(cold_pt));
            }
        }

        /// <summary>
        /// 맴머 변수의 정보를 토대로 원시 프로토콜 데이터를 구합니다.
        /// </summary>
        internal abstract void AssembleProtocol();
        /// <summary>
        /// 받은 원시 프로토콜 데이터를 바탕으로 프로토콜 구조와 데이터를 파악합니다.
        /// </summary>
        internal abstract void AnalysisProtocol();

        public abstract void Print();
    }
}
