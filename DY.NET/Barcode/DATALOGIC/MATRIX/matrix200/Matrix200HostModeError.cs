using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.DATALOGIC.MATRIX
{
    public enum Matrix200HostModeError
    {
        OK,

        SERIALPORT_DISCONNECTION_ERROR,

        HOST_MODE_ENTER_FAILURE_ERROR,
        PROGRAMMING_MODE_ENTER_FAILURE_ERROR,

        HOST_MODE_EXIT_FAILURE_ERROR,
        PROGRAMMING_MODE_EXIT_FAILURE_ERROR,
    }
}
