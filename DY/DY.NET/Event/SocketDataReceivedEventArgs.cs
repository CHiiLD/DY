﻿/*
작성자: CHILD	
기능: IProtocol을 위한 이벤트 핸들러 매개변수 클래스
날짜: 2015-03-25
*/

using System;
using System.Threading.Tasks;

namespace DY.NET
{
    /// <summary>
    /// EventHandler 제네릭 타입을 위한 클래스입니다.
    /// </summary>
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