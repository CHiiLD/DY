namespace DY.NET.DATALOGIC.MATRIX
{
    /// <summary>
    /// Matrix200과 통신에 쓰일 메세지
    /// </summary>
    public class Matrix200Command
    {
#if true
        private const byte ESC = 0x1B;
        protected const byte CR = 0x0D;
        protected const byte LF = 0x0A;
        protected const byte SP = 0x20;

        protected static readonly byte[] R_CMD_OK = { ESC, (byte)'K', CR, LF };
        protected static readonly byte[] R_CMD_WRONG = { ESC, (byte)'W', (byte)'1', CR, LF };
        //connect to device
        protected static readonly byte[] H_CMD_ENTER_HOST_MODE = { ESC, (byte)'[', (byte)'C' };
        protected static readonly byte[] R_CMD_ENTER_HOST_MODE = { ESC, (byte)'H', CR, LF };

        protected static readonly byte[] H_CMD_ENTER_PROGRAMMING_MODE = { ESC, (byte)'[', (byte)'B' };
        protected static readonly byte[] R_CMD_ENTER_PROGRAMMING_MODE = { ESC, (byte)'Q', CR, LF };
        // disconnect
        protected static readonly byte[] CMD_END_SINGLE_PARAMETER_SEQ = { ESC, (byte)'I', (byte)'A', (byte)'#' };
        protected static readonly byte[] CMD_EXIT_PROG_MODE_AND_DATA_STORAGE = { ESC, (byte)'I', (byte)'A', SP };

        protected static readonly byte[] H_CMD_EXIT_HOST_MODE = { ESC, (byte)'[', (byte)'A' };
        protected static readonly byte[] R_CMD_EXIT_HOST_MODE = { ESC, (byte)'X', CR, LF };
        // reader information
        protected static readonly byte[] H_CMD_GET_READER_MODEL = { ESC, (byte)'[', (byte)'E' };
        protected static readonly byte[] H_CMD_GET_SOFTWARE_VERSION = { ESC, (byte)'[', (byte)'F' };
        protected static readonly byte[] H_CMD_GET_READER_NAME = { ESC, (byte)'[', (byte)'G' };
        // button function
        protected static readonly byte[] CMD_BTN_FUNC_1 = { ESC, (byte)'0', ESC, (byte)'I', (byte)'C', (byte)'#' };
        protected static readonly byte[] CMD_BTN_FUNC_2 = { ESC, (byte)'1', ESC, (byte)'I', (byte)'C', (byte)'#' };
        protected static readonly byte[] CMD_BTN_FUNC_3 = { ESC, (byte)'2', ESC, (byte)'I', (byte)'C', (byte)'#' };
        protected static readonly byte[] CMD_BTN_FUNC_4 = { ESC, (byte)'3', ESC, (byte)'I', (byte)'C', (byte)'#' };
        // other function
        protected static readonly byte[] CMD_DISABLE_ALL_SYMBOLOGIES = { ESC, (byte)'4', ESC, (byte)'I', (byte)'C', (byte)'#' };
        protected static readonly byte[] CMD_RESTORE_DEFAULT = { ESC, (byte)'5', ESC, (byte)'I', (byte)'C', (byte)'#' };
        //parameter 
        protected static readonly byte[] CMD_ACQ_TRIGGER_ONESHOT_1READ = { ESC, (byte)'A', (byte)'B', (byte)'1' };

        protected static readonly byte[] CMD_OPER_MODE_1ONESHOT = { ESC, (byte)'A', (byte)'A', (byte)'0' };
        protected static readonly byte[] CMD_OPER_MODE_2CONTINUOUS = { ESC, (byte)'A', (byte)'A', (byte)'1' };
        protected static readonly byte[] CMD_OPER_MODE_3PHASEMODE = { ESC, (byte)'A', (byte)'A', (byte)'2' };

        //이미지 프로세싱 - 타임아웃 설정
        protected static readonly byte[] CMD_IMG_PROC_TIMEOUT_DISABLE = { ESC, (byte)'F', (byte)'F', (byte)'0' };
        protected static readonly byte[] CMD_IMG_PROC_TIMEOUT_500MS = { ESC, (byte)'F', (byte)'F', (byte)'5', (byte)'0', (byte)'0' };
        protected static readonly byte[] CMD_IMG_PROC_TIMEOUT_1000MS = { ESC, (byte)'F', (byte)'F', (byte)'1', (byte)'0', (byte)'0', (byte)'0' };

        protected static readonly byte[] CMD_LED_AUTOLEARN_TIMEOUT_1000MS = { ESC, (byte)'N', (byte)'O', (byte)'1' };
#endif
        //Visiset 패킷 캐치 코드

        //접속 시도
        protected static readonly byte[] CMD_VISISET_CONNECT = { 
        0x1B, 0x5B, 0x43, 0x02, 0x00, 0x04, 0x00, 0x04, 
        0x06, 0x06, 0x06, 0x06, 0x03, 0x00, 0x25, 0x1B, 
        0x5B, 0x41, 0x02, 0x00, 0x01, 0x00, 0x48, 0x00, 
        0x03, 0x00, 0x4E, 0x02, 0x00, 0x01, 0x00, 0x1E, 
        0x00, 0x03, 0x00, 0x24, };
        //접속 해제
        protected static readonly byte[] CMD_VISISET_DISCONNECT = { 
        0x02, 0x00, 0x02, 0xFE, 0x00, 0x0E, 0x00, 0x00, 
        0x03, 0x00, 0x15 };

        //코드 읽기
        protected static readonly byte[] CMD_VISISET_CAPTURE =  { 
        0x02, 0x00, 0x01, 0x00, 0x04, 0x42, 0x03, 0x00, 
        0x4C };
        protected static readonly byte[] CMD_VISISET_DECODING = { 
        0x02, 0x00, 0x01, 0x00, 0x04, 0x43, 0x03, 0x00, 
        0x4D };

        //코드 인식
        protected static readonly byte[] CMD_VISISET_SETUP_OPEN = { 
        0x02, 0x00, 0x01, 0x00, 0x04, 0x4D, 0x03, 0x00, 
        0x57, 0x02, 0x00, 0x01, 0x00, 0x32, 0x0A, 0x03,
        0x00, 0x42 };
        protected static readonly byte[] CMD_VISISET_SETUP_CAPTURE = { 
        0x02, 0x00, 0x01, 0x00, 0x3D, 0x01, 0x03, 0x00, 
        0x44, 0x02, 0x00, 0x00, 0x00, 0x3B, 0x03, 0x00, 
        0x40 };
        protected static readonly byte[] CMD_VISISET_SETUP_CODESETTING = { 
        0x02, 0x00, 0x02, 0xFE, 0x00, 0x44, 0x01, 0x00, 
        0x03, 0x00, 0x4C, 0x02, 0x00, 0x00, 0x00, 0x3B, 
        0x03, 0x00, 0x40 };
        protected static readonly byte[] CMD_VISISET_SETUP_CLOSE = { 
        0x02, 0x00, 0x01, 0x00, 0x04, 0x1B, 0x03, 0x00, 
        0x25 };
        protected readonly byte[] CMD_VISISET_SETUP_SAVE = {  
        0x02, 0x00, 0x02, 0xFE, 0x00, 0x44, 0x02, 0xFE,
        0x00, 0x03, 0x00, 0x4D };

        protected static readonly byte[] CMD_VISISET_LEARN_START = {  
        0x02, 0x00, 0x05, 0x00, 0x04, 0x59, 0x31, 0x2C, 
        0x34, 0x00, 0x03, 0x00, 0xF8 };
        protected static readonly byte[] CMD_VISISET_LEARN_CANCLE = { 
        0x02, 0x00, 0x01, 0x00, 0x04, 0x1B, 0x03, 0x00, 
        0x25 };
    }
}