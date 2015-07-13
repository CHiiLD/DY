using System;

namespace DY.NET.DATALOGIC.MATRIX
{
    /// <summary>
    /// Matrix200Queue 객체의 이벤트 인자로 사용하는 이벤트 아큐먼트 클래스
    /// </summary>
    public class Matrix200EventArgs : EventArgs
    {
        public Matrix200Code CodeInfo { get; private set; }
        public Matrix200Queue.Todo Todo { get; private set; }

        public Matrix200EventArgs(Matrix200Queue.Todo todo, Matrix200Code code_info)
        {
            Todo = todo;
            CodeInfo = code_info;
        }

        public Matrix200EventArgs()
        {
            CodeInfo = null;
        }
    }
}
