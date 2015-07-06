using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.DATALOGIC.MATRIX
{
    public class Matrix200EventArgs : EventArgs
    {
        public Matrix200HostModeError Error;
        public Matrix200HostModeState State;
        public string Data;
    }
}
