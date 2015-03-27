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
    }
}