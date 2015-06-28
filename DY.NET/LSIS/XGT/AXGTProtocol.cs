using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public abstract class AXGTProtocol : IProtocol
    {
        protected const string ERROR_ENQ_IS_NULL_OR_EMPTY = "ENQDATAS HAVE PROBLEM (NULL OR EMPTY DATA)";
        protected const string ERROR_READED_MEM_COUNT_LIMIT = "ENQDATAS OVER LIMIT OF COUNT (NULL OR EMPTY DATA)";
        protected const string ERROR_MONITER_INVALID_REGISTER_NUMBER = "REGISTER_NUMBER HAVE TO REGISTER TO 0 FROM 31";
        protected const int READED_MEM_MAX_COUNT = 16;

        public List<PValue> ReqeustList { get; private set; }
        public Dictionary<string, object> ResponseDic { get; private set; }

        #region INTENAL VARIABLE
        internal byte[] ASC2Protocol { get; set; } // 원시 프로토콜 데이터
        internal IProtocol OtherParty { get; set; } //응답 프로토콜일 경우 요청프로토콜 주소를 저장하는 변수
        #endregion

        protected AXGTProtocol()
        {
            ReqeustList = new List<PValue>();
            ResponseDic = new Dictionary<string, object>();
        }

        public AXGTProtocol(AXGTProtocol that)
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

            ReqeustList = new List<PValue>();
            ResponseDic = new Dictionary<string, object>();
            ReqeustList.AddRange(that.ReqeustList);
            foreach (var d in ResponseDic)
                ResponseDic.Add(d.Key, d.Value);
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

        /// <summary>
        /// Requested 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        public void OnDataReceived(object obj, IProtocol protocol)
        {
            var pt = System.Threading.Volatile.Read(ref protocol);
            if (ProtocolReceived != null)
                ProtocolReceived(obj, new DataReceivedEventArgs(pt));
        }
        /// <summary>
        /// Requested 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        public void OnDataRequested(object obj, IProtocol protocol)
        {
            var pt = System.Threading.Volatile.Read(ref protocol);
            if (ProtocolRequested != null)
                ProtocolRequested(obj, new DataReceivedEventArgs(pt));
        }
        /// <summary>
        /// OnError 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        public void OnError(object obj, IProtocol protocol)
        {
            var pt = System.Threading.Volatile.Read(ref protocol);
            if (ErrorReceived != null)
                ErrorReceived(obj, new DataReceivedEventArgs(pt));
        }

        /// <summary>
        /// 맴머 변수의 정보를 토대로 원시 프로토콜 데이터를 구합니다.
        /// </summary>
        internal abstract void AssembleProtocol();
        /// <summary>
        /// 받은 원시 프로토콜 데이터를 바탕으로 프로토콜 구조와 데이터를 파악합니다.
        /// </summary>
        internal abstract void AnalysisProtocol();
    }
}
