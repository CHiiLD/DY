using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

namespace DY.NET.Honeywell.Vuquest
{
    public class Vuquest3310g : SerialPort, IScannerStream
    {
        public const int SCAN_MAX_TIMEOUT = 3000 * 10;

        private const byte SYN = 0x16;
        private const byte CR = 0x0D;
        private const byte ACK = 0x06;
        private const byte DOT = (byte)'.';

        private static readonly byte[] PREFIX = { 
        SYN, (byte)'M', CR };
        private static readonly byte[] TRIGGER_ACTIVATE = { 
        SYN, (byte)'T', CR };
        private static readonly byte[] TRIGGER_DEACTIVATE = { 
        SYN, (byte)'U', CR };
        private static readonly byte[] READ_TIMEOUT_N = { 
        (byte)'T', (byte)'R', (byte)'G', (byte)'S', (byte)'T', (byte)'O' };
        private static readonly byte[] SOFTWARE_REVERSION = {
        (byte)'R', (byte)'E', (byte)'V', (byte)'I', (byte)'N', (byte)'F'};
        private static readonly byte[] ADD_CR_SUFFIX = {
        (byte)'V', (byte)'S', (byte)'U', (byte)'F', (byte)'C', (byte)'R'};

        private int m_ReceiveTimeout;
        private int m_SendTimeout;
        private int m_ConnectTimeout;
        private bool m_NeedActivateTimeSet;

        protected byte[] ReadBuffer;

        public int InputTimeout
        {
            get
            {
                return m_ReceiveTimeout;
            }
            set
            {
                if (value != m_ReceiveTimeout && 0 <= value && value <= SCAN_MAX_TIMEOUT)
                {
                    m_ReceiveTimeout = value;
                    m_NeedActivateTimeSet = true;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public int OutputTimeout
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
            OutputTimeout = OpenTimeout = -1;
            InputTimeout = 500;
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
            Task openTask = Task.Run(() => { base.Open(); });
            if (await Task.WhenAny(openTask, Task.Delay(OpenTimeout)) == openTask)
            {
                await openTask;
                await PrepareAsync();
            }
            else
            {
                Close();
                throw new TimeoutException();
            }
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

        public async Task WriteAsync(byte[] buffer)
        {
            base.DiscardOutBuffer();
            Task writeTask = GetStream().WriteAsync(buffer, 0, buffer.Length);
            if(await Task.WhenAny(writeTask, Task.Delay(OutputTimeout)) == writeTask)
            {
                await writeTask;
            }
            else
            {
                Close();
                await OpenAsync();
                throw new WriteTimeoutException();
            }
        }

        public async Task<int> ReadAsync(byte ETX)
        {
            base.DiscardInBuffer();
            int idx = 0;
            Array.Clear(this.ReadBuffer, 0, this.ReadBuffer.Length);
            Stream stream = GetStream();
            do
            {
                Task<int> readTask = stream.ReadAsync(this.ReadBuffer, idx, this.ReadBuffer.Length - idx);
                if (await Task.WhenAny(readTask, Task.Delay(InputTimeout)) == readTask)
                {
                    idx += await readTask;
                }
                else
                {
                    Close();
                    await OpenAsync();
                    throw new ReadTimeoutException();
                }
            } while (ReadBuffer[idx - 1] != ETX);
            return idx;
        }

        public virtual async Task<IValue> ScanAsync()
        {
            await CheckActivateTime();
            await WriteAsync(TRIGGER_ACTIVATE);
            int size = await ReadAsync(CR);
            byte[] code = new byte[size - 1]; //CR은 제거
            Buffer.BlockCopy(ReadBuffer, 0, code, 0, code.Length);
            return new CodeValue(code);
        }
        private byte[] GetRequestCommandCode(byte[] cmd)
        {
            List<byte> code = new List<byte>();
            code.AddRange(PREFIX);
            code.AddRange(cmd);
            code.Add(DOT);
            return code.ToArray();
        }

        /// <summary>
        /// 스캐너로부터 받을(받을 예정인) 응답 명령어 코드를 반환한다.
        /// </summary>
        /// <param name="cmd">명령어</param>
        /// <returns>응답 명령어 코드</returns>
        private byte[] GetResponseCommandCode(byte[] cmd)
        {
            List<byte> code = new List<byte>();
            code.AddRange(cmd);
            code.Add(ACK);
            code.Add(DOT);
            return code.ToArray();
        }

        private async Task CheckActivateTime()
        {
            if (m_NeedActivateTimeSet)
            {
                await SetActivateTimeout(InputTimeout);
                m_NeedActivateTimeSet = false;
            }
        }

        private async Task SetActivateTimeout(int time)
        {
            List<byte> message = READ_TIMEOUT_N.ToList();
            foreach (char i in time.ToString())
                message.Add((byte)i);
            await SendCommendMessageAsync(message.ToArray());
        }

        public async Task<byte[]> SendCommendMessageAsync(byte[] message)
        {
            byte[] reqtCode = GetRequestCommandCode(message);
            byte[] expectedRespCode = GetResponseCommandCode(message);
            await WriteAsync(reqtCode);
            int size = await ReadAsync(expectedRespCode.Last());

            byte[] resultRespCode = new byte[expectedRespCode.Length];
            Buffer.BlockCopy(ReadBuffer, size - resultRespCode.Length, resultRespCode, 0, resultRespCode.Length);
            if (!resultRespCode.SequenceEqual(expectedRespCode))
                throw new Exception("Expected " + Encoding.ASCII.GetString(expectedRespCode));

            byte[] recvCode = new byte[size];
            Buffer.BlockCopy(ReadBuffer, 0, recvCode, 0, recvCode.Length);
            return recvCode;
        }
        public async Task<string> GetProductSerialNumber()
        {
            /*버그: 명령어를 2번 호출할 때 올바른 응답 코드를 수신할 수 있다.*/
            byte[] recvData = await SendCommendMessageAsync(SOFTWARE_REVERSION);
            if(recvData.SequenceEqual(GetResponseCommandCode(SOFTWARE_REVERSION)))
                recvData = await SendCommendMessageAsync(SOFTWARE_REVERSION);
            string input = Encoding.ASCII.GetString(recvData);
            Console.WriteLine(input);
            return new Regex("Serial Number: ([A-Za-z0-9/]+)").Match(input).Groups[1].Value;
        }

        public virtual async Task PrepareAsync()
        {
            await SendCommendMessageAsync(ADD_CR_SUFFIX);
        }
    }
}