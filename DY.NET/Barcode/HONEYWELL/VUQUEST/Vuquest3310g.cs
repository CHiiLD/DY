using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

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

        private byte[] m_Buffer = new byte[4096];

        private SerialPort m_SerialPort;
        public bool IsEnableSerial
        {
            get
            {
                if (m_SerialPort == null)
                    return false;
                return m_SerialPort.IsOpen;
            }
        }

        public int TimeOut
        {
            get;
            set;
        }

        private Vuquest3310g() 
        {
            TimeOut = 1000; //1초
        }

        ~Vuquest3310g()
        {
            Dispose();
        }

        //타임 아웃 설정
        //액티베이트
        //대기
        //디엑티베이트

        //public async Task PrepareAsync()
        //{
        //    if (!IsEnableSerial)
        //        return;
        //    await SetTimeOutAsync(TimeOut);
        //}

        //public async Task SetTimeOutAsync(int ms)
        //{
        //    if (!(0 <= ms && ms <= 300000))
        //        return;
        //    TimeOut = ms;
        //    var cmd_list = PREFIX.ToList();
        //    cmd_list.AddRange(SPC_TRIGGER_READ_TIMEOUT_N);
        //    foreach (var i in ms.ToString())
        //        cmd_list.Add((byte)i);
        //    var bytes = cmd_list.ToArray();
        //    await m_SerialPort.BaseStream.WriteAsync(bytes, 0, bytes.Length);

        //    int size = await m_SerialPort.BaseStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
        //    Console.Write("");
        //    //return size == 1 && m_Buffer[0] == ACK;
        //}

        public void ActivateScan()
        {
            if (!IsEnableSerial)
                return;
            m_SerialPort.Write(SPC_TRIGGER_ACTIVATE, 0, SPC_TRIGGER_ACTIVATE.Length);
        }

        public void DeactivateScan()
        {
            if (!IsEnableSerial)
                return;
            m_SerialPort.Write(SPC_TRIGGER_DEACTIVATE, 0, SPC_TRIGGER_DEACTIVATE.Length);
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
            if (IsEnableSerial)
                m_SerialPort.Close();
        }

        /// <summary>
        /// 시리얼포트 객체의 자원을 소멸
        /// </summary>
        public void Dispose()
        {
            m_SerialPort.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
