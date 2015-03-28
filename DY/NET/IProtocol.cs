/*
작성자: CHILD	
기능: 네트워크 기능에 쓰이는 프로토콜의 베이스 구조를 인터페이스로 구현
날짜: 2015-03-25
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IProtocol
    {
        byte[] ASCData
        {
            get;
            set;
        }

        event SocketDataReceivedEventHandler ErrorEvent;         //통신 중 생긴 에러를 알리는 이벤트
        event SocketDataReceivedEventHandler DataRequestedEvent; //데이터를 요청한 뒤에 알리는 이벤트 
        event SocketDataReceivedEventHandler DataReceivedEvent;  //요청한 뒤 받은 데이터를 알리는 이벤트
    }
}