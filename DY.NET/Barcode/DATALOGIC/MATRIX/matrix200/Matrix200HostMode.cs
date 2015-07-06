using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace DY.NET.DATALOGIC.MATRIX
{
    //Prog in
    //Oneshot -> Phase Mode
    //Host in
    //Phase On 
    //Trigger Read

    //1초뒤
    //Phase Off
    //Trigger Trailing

    /// <summary>
    /// 매트릭스200 바코드 리더기와 통신하는 객체
    /// 기능 설명 
    /// 1. 연결 시도 
    /// 연결 시도, 연결 성공과 이벤트 콜백
    /// 2. 현재 리더기 정보 알기
    /// 펌웨어 버전, 설정 툴, 디바이스 모델명, 시리얼넘버
    /// 3. 코드 읽기 시도 <-- 
    /// 
    /// 4. 코드 읽은 후 정보 얻기 
    /// 심볼: EAN-13 .. 등 
    /// 데이터 코드: 880 10064 2142 3 ..
    /// 픽셀 펄 엘리먼트 
    /// 반사율 최대값, 최소값
    /// 디코딩 타임 ms 
    /// 5. 연결 해제 
    /// </summary>
    public class Matrix200HostMode : Matrix200HostModeCommand
    {
        private SerialPort m_SerialPort;
        public SerialPort Serial
        {
            get
            {
                return m_SerialPort;
            }
        }

        public Matrix200HostModeState State { get; private set; }
        public Matrix200HostModeError Error { get; private set; }
        private EventHandler<Matrix200EventArgs> m_Callback;

        private byte[] m_Buf;
        private int m_BufIdx;

        private Matrix200HostMode()
        {
            Error = Matrix200HostModeError.OK;
            State = Matrix200HostModeState.DISCONNECT;
            m_Buf = new byte[1024];
            m_BufIdx = 0;
        }

        /// <summary>
        /// 정적 팩토리 생성 메서드
        /// </summary>
        /// <param name="com"></param>
        /// <param name="baudrate"></param>
        /// <returns></returns>
        public static Matrix200HostMode CreateMaxtrix200HostMode(string com, int baudrate)
        {
            var instance = new Matrix200HostMode();
            instance.m_SerialPort = new SerialPort(com, baudrate);
            return instance;
        }

        /// <summary>
        /// 시리얼통신 연결을 시작합니다.
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (!m_SerialPort.IsOpen)
            {
                m_SerialPort.Open();
                m_SerialPort.DataReceived += OnDataReceived;
            }
            return m_SerialPort.IsOpen;
        }

        private void OnDataReceived(object serial, SerialDataReceivedEventArgs args)
        {
            byte[] recv_data = System.Text.Encoding.ASCII.GetBytes(m_SerialPort.ReadExisting());
            Buffer.BlockCopy(recv_data, 0, m_Buf, m_BufIdx, recv_data.Length);
            m_BufIdx += recv_data.Length;

            recv_data = new byte[m_BufIdx];
            Buffer.BlockCopy(m_Buf, 0, recv_data, 0, recv_data.Length);
            if (recv_data.Last() != LF)
                return;
            m_BufIdx = 0;

            List<byte> reply_list = new List<byte>();
            foreach (var b in reply_list)
            {
                reply_list.Add(b);
                if(b == LF)
                {
#if DEBUG
                    Console.Write("<<< ");
                    foreach (var r in reply_list)
                        Console.Write(r + " ");
                    Console.WriteLine("");
                    Console.WriteLine("=============================================");
#endif
                    switch (State)
                    {
                        case Matrix200HostModeState.DISCONNECT:
                            break;
                        case Matrix200HostModeState.NOW_PROG_MODE:
                            break;
                        case Matrix200HostModeState.TRY_HOST_MODE_ENTER:
                            /* 호스트 모드 진입 시도 */
                            if (R_CMD_ENTER_HOST_MODE.SequenceEqual(reply_list))
                            {
                                State = Matrix200HostModeState.NOW_HOST_MODE;
                                Error = Matrix200HostModeError.OK;
                            }
                            else
                            {
                                State = Matrix200HostModeState.DISCONNECT;
                                Error = Matrix200HostModeError.HOST_MODE_ENTER_FAILURE_ERROR;
                                return;
                            }
                            break;
                        case Matrix200HostModeState.TRY_PROG_MODE_ENTER:
                            /* 호스트 프로그래밍 모드 진입 시도 */
                            if (R_CMD_ENTER_PROGRAMMING_MODE.SequenceEqual(reply_list))
                            {
                                State = Matrix200HostModeState.NOW_PROG_MODE;
                                Error = Matrix200HostModeError.OK;
                            }
                            else
                            {
                                State = Matrix200HostModeState.NOW_HOST_MODE;
                                Error = Matrix200HostModeError.PROGRAMMING_MODE_ENTER_FAILURE_ERROR;
                            }
                            break;
                        case Matrix200HostModeState.TRY_PROG_MODE_EXIT:
                            /* 호스트 프로그래밍 모드 종료 및 데이터 영구 저장 시도 */
                            if (R_CMD_OK.SequenceEqual(reply_list))
                            {
                                State = Matrix200HostModeState.NOW_HOST_MODE;
                                Error = Matrix200HostModeError.OK;
                            }
                            else if (R_CMD_WRONG.SequenceEqual(reply_list))
                            {
                                State = Matrix200HostModeState.NOW_HOST_MODE;
                                Error = Matrix200HostModeError.PROGRAMMING_MODE_EXIT_FAILURE_ERROR;
                            }
                            break;
                        case Matrix200HostModeState.TRY_HOST_MODE_EXIT:
                            if (R_CMD_EXIT_HOST_MODE.SequenceEqual(reply_list))
                            {
                                State = Matrix200HostModeState.DISCONNECT;
                                Error = Matrix200HostModeError.OK;
                            }
                            else
                            {
                                State = Matrix200HostModeState.NOW_HOST_MODE;
                                Error = Matrix200HostModeError.HOST_MODE_EXIT_FAILURE_ERROR;
                            }
                            /* 호스프 프로그래밍 종료 시도 */
                            break;
                        case Matrix200HostModeState.TRY_READING:
                            /* 바코드 읽기 시도 */

                            break;
                    }
                    reply_list.Clear();
                }
            }
            Call();
        }

        private void Call()
        {
            Call("");
        }

        private void Call(string data)
        {
            if (m_Callback != null)
                m_Callback(this, new Matrix200EventArgs()
                {
                    Error = this.Error,
                    State = this.State,
                    Data = data
                });
        }

        public void ReadBarcode(EventHandler<Matrix200EventArgs> callback)
        {
            if (!m_SerialPort.IsOpen)
                throw new Exception("The serial port is not open.");
            if (State == Matrix200HostModeState.DISCONNECT)
                throw new Exception("Now is not the programming mode.");
            m_Callback = callback;
            State = Matrix200HostModeState.TRY_READING;
            m_SerialPort.Write(CMD_BTN_FUNC_4, 0, CMD_BTN_FUNC_4.Length);
        }

        public void EnterHostMode(EventHandler<Matrix200EventArgs> callback)
        {
            if (!m_SerialPort.IsOpen)
                throw new Exception("The serial port is not open.");
            m_Callback = callback;
            State = Matrix200HostModeState.TRY_HOST_MODE_ENTER;
            m_SerialPort.Write(H_CMD_ENTER_HOST_MODE, 0, H_CMD_ENTER_HOST_MODE.Length);
        }

        public void EnterProgrammingMode(EventHandler<Matrix200EventArgs> callback)
        {
            if (!m_SerialPort.IsOpen)
                throw new Exception("The serial port is not open.");
            m_Callback = callback;
            State = Matrix200HostModeState.TRY_PROG_MODE_ENTER;
            m_SerialPort.Write(H_CMD_ENTER_PROGRAMMING_MODE, 0, H_CMD_ENTER_PROGRAMMING_MODE.Length);
        }

        public void ExitHostMode(EventHandler<Matrix200EventArgs> callback)
        {
            if (!m_SerialPort.IsOpen)
                throw new Exception("The serial port is not open.");
            m_Callback = callback;
            State = Matrix200HostModeState.TRY_HOST_MODE_EXIT;
            m_SerialPort.Write(H_CMD_EXIT_HOST_MODE, 0, H_CMD_EXIT_HOST_MODE.Length);
        }

        public void ExitProgrammingMode(EventHandler<Matrix200EventArgs> callback)
        {
            if (!m_SerialPort.IsOpen)
                throw new Exception("The serial port is not open.");
            m_Callback = callback;
            State = Matrix200HostModeState.TRY_PROG_MODE_EXIT;
            m_SerialPort.Write(CMD_EXIT_PROG_MODE_AND_DATA_STORAGE, 0, CMD_EXIT_PROG_MODE_AND_DATA_STORAGE.Length);
        }
    }
}
