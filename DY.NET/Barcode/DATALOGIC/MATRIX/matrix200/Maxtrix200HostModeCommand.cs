using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.DATALOGIC.MATRIX
{
    public class Maxtrix200HostModeCommand
    {
        private const byte ESC = 0x1B;
        protected const byte CR = 0x0D;
        protected const byte LF = 0x0A;

        protected readonly byte[] R_CMD_OK = { ESC, (byte)'K', CR, LF };
        protected readonly byte[] R_CMD_WRONG = { ESC, (byte)'W', (byte)'1', CR, LF };
        //connect to device
        protected readonly byte[] H_CMD_ENTER_HOST_MODE = { ESC, (byte)'[', (byte)'C' };
        protected readonly byte[] R_CMD_ENTER_HOST_MODE = { ESC, (byte)'H', CR, LF };

        protected readonly byte[] H_CMD_ENTER_PROGRAMMING_MODE = { ESC, (byte)'[', (byte)'B' };
        protected readonly byte[] R_CMD_ENTER_PROGRAMMING_MODE = { ESC, (byte)'Q', CR, LF };
        // disconnect
        protected readonly byte[] H_CMD_END_SINGLE_PARAMETER_SEQ = { ESC, (byte)'|', (byte)'A', (byte)'#' };
        protected readonly byte[] H_CMD_EXIT_PROG_MODE_AND_DATA_STORAGE = { ESC, (byte)'|', (byte)'A', (byte)'!' };

        protected readonly byte[] H_CMD_EXIT_HOST_MODE = { ESC, (byte)'[', (byte)'A' };
        protected readonly byte[] R_CMD_EXIT_HOST_MODE = { ESC, (byte)'X', CR, LF };

        protected readonly byte[] H_CMD_GET_READER_MODEL = { ESC, (byte)'[', (byte)'E' };
        protected readonly byte[] H_CMD_GET_SOFTWARE_VERSION = { ESC, (byte)'[', (byte)'F' };
        protected readonly byte[] H_CMD_GET_READER_NAME = { ESC, (byte)'[', (byte)'G' };

        public readonly byte[] CMD_OPER_READING_PARSE_ON = { ESC, (byte)'A', (byte)'B' };
        public enum READING_PARSE_ON_DATA
        {
            EXT_TRAG_READ_EDGE = 1,
            EXT_TRAG_TRAILING_EDGE = 2,
            MAIN_PORT_STRING = 4,
            AUXILIARY_PORT_STRING = 8,
            INPUT_2_LEADING_EDGE = 16,
            INPUT_2_TRAILING_EDGE = 32,
            ETHERNET_STRING = 64,
            FIELDBUS_STRING = 128,
            FIELDBUS_INPUT_READING_EDGE = 256,
            FIELDBUS_INPUT_TRAILING_EDGE = 512,
            IDNET_STRING = 1024,
        }
    }
}