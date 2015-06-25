using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// RSS, WSS 포맷 기반의 시리얼 통신 Cnet 요청 프로토콜 처리 클래스
    /// 여러 타입의 변수와 개수에 상관없이 사전 정의된 알고리즘과 시스템에 의해 자동처리됩니다.
    /// Bit, Byte, Word는 1 DWord는 2, LWord는 4로 취급하여 합이 16이하일 때 가장 빠른 응답을 보여줍니다.
    /// </summary>
    public class XGTCnetPost : AbstructXGTCnetFrame
    {
        public const int RSS_MAX_BLOCK_COUNT = 16;
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="Cmd">READ/WRITE</param>
        /// <param name="localport">국번</param>
        /// <param name="post">읽거나 쓸 데이터</param>
        public XGTCnetPost(XGTCnetCommand cmd, ushort localport, IEnumerable<Mail> post)
            : base()
        {
            Init(cmd, localport, post);
            SetWrap();
        }

        /// <summary>
        /// 복사 생성자
        /// </summary>
        /// <param name="that">copy 할 객체</param>
        public XGTCnetPost(XGTCnetPost that)
            : base(that)
        {

        }

        /// <summary>
        /// 헤더 붙이기
        /// </summary>
        /// <returns>header 데이터</returns>
        virtual private List<byte> GetHeader(XGTCnetCommand cmd)
        {
            List<byte> header = new List<byte>();
            header.Add(XGTCnetControlCodeType.ENQ.ToByte());
            header.AddRange(CA2C.ToASC(LocalPort));
            header.Add(cmd.ToByte());
            header.AddRange(XGTCnetCommandType.SS.ToByteArray());
            return header;
        }

        /// <summary>
        /// 테일 설정
        /// </summary>
        /// <param name="data"></param>
        virtual private void SetTail(List<byte> data)
        {
            data.Add(XGTCnetControlCodeType.EOT.ToByte());
        }

        /// <summary>
        /// 블럭 수 설정
        /// </summary>
        /// <param name="data"></param>
        /// <param name="block"></param>
        virtual private void SetBlockNumber(List<byte> data, int block)
        {
            data.InsertRange(6, CA2C.ToASC(block, typeof(ushort))); //블록 수
        }

        virtual private void Init(XGTCnetCommand cmd, ushort localport, IEnumerable<Mail> post)
        {
            Cmd = cmd;
            LocalPort = localport;
            if (post != null)
                Post.AddRange(post);
        }

        /// <summary>
        /// 쓰거나 저장할 데이터들을 적절하게 포장
        /// </summary>
        private void SetWrap()
        {
            int block_cnt = 0;
            List<byte> p_temp = GetHeader(Cmd);
            if (Post != null)
            {
                for (int i = 0; i < Post.Count(); i++)
                {
                    List<Mail> sealedMails = PostAlgorithm.SealingWax(Post[i]);
                    WrappedPost.AddRange(sealedMails);
                    if (block_cnt + sealedMails.Count() > RSS_MAX_BLOCK_COUNT)
                    {
                        SetBlockNumber(p_temp, block_cnt);
                        SetTail(p_temp);
                        Protocols.Add(p_temp); //저장
                        p_temp = GetHeader(Cmd);
                        block_cnt = 0;
                        i--;
                        continue;
                    }
                    foreach (var m4w in sealedMails)
                    {
                        p_temp.AddRange(CA2C.ToASC(m4w.Name.Length, typeof(ushort))); //변수 길이
                        p_temp.AddRange(CA2C.ToASC(m4w.Name));                        //변수 이름
                        if (Cmd == XGTCnetCommand.W)
                            p_temp.AddRange(CA2C.ToASC(m4w.Value));                   //값
                        block_cnt++;
                    }
                }
                SetBlockNumber(p_temp, block_cnt);
                SetTail(p_temp);
                Protocols.Add(p_temp); //저장
            }
        }
    }
}