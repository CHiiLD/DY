/*
 * 작성자: CHILD	
 * 기능: 네트워크 기능에 쓰이는 프로토콜의 베이스 구조를 인터페이스로 구현
 * 날짜: 2015-03-25
 */

using System;

namespace DY.NET
{
    /// <summary>
    /// PLC에서 보낼 원시 프로토콜 데이터는 반드시 이 인터페이스를 상속받아 구현합니다.
    /// 일반적인 Socket에서의 이벤트 설정과 달리, 유연한 이벤트 제어를 위해 프로토콜 단위로 이벤트를 설정합니다.
    /// </summary>
    public interface IProtocol : ITag
    {
        /// <summary>
        /// 통신 중 예외 또는 에러가 발생시 통지
        /// </summary>
        event EventHandler<SocketDataReceivedEventArgs> ErrorReceived;         
        /// <summary>
        /// 프로토콜 요청을 성공적으로 전달되었을 시 통지
        /// </summary>
        event EventHandler<SocketDataReceivedEventArgs> DataRequested; 
        /// <summary>
        /// 요청된 프로토콜에 따른 응답 프로토콜을 성공적으로 받았을 시 통지
        /// </summary>
        event EventHandler<SocketDataReceivedEventArgs> DataReceived;

        /// <summary>
        /// RequestedEvent 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        void OnDataReceived(object obj, IProtocol protocol);
        /// <summary>
        /// OnDataRequestedEvent 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        void OnDataRequested(object obj, IProtocol protocol);
        /// <summary>
        /// OnErrorEvent 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="obj"> DYSocekt 클래스 객체 </param>
        /// <param name="protocol"> IProtocol 인터페이스 객체 </param>
        void OnError(object obj, IProtocol protocol);
    }
}