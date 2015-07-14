using System;
using System.IO.Ports;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace DY.NET.HONEYWELL.VUQUEST
{
    /// <summary>
    /// 허니웰 Vuquest3310g 바코드 리더기 통신 클래스
    /// 115200-N-8-1
    /// </summary>
    public partial class Vuquest3310g : IScannerSerialCommAsync, ITag
    {
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

        /// <summary>
        /// read time out ( 0 - 300000ms) TRGSTO#### 으로 전송해야함
        /// </summary>
        private static readonly byte[] SPC_TRIGGER_READ_TIMEOUT_N = {
        (byte)'T', (byte)'R', (byte)'G', (byte)'S', (byte)'T', (byte)'O'};

        private static readonly byte[] MNC_SAVE_CUSTOM_DEFAULTS = {
        (byte)'M', (byte)'N', (byte)'U', (byte)'C', (byte)'D', (byte)'S'};
        
        private static readonly byte[] PSS_ADD_CR_SUFIX_ALL_SYMBOL = {
        (byte)'V', (byte)'S', (byte)'U', (byte)'F', (byte)'C', (byte)'R'};

        private static readonly byte[] SPC_TRIGGER_READ_TIMEOUT_300000MS = {
        (byte)'T', (byte)'R', (byte)'G', (byte)'S', (byte)'T', (byte)'O',
        (byte)'3', (byte)'0', (byte)'0', (byte)'0', (byte)'0', (byte)'0'};

        public const int VUQUEST3310G_TIMEOUT_MAX = 300000;

        private volatile byte[] m_Buffer = new byte[4096];
        private volatile int m_BufferIdx;
        private volatile bool m_IsActivate = false;
        private volatile System.Timers.Timer m_TimeoutTimer;
        private volatile SerialPort m_SerialPort;

        /// <summary>
        /// 리더기가 바코드를 스캔하여 값을 읽어들일 때 발생
        /// </summary>
        public EventHandler<Vuquest3310gScanEventArgs> Scanned;
        
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
            m_TimeoutTimer = new System.Timers.Timer();
            m_TimeoutTimer.Elapsed += OnElapsedTimer;
        }

        ~Vuquest3310g()
        {
            Dispose();
        }

        public bool IsConnected()
        {
            return IsEnableSerial;
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
                Scanned(this, new Vuquest3310gScanEventArgs(bytes));
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
                m_BufferIdx += await bs.ReadAsync(m_Buffer, m_BufferIdx, m_Buffer.Length);
            } while (m_Buffer.Last() != CR);
            byte[] ret = new byte[m_BufferIdx - 1];
            Array.Copy(m_Buffer, 0, ret, 0, ret.Length);

            await bs.WriteAsync(SPC_TRIGGER_DEACTIVATE, 0, SPC_TRIGGER_DEACTIVATE.Length);
            m_IsActivate = false;
            return ret;
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

            foreach(var CMD in CMDDIC)
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
            if (!(0 <= timeout && timeout <= VUQUEST3310G_TIMEOUT_MAX))
                throw new ArgumentOutOfRangeException("timeout");
            TimeOut = timeout;
            if (m_IsActivate)
                return;
            ActivateScan();
            m_TimeoutTimer.Interval = TimeOut;
            m_TimeoutTimer.Start();
        }

        public async Task<object> ScanAsync()
        {
            return await ScanAsync(TimeOut);
        }

        public async Task<object> ScanAsync(int timeout)
        {
            if (!(0 <= timeout && timeout <= VUQUEST3310G_TIMEOUT_MAX))
                throw new ArgumentOutOfRangeException("timeout");
            TimeOut = timeout;
            byte[] ret = null;
            var task = ActivateAsync();
            if (await Task.WhenAny(task, Task.Delay(TimeOut)) == task)
                ret = await task;
            else
                DeactivateScan();
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
