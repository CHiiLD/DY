using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT.Cnet
{
    /// <summary>
    /// RSS, WSS 포맷 기반의 시리얼 통신 Cnet 응답 프로토콜 처리 클래스
    /// </summary>
    public class XGTCnetReply : AbstructXGTCnetFrame
    {
        private int ProtocolCount;

        /// <summary>
        /// 응답 프로토콜을 저장한다. 
        /// </summary>
        /// <param name="data">시리얼포트에서 받은 데이터</param>
        /// <returns>요청된 프로토콜에 대응하여 응답 프로토콜이 전부 도착하였다면 true를 반환</returns>
        public bool AddProtocolData(IEnumerable<byte> data)
        {
            List<byte> copy = new List<byte>();
            copy.AddRange(data);
            Protocols.Add(copy);
            return Protocols.Count() == ProtocolCount;
        }

        /// <summary>
        /// 복사 생성자
        /// </summary>
        /// <param name="that"></param>
        private XGTCnetReply(XGTCnetReply that)
            : base(that)
        {

        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="post">요청한 XGTCnetPost 객체</param>
        internal XGTCnetReply(XGTCnetPost post) : base()
        {
            LocalPort = post.LocalPort;
            Post = post.Post;
            ProtocolCount = post.Protocols.Count();
            Protocols = new List<List<byte>>();
        }
    }
}