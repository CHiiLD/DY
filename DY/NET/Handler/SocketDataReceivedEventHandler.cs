/*
작성자: CHILD	
기능: ISocket에서 데이터를 수신할 시 발생시킬 이벤트 핸들러 대리자
날짜: 2015-03-25
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public delegate void SocketDataReceivedEventHandler(object socket, SocketDataReceivedEventArgs args);
}
