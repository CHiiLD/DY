using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace DY.NET.Honeywell.Vuquest
{
    public class Vuquest3310g : SerialPort, IScannerStream
    {
        private int m_ReceiveTimeout;
        private int m_SendTimeout;
        private int m_ConnectTimeout;
        protected byte[] ReadBuffer;

        public int ReceiveTimeout
        {
            get
            {
                return m_ReceiveTimeout;
            }
            set
            {
                m_ReceiveTimeout = value >= 0 ? value : -1;
            }
        }

        public int SendTimeout
        {
            get
            {
                return m_SendTimeout;
            }
            set
            {
                m_SendTimeout = value >= 0 ? value : -1;
            }
        }

        public int OpenTimeout
        {
            get
            {
                return m_ConnectTimeout;
            }
            set
            {
                m_ConnectTimeout = value >= 0 ? value : -1;
            }
        }

        protected Vuquest3310g() 
        {
            ReadBuffer = new byte[base.ReadBufferSize];
            ReceiveTimeout = SendTimeout = OpenTimeout = -1;
        }

        public Vuquest3310g(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : this()
        {
            PortName = portName;
            BaudRate = baudRate;
            base.Parity = parity;
            DataBits = dataBits;
            base.StopBits = stopBits;
        }

        /// <summary>
        /// SerialPort의 BaseStream을 반환한다.
        /// </summary>
        public virtual Stream GetStream()
        {
            return base.BaseStream;
        }

        /// <summary>
        /// Vquest3310g와 비동기로 연결을 시도한다.
        /// </summary>
        public virtual async Task OpenAsync()
        {
            Task connect_task = Task.Run(() => { base.Open(); });
            if (await Task.WhenAny(connect_task, Task.Delay(OpenTimeout)) == connect_task)
                await connect_task;
            else
                throw new TimeoutException();
        }

        /// <summary>
        /// Vquest3310g와 비동기로 연결을 해제한다.
        /// </summary>
        /// <returns></returns>
        public new virtual void Close()
        {
            base.Close();
        }

        /// <summary>
        /// 포트와 물리적인 연결이 있었는지 질의한다.
        /// </summary>
        public virtual bool IsOpend()
        {
            return base.IsOpen;
        }

        public virtual async Task<IValue> ScanAsync()
        {
            return null;
        }
    }
}
