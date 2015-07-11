/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜을 사용하여 통신하는 소켓 구현 (PLC호스트 PC가 게스트일 때)
 * 주의: Cnet통신은 시리얼통신을 기반으로 합니다. 따라서 시리얼통신을 랩핑합시다.
 *       또한 시리얼통신은 동시에 여러 명령어를 보낼 수 없습니다. (1:1 요청 -> 응답 ->요청 ->응답의 루틴) 
 *       쓰레드에 안전하게 설계되어야 합니다.
 * 날짜: 2015-03-25
 */

using System;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGTCnetExclusiveProtocol을 이용한 Serial 통신소켓 클래스
    /// </summary>
    public partial class XGTCnetSocket : ASocketCover
    {
        public class Builder : ASerialPortBuilder
        {
            public Builder(string name, int baud)
                : base(name, baud)
            {
            }

            public override object Build()
            {
                var skt = new XGTCnetSocket() { m_SerialPort = new SerialPort(_PortName, _BaudRate, _Parity, _DataBits, _StopBits) };
                skt.m_SerialPort.DataReceived += skt.OnDataRecieve;
                skt.m_SerialPort.ErrorReceived += skt.OnSerialErrorReceived;
                skt.m_SerialPort.PinChanged += skt.OnSerialPinChanged;
                return skt;
            }
        }

        #region VAR_PROPERTIES_EVENT
        private const string ERROR_SERIAL_IS_NULL = "m_SerialPort is null.";
        private volatile SerialPort m_SerialPort;
        public event SerialErrorReceivedEventHandler ErrorReceived;
        public event SerialPinChangedEventHandler PinChanged;
        #endregion

        #region METHOD
        private XGTCnetSocket()
        {
        }

        ~XGTCnetSocket()
        {
            Dispose();
        }

        /// <summary>
        /// 접속
        /// </summary>
        /// <returns> 통신 접속 성공 여부 </returns>
        public override bool Connect()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIAL_IS_NULL);
            if (!m_SerialPort.IsOpen)
                m_SerialPort.Open();
            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// 끊기
        /// </summary>
        /// <returns> 통신 끊기 성공 여부 </returns>
        public override void Close()
        {
            m_SerialPort.Close();
        }

        /// <summary>
        /// 연결 확인 
        /// </summary>
        /// <returns> 결과 </returns>
        public override bool IsConnected()
        {
            if (m_SerialPort == null)
                return false;
            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// 프로토콜 전송
        /// </summary>
        /// <param name="protocol"> XGTCnetProtocol 프로토콜 클래스, 반드시 사전에 AssembleProtocol() 함수 실행해야함</param>
        public override void Send(IProtocol protocol)
        {
            if (m_SerialPort == null)
                return;
            if (!m_SerialPort.IsOpen)
                throw new Exception("Serial port is not opend");
            AProtocol reqt_p = protocol as AProtocol;
            if (reqt_p == null)
                throw new ArgumentNullException("Argument is not XGTCnetProtocol<T>.");
            byte[] reqt_bytes = reqt_p.ASCIIProtocol;
            if (IsWait) //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(reqt_p);
                return;
            }
            SavePoint_ReqeustProtocol = reqt_p;
            m_SerialPort.Write(reqt_bytes, 0, reqt_bytes.Length);
            SendedProtocolSuccessfullyEvent(reqt_p);
            reqt_p.ProtocolRequestedEvent(this, reqt_p);
            IsWait = true;
        }

        private void ReportResponseProtocol(byte[] buf_temp)
        {
            Type type_T = SavePoint_ReqeustProtocol.GetType().GenericTypeArguments[0]; //<T>의 Type 얻기
            Type type_pt = typeof(XGTCnetProtocol<>).MakeGenericType(type_T); //XGTCnetProtocol<T> 타입 생성
            AXGTCnetProtocol reqt_p = SavePoint_ReqeustProtocol as AXGTCnetProtocol;
            AXGTCnetProtocol resp_p = reqt_p.MirrorProtocol as AXGTCnetProtocol; //응답 객체 생성 전에 재활용이 가능한지 검토
            if (reqt_p.MirrorProtocol == null)
            {
                resp_p = type_pt.GetMethod("CreateReceiveProtocol").Invoke(reqt_p, new object[] { buf_temp, reqt_p }) as AXGTCnetProtocol;
                reqt_p.MirrorProtocol = resp_p;
            }
            else
            {
                resp_p.ASCIIProtocol = buf_temp;
            }
#if !DEBUG
            try
            {
#endif
            resp_p.AnalysisProtocol();
#if !DEBUG
            }
            catch (Exception exception)
            {
                resp_p.Error = XGTCnetProtocolError.EXCEPTION;
            }
            finally
            {
#endif
            ReceivedProtocolSuccessfullyEvent(resp_p);
            if (resp_p.Error == XGTCnetProtocolError.OK)
                SavePoint_ReqeustProtocol.ProtocolReceivedEvent(this, resp_p);
            else
                SavePoint_ReqeustProtocol.ErrorReceivedEvent(this, resp_p);
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 요청 프로토콜에 의한 응답 프로토콜 이벤트 메서드
        /// </summary>
        /// <param name="sender"> DYSerialPort 객체 </param>
        /// <param name="e"> SerialDataReceivedEventArgs 이벤트 argument </param>
        private void OnDataRecieve(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = sender as SerialPort;
            do
            {
                if (sp == null)
                    break;
                if (!sp.IsOpen)
                    break;
                BufferIdx += sp.Read(Buffer_, BufferIdx, Buffer_.Length);
                AXGTCnetProtocol reqt_p = SavePoint_ReqeustProtocol as AXGTCnetProtocol;
                if (XGTCnetCCType.ETX != (XGTCnetCCType)(Buffer_[BufferIdx - ((bool)reqt_p.IsExistBCC() ? 2 : 1)]))
                    return;
                byte[] recv_data = new byte[BufferIdx];
                Buffer.BlockCopy(Buffer_, 0, recv_data, 0, recv_data.Length);
                ReportResponseProtocol(recv_data);
                SavePoint_ReqeustProtocol = null;
                BufferIdx = 0;
                IsWait = false;
                if (ProtocolStandByQueue.Count != 0)
                    SendNextProtocol();
            }
            while (false);
        }

        private void SendNextProtocol()
        {
            IProtocol temp = null;
            if (ProtocolStandByQueue.TryDequeue(out temp))
                Send(temp);
        }

        private void OnSerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            if (ErrorReceived != null)
                ErrorReceived(sender, e);
        }

        private void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            if (PinChanged != null)
                PinChanged(sender, e);
        }

        /// <summary>
        /// 메모리 반환
        /// </summary>
        public override void Dispose()
        {
            if (m_SerialPort != null)
            {
                ErrorReceived = null;
                PinChanged = null;
                SavePoint_ReqeustProtocol = null;
                if (IsConnected())
                    Close();
                m_SerialPort.Dispose();
            }
            base.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}