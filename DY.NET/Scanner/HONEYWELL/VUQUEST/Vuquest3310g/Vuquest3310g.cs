using System;
using System.IO.Ports;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text;
using NLog;

namespace DY.NET.HONEYWELL.VUQUEST
{
    /// <summary>
    /// 허니웰 Vuquest3310g 바코드 리더기 통신 클래스
    /// 115200-N-8-1
    /// </summary>
    public partial class Vuquest3310g : IScannerSerialCommAsync, ITag, IPingPong, ITimeout
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private const byte SYN = 0x16;
        private const byte CR = 0x0D;
        private const byte ACK = 0x06;
        private const byte ENQ = 0x05;
        private const byte NAK = 0x15;
        private const byte DOT = (byte)'.';

        private static readonly byte[] SPC_TRIGGER_ACTIVATE = { SYN, (byte)'T', CR };
        private static readonly byte[] SPC_TRIGGER_DEACTIVATE = { SYN, (byte)'U', CR };
        private static readonly byte[] PREFIX = { SYN, (byte)'M', CR };

        public int Tag { get; set; }
        public string Description { get; set; }
        public object UserData { get; set; }
        public EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged { get; set; }

        /// <summary>
        /// read time out ( 0 - 300000ms) TRGSTO#### 으로 전송해야함
        /// </summary>
        private static readonly byte[] SPC_TRIGGER_READ_TIMEOUT_N = {
        (byte)'T', (byte)'R', (byte)'G', (byte)'S', (byte)'T', (byte)'O'};

        private static readonly byte[] MNC_SAVE_CUSTOM_DEFAULTS = {
        (byte)'M', (byte)'N', (byte)'U', (byte)'C', (byte)'D', (byte)'S'};

        private static readonly byte[] PSS_ADD_CR_SUFIX_ALL_SYMBOL = {
        (byte)'V', (byte)'S', (byte)'U', (byte)'F', (byte)'C', (byte)'R'};

        private static readonly byte[] UTI_SHOW_SOFTWARE_REVERSION = {
        (byte)'R', (byte)'E', (byte)'V', (byte)'I', (byte)'N', (byte)'F'};

        private static readonly byte[] SPC_TRIGGER_READ_TIMEOUT_300000MS = {
        (byte)'T', (byte)'R', (byte)'G', (byte)'S', (byte)'T', (byte)'O',
        (byte)'3', (byte)'0', (byte)'0', (byte)'0', (byte)'0', (byte)'0'};

        public const int VUQUEST3310G_TIMEOUT_MAX = 300000;

        private volatile byte[] m_Buffer = new byte[4096];
        private volatile int m_BufferIdx;
        private volatile bool m_IsActivate = false;
#if false
        private volatile System.Timers.Timer m_TimeoutTimer;
#endif
        private volatile SerialPort m_SerialPort;

        /// <summary>
        /// 리더기가 바코드를 스캔하여 값을 읽어들일 때 발생
        /// </summary>
        public EventHandler<Vuquest3310gScanEventArgs> Scanned;

        private int m_WriteTimeout = 500;
        private int m_ReadTimeout = 500;

        public int WriteTimeout
        {
            get
            {
                return m_WriteTimeout;
            }
            set
            {
                m_WriteTimeout = value;
                if (m_SerialPort != null)
                    m_SerialPort.BaseStream.WriteTimeout = value;
            }
        }

        public int ReadTimeout
        {
            get
            {
                return m_ReadTimeout;
            }
            set
            {
                m_ReadTimeout = value;
                if (m_SerialPort != null)
                    m_SerialPort.BaseStream.ReadTimeout = value;
            }
        }

        public bool IsEnableSerial
        {
            get
            {
                if (m_SerialPort == null)
                    return false;
                return m_SerialPort.IsOpen;
            }
        }

#if false
        /// <summary>
        /// 타임아웃 시간(ms)
        /// </summary>
        public int TimeOut { get; set; }
#endif

