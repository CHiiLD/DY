/*
작성자: CHILD	
기능: SocketDataReceivedEventHandler 대리자의 event args의 상속 클래스
날짜: 2015-03-25
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public class SocketDataReceivedEventArgs : EventArgs, ITag
    {
        public SocketDataReceivedEventArgs(IProtocol protocol)
        {
            Protocol = protocol;
        }

        public IProtocol Protocol
        {
            get;
            set;
        }

        public int Tag
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public object UserData
        {
            get;
            set;
        }
    }
}
