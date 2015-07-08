using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.DATALOGIC.MATRIX
{
    public class Matrix200DataReceivedEventArgs : EventArgs
    {
        public ProcessingInfo Data { get; private set; }
        public Matrix200Queue.Todo Todo { get; private set; }

        public Matrix200DataReceivedEventArgs(Matrix200Queue.Todo todo, ProcessingInfo data)
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
