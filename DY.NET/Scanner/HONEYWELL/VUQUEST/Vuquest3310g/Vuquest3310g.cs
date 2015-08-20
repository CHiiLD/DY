﻿using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using NLog;
using Nito.AsyncEx;

namespace DY.NET.HONEYWELL.VUQUEST
{
    /// <summary>
    /// 허니웰 Vuquest3310g 바코드 리더기 통신 클래스
    /// 115200-N-8-1
    /// </summary>
    public partial class Vuquest3310g : IScannerSerialCommAsync
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private const byte SYN = 0x16;
        private const byte CR = 0x0D;
        private const byte ACK = 0x06;
        private const byte ENQ = 0x05;
        private const byte NAK = 0x15;
        private const byte DOT = (byte)'.';

        public const string ERROR_WRITE_TIMEOUT = "WriteAsync timeout exception";
        public const string ERROR_READ_TIMEOUT = "ReadAsync timeout exception";

        private static readonly byte[] SPC_TRIGGER_ACTIVATE = { SYN, (byte)'T', CR };
        private static readonly byte[] SPC_TRIGGER_DEACTIVATE = { SYN, (byte)'U', CR };
        private static readonly byte[] PREFIX = { SYN, (byte)'M', CR };

        public int Tag { get; set; }
        public string Description { get; set; }
        public object UserData { get; set; }
        public EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged { get; set; }
        
        private readonly AsyncLock m_AsyncLock = new AsyncLock();
        private bool m_IsPrepared = false;
        
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

        private byte[] m_Buffer = new byte[4096];
        private SerialPort m_SerialPort;

        public int WriteTimeoutMaximum { get; set; }
        public int WriteTimeoutMinimum { get; set; }

        public int ReadTimeoutMaximum { get; set; }
        public int ReadTimeoutMinimum { get; set; }

        public int WriteTimeout { get; set; }
        public int ReadTimeout { get; set; }

        public Vuquest3310g(SerialPort serialPort)
        {
            WriteTimeout = 100;
            ReadTimeout = 1000;

            WriteTimeoutMaximum = 500;
            ReadTimeoutMaximum = VUQUEST3310G_TIMEOUT_MAX;

            WriteTimeoutMinimum = 0;
            ReadTimeoutMinimum = 0;

            m_SerialPort = serialPort;
            Description = "Hoenywell Vuquest 3310g(" + m_SerialPort.PortName + ")";
        }

        ~Vuquest3310g()
        {
            Dispose();
        }


        public bool IsConnected()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException("SerialPort is null.");
            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// 시리얼포트 객체의 자원을 소멸
        /// </summary>
        public void Dispose()
        {
            Close();
            m_SerialPort.Dispose();
            m_SerialPort = null;
            GC.SuppressFinalize(this);
            LOG.Debug(Description + " 접속종료 및 메모리 해제");
        }

        /// <summary>
        /// 리더기에 종료 신호를 보낸 뒤
        /// 시리얼통신을 종료
        /// </summary>
        public void Close()
        {
            m_SerialPort.Close();
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs(false));
            LOG.Debug(Description + " 시리얼포트 통신 해제");
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
            LOG.Debug(Description + " 시리얼포트 통신 접속");
            return m_SerialPort.IsOpen;
        }
    }
}