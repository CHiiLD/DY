using System;
using System.IO.Ports;
using System.Timers;

namespace DY.NET.HONEYWELL.VUQUEST
{
    public partial class Vuquest3310g : IDisposable
    {
        private const byte SYN = 0x16;
        private const byte CR = 0x0D;
        private const byte ACK = 0x06;
        private const byte ENQ = 0x05;
        private const byte NAK = 0x15;

        private static readonly byte[] SPC_TRIGGER_ACTIVATE = { SYN, (byte)'T', CR };
        private static readonly byte[] SPC_TRIGGER_DEACTIVATE = { SYN, (byte)'U', CR };
        private static readonly byte[] PREFIX = { SYN, (byte)'M', CR };

        /// <summary>
        /// read time out ( 0 - 300000ms) TRGSTO#### 으로 전송해야함
        /// </summary>
        private static readonly byte[] SPC_TRIGGER_READ_TIMEOUT_N = {
        (byte)'T', (byte)'R', (byte)'G', (byte)'S', (byte)'T', (byte)'O' };
        private static readonly byte[] MNC_SAVE_CUSTOM_DEFAULTS = {
        (byte)'M', (byte)'N', (byte)'U', (byte)'C', (byte)'D', (byte)'S' };

        private volatile byte[] m_Buffer = new byte[4096];
        private volatile int m_BufferIdx;
        private volatile bool m_IsActivate = false;
        private volatile Timer m_TimeoutTimer;
        private volatile SerialPort m_SerialPort;

        /// <summary>
        /// 리더기가 바코드를 스캔하여 값을 읽어들일 때 발생
        /// </summary>
        public EventHandler<Vuquest3310gDataReceivedEventArgs> Scanned;
        
        public bool IsEnableSerial
        {
            get
            {
                if (m_SerialPort == null)
                    return false;
                return m_SerialPort.IsOpen;
            }
        }
        
        /// <summary>
        /// 타임아웃 시간(ms)
        /// </summary>
        public int TimeOut { get; set; }

        private Vuquest3310g()
        {
            TimeOut = 1000; //1초
            m_TimeoutTimer = new Timer();
            m_TimeoutTimer.Elapsed += OnElapsedTimer;
        }

        ~Vuquest3310g()
        {
            Dispose();
        }

        private void OnElapsedTimer(object sender, ElapsedEventArgs args)
        {
            var timer = sender as Timer;
            timer.Stop();
            if (m_IsActivate)
            {
                DeactivateScan();
                m_IsActivate = false;
                m_BufferIdx = 0;
                if (Scanned != null)
                    Scanned(this, new Vuquest3310gDataReceivedEventArgs(null));
            }
        }

        private void OnCodeReceived(object sender, SerialDataReceivedEventArgs args)
        {
            if (!m_IsActivate)
                return;
            if (m_TimeoutTimer.Enabled)
                m_TimeoutTimer.Stop();
            SerialPort sp = sender as SerialPort;
            m_BufferIdx += sp.Read(m_Buffer, m_BufferIdx, m_Buffer.Length);
            if (m_Buffer[m_BufferIdx - 1] != CR)
                return;
            var bytes = new byte[m_BufferIdx - 1]; //for remove CR byte
            Buffer.BlockCopy(m_Buffer, 0, bytes, 0, bytes.Length);
            m_IsActivate = false;
            m_BufferIdx = 0;
            if (Scanned != null)
                Scanned(this, new Vuquest3310gDataReceivedEventArgs(bytes));
        }

        private void ActivateScan()
        {
            if (!IsEnableSerial)
                return;
            m_IsActivate = true;
            m_SerialPort.Write(SPC_TRIGGER_ACTIVATE, 0, SPC_TRIGGER_ACTIVATE.Length);
        }

        private void DeactivateScan()
        {
            if (!IsEnableSerial)
                return;
            m_IsActivate = false;
            m_SerialPort.Write(SPC_TRIGGER_DEACTIVATE, 0, SPC_TRIGGER_DEACTIVATE.Length);
        }

        /// <summary>
        /// 리더기의 스캔을 시작
        /// </summary>
        public void Scan()
        {
            Scan(TimeOut);
        }

        /// <summary>
        /// 리더기의 스캔을 시작 
        /// </summary>
        /// <param name="timeout">타임아웃 시간을 지정</param>
        public void Scan(int timeout)
        {
            if (!(0 <= timeout && timeout <= 300000))
                throw new ArgumentOutOfRangeException("timeout");
            if (m_IsActivate)
                return;
            ActivateScan();
            m_TimeoutTimer.Interval = timeout;
            m_TimeoutTimer.Start();
        }

        /// <summary>
        /// 시리얼 포트에 접속
        /// </summary>
        /// <returns>접속 여부</returns>
        public bool Connect()
        {
            if (m_SerialPort == null)
                return false;
            if (!m_SerialPort.IsOpen)
                m_SerialPort.Open();
            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// 리더기에 종료 신호를 보낸 뒤
        /// 시리얼통신을 종료
        /// </summary>
        public void Close()
        {
            if (m_TimeoutTimer.Enabled)
                m_TimeoutTimer.Stop();
            if (m_IsActivate)
                DeactivateScan();
            if (IsEnableSerial)
                m_SerialPort.Close();
        }

        /// <summary>
        /// 시리얼포트 객체의 자원을 소멸
        /// </summary>
        public void Dispose()
        {
            m_SerialPort.Dispose();
            m_TimeoutTimer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
