namespace DY.NET.DATALOGIC.MATRIX
{
    public class Matrix200Command
    {
#if false
        private const byte ESC = 0x1B;
        protected const byte CR = 0x0D;
        protected const byte LF = 0x0A;
        protected const byte SP = 0x20;

        protected readonly byte[] R_CMD_OK = { ESC, (byte)'K', CR, LF };
        protected readonly byte[] R_CMD_WRONG = { ESC, (byte)'W', (byte)'1', CR, LF };
        //connect to device
        protected readonly byte[] H_CMD_ENTER_HOST_MODE = { ESC, (byte)'[', (byte)'C' };
        protected readonly byte[] R_CMD_ENTER_HOST_MODE = { ESC, (byte)'H', CR, LF };

        protected readonly byte[] H_CMD_ENTER_PROGRAMMING_MODE = { ESC, (byte)'[', (byte)'B' };
        protected readonly byte[] R_CMD_ENTER_PROGRAMMING_MODE = { ESC, (byte)'Q', CR, LF };
        // disconnect
        protected readonly byte[] CMD_END_SINGLE_PARAMETER_SEQ = { ESC, (byte)'I', (byte)'A', (byte)'#' };
        protected readonly byte[] CMD_EXIT_PROG_MODE_AND_DATA_STORAGE = { ESC, (byte)'I', (byte)'A', SP };

        protected readonly byte[] H_CMD_EXIT_HOST_MODE = { ESC, (byte)'[', (byte)'A' };
        protected readonly byte[] R_CMD_EXIT_HOST_MODE = { ESC, (byte)'X', CR, LF };
        // reader information
        protected readonly byte[] H_CMD_GET_READER_MODEL = { ESC, (byte)'[', (byte)'E' };
        protected readonly byte[] H_CMD_GET_SOFTWARE_VERSION = { ESC, (byte)'[', (byte)'F' };
        protected readonly byte[] H_CMD_GET_READER_NAME = { ESC, (byte)'[', (byte)'G' };
        // button function
        protected readonly byte[] CMD_BTN_FUNC_1 = { ESC, (byte)'0', ESC, (byte)'I', (byte)'C', (byte)'#' };
        protected readonly byte[] CMD_BTN_FUNC_2 = { ESC, (byte)'1', ESC, (byte)'I', (byte)'C', (byte)'#' };
        protected readonly byte[] CMD_BTN_FUNC_3 = { ESC, (byte)'2', ESC, (byte)'I', (byte)'C', (byte)'#' };
        protected readonly byte[] CMD_BTN_FUNC_4 = { ESC, (byte)'3', ESC, (byte)'I', (byte)'C', (byte)'#' };
        // other function
        protected readonly byte[] CMD_DISABLE_ALL_SYMBOLOGIES = { ESC, (byte)'4', ESC, (byte)'I', (byte)'C', (byte)'#' };
        protected readonly byte[] CMD_RESTORE_DEFAULT = { ESC, (byte)'5', ESC, (byte)'I', (byte)'C', (byte)'#' };
        //parameter 
        protected readonly byte[] CMD_ACQ_TRIGGER_ONESHOT_1READ = { ESC, (byte)'A', (byte)'B', (byte)'1' };

        protected readonly byte[] CMD_OPER_MODE_1ONESHOT = { ESC, (byte)'A', (byte)'A', (byte)'0' };
        protected readonly byte[] CMD_OPER_MODE_2CONTINUOUS = { ESC, (byte)'A', (byte)'A', (byte)'1' };
        protected readonly byte[] CMD_OPER_MODE_3PHASEMODE = { ESC, (byte)'A', (byte)'A', (byte)'2' };
#endif
        //Visiset 패킷 캐치 코드
        protected readonly byte[] CMD_VISISET_CONNECT = { 
        0x1B, 0x5B, 0x43, 0x02, 0x00, 0x04, 0x00, 0x04, 0x06, 0x06, 
        0x06, 0x06, 0x03, 0x00, 0x25, 0x1B, 0x5B, 0x41, 0x02, 0x00, 0x01, 0x00, 0x48, 0x00, 0x03, 0x00, 0x4E, 0x02, 0x00, 0x01, 0x00, 0x1E, 0x00, 
        0x03, 0x00, 0x24, };
        protected readonly byte[] CMD_VISISET_DISCONNECT = { 0x02, 0x00, 0x02, 0xFE, 0x00, 0x0E, 0x00, 0x00, 0x03, 0x00, 0x15 };
        protected readonly byte[] CMD_VISISET_CAPTURE =  { 0x02, 0x00, 0x01, 0x00, 0x04, 0x42, 0x03, 0x00, 0x4C };
        protected readonly byte[] CMD_VISISET_DECODING = { 0x02, 0x00, 0x01, 0x00, 0x04, 0x43, 0x03, 0x00, 0x4D };
        protected readonly byte[] CMD_VISISET_LEARN = { 0x02, 0x00, 0x05, 0x00, 0x04, 0x59, 0x31, 0x2C, 0x34, 0x00, 0x03, 0x00, 0xF8};
    }
}