        public Vuquest3310g(SerialPort serialPort)
        {
            m_SerialPort = serialPort;
            m_SerialPort.ErrorReceived += OnSerialErrorReceived;
            m_SerialPort.PinChanged += OnSerialPinChanged;
            m_SerialPort.DataReceived += OnCodeReceived;
#if false
            TimeOut = 1000; //1초

            m_TimeoutTimer = new System.Timers.Timer();
            m_TimeoutTimer.Elapsed += OnElapsedTimer;
#endif
        }

        ~Vuquest3310g()
        {
            Dispose();
        }

        private void OnSerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Close();
        }

        private void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Close();
        }
        private void OnElapsedTimer(object sender, ElapsedEventArgs args)
        {
            var timer = sender as System.Timers.Timer;
            timer.Stop();
            if (m_IsActivate)
            {
                DeactivateScan();
                m_IsActivate = false;
                m_BufferIdx = 0;
                if (Scanned != null)
                    Scanned(this, new Vuquest3310gScanEventArgs(null));
            }
        }

        private void OnCodeReceived(object sender, SerialDataReceivedEventArgs args)
        {
            if (!m_IsActivate)
                return;
#if false
            if (m_TimeoutTimer.Enabled)
                m_TimeoutTimer.Stop();
#endif
            SerialPort sp = sender as SerialPort;
            m_BufferIdx += sp.Read(m_Buffer, m_BufferIdx, m_Buffer.Length);
            if (m_Buffer[m_BufferIdx - 1] != CR)
                return;
            var bytes = new byte[m_BufferIdx - 1]; //for remove CR byte
            Buffer.BlockCopy(m_Buffer, 0, bytes, 0, bytes.Length);
            m_IsActivate = false;
            m_BufferIdx = 0;
            if (Scanned != null)
                Scanned(this, new Vuquest3310gScanEventArgs(bytes));
        }
        public bool IsConnected()
        {
            return IsEnableSerial;
        }

        private void ActivateScan()
        {
            if (!IsEnableSerial)
                return;
            m_IsActivate = true;
            m_SerialPort.Write(SPC_TRIGGER_ACTIVATE, 0, SPC_TRIGGER_ACTIVATE.Length);
        }

        public void DeactivateScan()
        {
            if (!IsEnableSerial)
                return;
            m_IsActivate = false;
            m_SerialPort.Write(SPC_TRIGGER_DEACTIVATE, 0, SPC_TRIGGER_DEACTIVATE.Length);
        }

        public async Task<long> PingAsync()
        {
            Stopwatch watch = Stopwatch.StartNew();
            Stream bs = m_SerialPort.BaseStream;
            List<byte> syn = new List<byte>();
            int size;
            syn.AddRange(PREFIX);
            syn.AddRange(PSS_ADD_CR_SUFIX_ALL_SYMBOL);
            syn.Add(DOT);
            byte[] reqt_code = syn.ToArray();
            await bs.WriteAsync(reqt_code, 0, reqt_code.Length);
            Task read_task = bs.ReadAsync(m_Buffer, 0, m_Buffer.Length);
            Task when_task = await Task.WhenAny(read_task, Task.Delay(m_ReadTimeout));
            if (when_task == read_task)
            {
                size = await (Task<int>)read_task;
            }
            else
            {
                size = -2;
                LOG.Trace(m_SerialPort.PortName + " PingPong (Read)Timeout: " + m_ReadTimeout + "ms");
            }

            if (size > 0)
            {
                size = await m_SerialPort.BaseStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
                LOG.Trace(m_SerialPort.PortName + " Software reversion: " + Encoding.ASCII.GetString(m_Buffer, 0, size));
            }
            watch.Stop();

            return size >= 0 ? watch.ElapsedMilliseconds : size;
        }

        private async Task<byte[]> ActivateAsync()
        {
            if (m_IsActivate)
                return null;
            m_IsActivate = true;
            Stream bs = m_SerialPort.BaseStream;
            await bs.WriteAsync(SPC_TRIGGER_ACTIVATE, 0, SPC_TRIGGER_ACTIVATE.Length);
            m_BufferIdx = 0;
            do
            {
                m_BufferIdx += await bs.ReadAsync(m_Buffer, m_BufferIdx, m_Buffer.Length - m_BufferIdx);
            } while (m_Buffer.Last() != CR);
            byte[] ret = new byte[m_BufferIdx - 1];
            Array.Copy(m_Buffer, 0, ret, 0, ret.Length);

            await bs.WriteAsync(SPC_TRIGGER_DEACTIVATE, 0, SPC_TRIGGER_DEACTIVATE.Length);
            m_IsActivate = false;
            return ret;
        }

        public async Task DeactivateAsync()
        {
            if (!IsEnableSerial)
                return;
            await m_SerialPort.BaseStream.WriteAsync(SPC_TRIGGER_DEACTIVATE, 0, SPC_TRIGGER_DEACTIVATE.Length);
            m_IsActivate = false;
        }

        /// <summary>
        /// 스캔에 필요한 리더기 파라미터를 설정한다. 
        /// 스캔에 앞서 먼저 한번 호출해야한다.
        /// </summary>
        /// <returns></returns>
        public async Task PrepareAsync()
        {
            if (!IsEnableSerial)
                return;
            List<byte> syn = new List<byte>();
            List<byte> rep = new List<byte>();
            byte[] reply;

            Dictionary<byte[], string> CMDDIC = new Dictionary<byte[], string>();
            CMDDIC.Add(PSS_ADD_CR_SUFIX_ALL_SYMBOL, "CR sufix setting error");
            CMDDIC.Add(SPC_TRIGGER_READ_TIMEOUT_300000MS, "Timeout setting error");

            foreach (var CMD in CMDDIC)
            {
                syn.Clear();
                syn.AddRange(PREFIX);
                syn.AddRange(CMD.Key);
                syn.Add(DOT);

                rep.Clear();
                rep.AddRange(CMD.Key);
                rep.Add(ACK);
                rep.Add(DOT);

                await m_SerialPort.BaseStream.WriteAsync(syn.ToArray(), 0, syn.Count);
                int size = await m_SerialPort.BaseStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
                reply = new byte[size];
                Array.Copy(m_Buffer, reply, size);

                if (!rep.SequenceEqual(reply))
                    throw new Exception(CMD.Value);
            }
        }

        public void Scan()
        {
            if (m_IsActivate)
                return;
            ActivateScan();
        }

