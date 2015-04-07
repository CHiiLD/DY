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
    public abstract class DYSocket : ITag
    {
        public DYSocket()
        {

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

        protected Queue<IProtocol> ProtocolStandByQueue = new Queue<IProtocol>();

        public abstract bool Connect();
        public abstract bool Close();
        public abstract void Send(IProtocol protocolFrame);
    }
}