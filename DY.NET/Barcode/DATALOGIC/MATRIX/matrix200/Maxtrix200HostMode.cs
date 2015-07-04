using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace DY.NET.DATALOGIC.MATRIX
{
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
    public class Maxtrix200HostMode : Maxtrix200HostModeCommand
    {
        private SerialPort m_SerialPort;
        private byte[] m_Buffer;
        private int m_BufferIdx;

        public Matrix200HostModeState State { get; private set; }
        public bool IsProgrammingMode { get; private set; }
        private bool IsHostMode { get; set; }
        private EventHandler<EventArgs> m_EnterCallback, m_ExitCallback;

        private Maxtrix200HostMode()
        {
            State = Matrix200HostModeState.OK;
            m_Buffer = new byte[1024];
            IsProgrammingMode = false;
            IsHostMode = false;
        }

        /// <summary>
        /// 정적 팩토리 생성 메서드
        /// </summary>
        /// <param name="com"></param>
        /// <param name="baudrate"></param>
        /// <returns></returns>
        public static Maxtrix200HostMode CreateMaxtrix200HostMode(string com, int baudrate)
        {
            var instance = new Maxtrix200HostMode();
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
                m_SerialPort.Open();
            return m_SerialPort.IsOpen;
        }

        private void OnDataReceived(object serial, SerialDataReceivedEventArgs args)
        {

        }

        /// <summary>
        /// 시리얼포트 객체의 버퍼를 m_Buffer 객체로 데이터를 복사합니다.
        /// 데이터를 전부 받은 경우 true 아니면 false를 반환합니다
        /// </summary>
        /// <returns>데이터를 전부 받은 경우 true 아니면 false</returns>
        private byte[] CopyToBuffer()
        {
            byte[] ret = null;
            do
            {
                byte[] buf_data = System.Text.Encoding.ASCII.GetBytes(m_SerialPort.ReadExisting());
                if (buf_data.Length == 0)
                    break;
                Buffer.BlockCopy(buf_data, 0, m_Buffer, m_BufferIdx, buf_data.Length);
                m_BufferIdx += buf_data.Length;
                if (buf_data[m_BufferIdx - 2] == CR && buf_data[m_BufferIdx - 1] == LF)
                {
                    ret = new byte[m_BufferIdx];
                    Buffer.BlockCopy(m_Buffer, 0, ret, 0, ret.Length);
                    m_BufferIdx = 0;
                }
            } while (false);
            return ret;
        }

        private void OnDataReceived_EnterHostMode(object serial, SerialDataReceivedEventArgs args)
        {
            var bytes = CopyToBuffer();
            if (bytes == null)
                return;

            if (!bytes.SequenceEqual(R_CMD_ENTER_HOST_MODE))
            {
                State = Matrix200HostModeState.HOST_MODE_ENTER_FAILURE_ERROR;
                m_SerialPort.DataReceived -= OnDataReceived_EnterHostMode;
                if (m_EnterCallback != null)
                    m_EnterCallback(this, EventArgs.Empty);
                return;
            }
            IsHostMode = true;
            m_SerialPort.DataReceived -= OnDataReceived_EnterHostMode;
            m_SerialPort.DataReceived += OnDataReceived_EnterProgrammingMode;
            m_SerialPort.Write(H_CMD_ENTER_PROGRAMMING_MODE, 0, H_CMD_ENTER_PROGRAMMING_MODE.Length);
        }

        private void OnDataReceived_EnterProgrammingMode(object serial, SerialDataReceivedEventArgs args)
        {
            var bytes = CopyToBuffer();
            if (bytes == null)
                return;
            if (!bytes.SequenceEqual(R_CMD_ENTER_PROGRAMMING_MODE))
            {
                State = Matrix200HostModeState.PROGRAMMING_MODE_ENTER_FAILURE_ERROR;
            }
            else
            {
                m_SerialPort.DataReceived += OnDataReceived;
                IsProgrammingMode = true;
                State = Matrix200HostModeState.OK;
            }
            m_SerialPort.DataReceived -= OnDataReceived_EnterProgrammingMode;
            if (m_EnterCallback != null)
                m_EnterCallback(this, EventArgs.Empty);
        }

        /// <summary>
        /// 리더기와 호스트모드 접속을 시도합니다.
        /// </summary>
        /// <param name="callback">작업 후, 결과를 알리는 콜백 핸들러</param>
        public void TryEnterHostProgorammingMode(EventHandler<EventArgs> callback)
        {
            if (!Connect())
            {
                State = Matrix200HostModeState.SERIALPORT_DISCONNECTION_ERROR;
                callback(this, EventArgs.Empty);
                return;
            }

            m_EnterCallback = callback;
            m_SerialPort.DataReceived += OnDataReceived_EnterHostMode;
            m_SerialPort.Write(H_CMD_ENTER_HOST_MODE, 0, H_CMD_ENTER_HOST_MODE.Length);
        }

        // device disconnection 
        private void OnDataReceived_ExitHostMode(object serial, SerialDataReceivedEventArgs args)
        {
            var bytes = CopyToBuffer();
            if (bytes == null)
                return;
            if (!bytes.SequenceEqual(R_CMD_EXIT_HOST_MODE))
                State = Matrix200HostModeState.HOST_MODE_EXIT_FAILURE_ERROR;
            else
            {
                IsHostMode = false;
                State = Matrix200HostModeState.OK;
            }
                
            m_SerialPort.DataReceived -= OnDataReceived_ExitHostMode;
            if (m_ExitCallback != null)
                m_ExitCallback(this, EventArgs.Empty);
        }

        private void OnDataReceived_ExitProgrammingMode(object serial, SerialDataReceivedEventArgs args)
        {
            var bytes = CopyToBuffer();
            if (bytes == null)
                return;
            if (!bytes.SequenceEqual(R_CMD_OK))
            {
                State = Matrix200HostModeState.PROGRAMMING_MODE_EXIT_FAILURE_ERROR;
                m_SerialPort.DataReceived -= OnDataReceived_ExitProgrammingMode;
                if (m_ExitCallback != null)
                    m_ExitCallback(this, EventArgs.Empty);
                return;
            }
            IsProgrammingMode = false;
            m_SerialPort.DataReceived -= OnDataReceived_ExitProgrammingMode;
            m_SerialPort.DataReceived += OnDataReceived_ExitHostMode;
            m_SerialPort.Write(H_CMD_EXIT_HOST_MODE, 0, H_CMD_EXIT_HOST_MODE.Length);
        }

        /// <summary>
        /// 리더기와 연결을 종료합니다.
        /// </summary>
        /// <param name="callback">작업 후, 결과를 알리는 콜백 핸들러</param>
        public void TryExitHostProgrammingMode(EventHandler<EventArgs> callback)
        {
            if (!Connect())
            {
                State = Matrix200HostModeState.SERIALPORT_DISCONNECTION_ERROR;
                callback(this, EventArgs.Empty);
                return;
            }
            m_ExitCallback = callback;
            m_SerialPort.DataReceived -= OnDataReceived;
            m_SerialPort.DataReceived += OnDataReceived_ExitProgrammingMode;
            m_SerialPort.Write(H_CMD_EXIT_PROG_MODE_AND_DATA_STORAGE, 0, H_CMD_EXIT_PROG_MODE_AND_DATA_STORAGE.Length);
        }
    }
}
