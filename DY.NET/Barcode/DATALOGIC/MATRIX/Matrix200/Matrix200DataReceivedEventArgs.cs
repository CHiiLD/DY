using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.DATALOGIC.MATRIX
{
    /// <summary>
    /// Matrix200Queue 객체의 이벤트 인자로 사용하는 이벤트 아큐먼트 클래스
    /// </summary>
    public class Matrix200DataReceivedEventArgs : EventArgs
    {
        public Matrix200Code Data { get; private set; }
        public Matrix200Queue.Todo Todo { get; private set; }

        public Matrix200DataReceivedEventArgs(Matrix200Queue.Todo todo, Matrix200Code data)
        {
            Todo = todo;
            Data = data;
        }

        public Matrix200DataReceivedEventArgs()
        {
            Data = null;
        }
    }
}
