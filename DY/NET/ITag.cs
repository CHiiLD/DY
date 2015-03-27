/*
작성자: CHILD	
기능: 최상위 부모 클래스
날짜: 2015-03-25
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface ITag
    {
        int Tag
        {
            get;
            set;
        }
        string Description
        {
            get;
            set;
        }
        object UserData
        {
            get;
            set;
        }
    }
}
