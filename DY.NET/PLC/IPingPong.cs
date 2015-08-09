using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    /// <summary>
    /// IPingPong.PingAsync의 음수 리턴값 열거
    /// </summary>
    public enum PingPongState
    {
        DISCONNECT = -1,
        TIMEOUT = -2,
        COMM_ERROR = -3
    }

    /// <summary>
    /// 통신의 속도 측정 및 통신 가능 여부를 체크
    /// </summary>
    public interface IPingPong
    {
        /// <summary>
        /// 서버와 통신하여 통신 속도를 측정한다. 
        /// </summary>
        /// <param name="args">프로토콜 생성에 필요한 아규먼트</param>
        /// <returns> 
        /// -1: Disconnect 상태
        /// -2: 타임아웃
        /// -3: 프로토콜 통신 에러
        /// 0>=: 요청 시 응답까지의 속도
        /// </returns>
        Task<long> PingAsync(object args);
    }
}