#if false
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
            LOG.Trace("Vuquest3310g 타이머 스캔 시도, 타임아웃: " + timeout);
            if (!(0 <= timeout && timeout <= VUQUEST3310G_TIMEOUT_MAX))
                throw new ArgumentOutOfRangeException("timeout");
#if false
            TimeOut = timeout;
#endif
            if (m_IsActivate)
                return;
            ActivateScan();
#if false
            m_TimeoutTimer.Interval = TimeOut;
            m_TimeoutTimer.Start();
#endif
        }
#endif

        public async Task<object> ScanAsync()
        {
            byte[] ret = null;
            ret = await ActivateAsync();
            await DeactivateAsync();
            return ret;
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
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs(m_SerialPort.IsOpen));
            LOG.Debug("Vuquest3310g 시리얼포트 통신 접속");
            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// 리더기에 종료 신호를 보낸 뒤
        /// 시리얼통신을 종료
        /// </summary>
        public void Close()
        {
#if false
            if (m_TimeoutTimer.Enabled)
                m_TimeoutTimer.Stop();
#endif
            if (m_IsActivate)
                DeactivateScan();
            if (IsEnableSerial)
                m_SerialPort.Close();
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs(m_SerialPort.IsOpen));
            LOG.Debug("Vuquest3310g 시리얼포트 통신 해제");
        }

        /// <summary>
        /// 시리얼포트 객체의 자원을 소멸
        /// </summary>
        public void Dispose()
        {
            Close();
            m_SerialPort.Dispose();
#if false
            m_TimeoutTimer.Dispose();
#endif
            GC.SuppressFinalize(this);
            LOG.Debug("Vuquest3310g 접속종료 및 메모리 해제");
        }
    }
}
