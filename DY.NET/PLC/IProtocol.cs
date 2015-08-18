/*
 * 작성자: CHILD	
 * 기능: 네트워크 기능에 쓰이는 프로토콜의 베이스 구조를 인터페이스로 구현
 * 날짜: 2015-03-25
 */

using System;
using System.Collections.Generic;

namespace DY.NET
{
    /// <summary>
    /// PLC에서 보낼 원시 프로토콜 데이터는 반드시 이 인터페이스를 상속받아 구현합니다.
    /// 일반적인 Socket에서의 이벤트 설정과 달리, 유연한 이벤트 제어를 위해 프로토콜 단위로 이벤트를 설정합니다.
    /// </summary>
    public interface IProtocol : ITag, IDebug
    {
        Dictionary<string, object> DrawTickets();
    }
}