using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DY.NET
{
    /// <summary>
    /// 프로토콜 IO 관리 인터페이스
    /// </summary>
    public interface ICommIOData : INotifyPropertyChanged
    {
        DataType Type { get; set; }
        string Address { get; set; }
        object Input { get; set; }
        object Output { get; set; }
        string Comment { get; set; }

        //void SetOutput(object output);
        //string GetAddress();
        //DataType GetDataType();
    }
}