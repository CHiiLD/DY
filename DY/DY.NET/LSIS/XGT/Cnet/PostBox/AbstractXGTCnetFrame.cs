using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGTCnetPost, Reply 상위 추상 클래스
    /// </summary>
    public abstract class AbstractXGTCnetFrame : IProtocol
    {
        public int Tag { get; set; }
        public string Description { get; set; }
        public object UserData { get; set; }
        public event EventHandler<SocketDataReceivedEventArgs> ErrorReceived;
        public event EventHandler<SocketDataReceivedEventArgs> DataRequested;
        public event EventHandler<SocketDataReceivedEventArgs> DataReceived;
        /// <summary>
        /// Requested 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        public void OnDataReceived(object obj, IProtocol protocol)
        {
            var pt = System.Threading.Volatile.Read(ref protocol);
            if (DataReceived != null)
                DataReceived(obj, new SocketDataReceivedEventArgs(pt));
        }
        /// <summary>
        /// Requested 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        public void OnDataRequested(object obj, IProtocol protocol)
        {
            var pt = System.Threading.Volatile.Read(ref protocol);
            if (DataRequested != null)
                DataRequested(obj, new SocketDataReceivedEventArgs(pt));
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
                ErrorReceived(obj, new SocketDataReceivedEventArgs(pt));
        }

        public XGTCnetCommand Cmd { get; internal set; }
        public XGTCnetCommandType CmdType { get; internal set; }
        public ushort LocalPort { get; internal set; }
        internal List<List<byte>> Protocols { get; set; } //프로토콜 전송 데이터
        internal List<Mail> Post { get; set; } //보낼 메일들
        internal List<Mail> WrappedPost { get; set; } //보내기 위해 랩핑한 메일들

        /// <summary>
        /// 초기화
        /// </summary>
        private void Init()
        {
            CmdType = XGTCnetCommandType.SS;
            Post = new List<Mail>(); //post 복사
            WrappedPost = new List<Mail>(); //post 복사
            Protocols = new List<List<byte>>();
        }

        /// <summary>
        /// 생성자 방지
        /// </summary>
        protected AbstractXGTCnetFrame()
        {
            Init();
        }

        /// <summary>
        /// 복사 생성자
        /// </summary>
        /// <param name="that"></param>
        public AbstractXGTCnetFrame(AbstractXGTCnetFrame that)
        {
            Cmd = that.Cmd;
            CmdType = that.CmdType;
            LocalPort = that.LocalPort;
            Post = that.Post;
            WrappedPost = that.WrappedPost;
            foreach (var p in Protocols)
            {
                List<byte> copy = new List<byte>();
                copy.AddRange(p);
                Protocols.Add(copy);
            }
        }
    }
}
