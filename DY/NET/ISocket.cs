/*
작성자: CHILD	
기능: 통신에 필요한 최소한의 기능들을 인터페이스로 구현
날짜: 2015-03-25
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface ISocket : ITag
    {
        event SocketDataReceivedEventHandler DataReceivedEvent;
        bool Connect();
        bool Close();
        void Send(IProtocol protocolFrame);
    }
}