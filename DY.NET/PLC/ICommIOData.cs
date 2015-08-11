using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    /// <summary>
    /// 프로토콜 IO 관리 인터페이스
    /// </summary>
    public interface ICommIOData
    {
        void SetOutput(object output);
        string GetAddress();
        DataType GetDataType();
    }
}