using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.DATALOGIC.MATRIX
{
    public class Matrix200HostModeCommand
    {
        private const byte ESC = 0x1B;
        protected const byte CR = 0x0D;
        protected const byte LF = 0x0A;
        protected const byte SP = 0x20;

        public readonly byte[] R_CMD_OK = { ESC, (byte)'K', CR, LF };
        public readonly byte[] R_CMD_WRONG = { ESC, (byte)'W', (byte)'1', CR, LF };
        //connect to device
        public readonly byte[] H_CMD_ENTER_HOST_MODE = { ESC, (byte)'[', (byte)'C' };
        public readonly byte[] R_CMD_ENTER_HOST_MODE = { ESC, (byte)'H', CR, LF };

        public readonly byte[] H_CMD_ENTER_PROGRAMMING_MODE = { ESC, (byte)'[', (byte)'B' };
        public readonly byte[] R_CMD_ENTER_PROGRAMMING_MODE = { ESC, (byte)'Q', CR, LF };
        // disconnect
        public readonly byte[] CMD_END_SINGLE_PARAMETER_SEQ = { ESC, (byte)'I', (byte)'A', (byte)'#' };
        public readonly byte[] CMD_EXIT_PROG_MODE_AND_DATA_STORAGE = { ESC, (byte)'I', (byte)'A', SP };

        public readonly byte[] H_CMD_EXIT_HOST_MODE = { ESC, (byte)'[', (byte)'A' };
        public readonly byte[] R_CMD_EXIT_HOST_MODE = { ESC, (byte)'X', CR, LF };
        // reader information
        public readonly byte[] H_CMD_GET_READER_MODEL = { ESC, (byte)'[', (byte)'E' };
        public readonly byte[] H_CMD_GET_SOFTWARE_VERSION = { ESC, (byte)'[', (byte)'F' };
        public readonly byte[] H_CMD_GET_READER_NAME = { ESC, (byte)'[', (byte)'G' };
        // button function
        public readonly byte[] CMD_BTN_FUNC_1 = { ESC, (byte)'0', ESC, (byte)'I', (byte)'C', (byte)'#' };
        public readonly byte[] CMD_BTN_FUNC_2 = { ESC, (byte)'1', ESC, (byte)'I', (byte)'C', (byte)'#' };
        public readonly byte[] CMD_BTN_FUNC_3 = { ESC, (byte)'2', ESC, (byte)'I', (byte)'C', (byte)'#' };
        public readonly byte[] CMD_BTN_FUNC_4 = { ESC, (byte)'3', ESC, (byte)'I', (byte)'C', (byte)'#' };
        // other function
        public readonly byte[] CMD_DISABLE_ALL_SYMBOLOGIES = { ESC, (byte)'4', ESC, (byte)'I', (byte)'C', (byte)'#' };
        public readonly byte[] CMD_RESTORE_DEFAULT = { ESC, (byte)'5', ESC, (byte)'I', (byte)'C', (byte)'#' };
        //parameter 
        public readonly byte[] CMD_ACQ_TRIGGER_ONESHOT_1READ = { ESC, (byte)'A', (byte)'B', (byte)'1' };

        public readonly byte[] CMD_OPER_MODE_1ONESHOT = { ESC, (byte)'A', (byte)'A', (byte)'0' };
        public readonly byte[] CMD_OPER_MODE_2CONTINUOUS = { ESC, (byte)'A', (byte)'A', (byte)'1' };
        public readonly byte[] CMD_OPER_MODE_3PHASEMODE = { ESC, (byte)'A', (byte)'A', (byte)'2' };
    }
}