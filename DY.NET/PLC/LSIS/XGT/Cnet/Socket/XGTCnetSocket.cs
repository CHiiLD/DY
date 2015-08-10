/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜을 사용하여 통신하는 소켓 구현 (PLC호스트 PC가 게스트일 때)
 * 주의: Cnet통신은 시리얼통신을 기반으로 합니다. 따라서 시리얼통신을 랩핑합시다.
 *       또한 시리얼통신은 동시에 여러 명령어를 보낼 수 없습니다. (1:1 요청 -> 응답 ->요청 ->응답의 루틴) 
 *       쓰레드에 안전하게 설계되어야 합니다.
 * 날짜: 2015-03-25
 */

using System;
using System.IO.Ports;
using NLog;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet 시리얼 클라이언트 통신 클래스
    /// </summary>
    public partial class XGTCnetSocket : ASocketCover
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private const string ERROR_SERIAL_IS_NULL = "SerialPort is null.";
        private volatile SerialPort m_SerialPort;
        public event SerialErrorReceivedEventHandler ErrorReceived;
        public event SerialPinChangedEventHandler PinChanged;

        /// <summary>
        /// PostAsync(WriteAsync), SerialPort.WriteTimeout
        /// 의 타임아웃 설정을 한다.
        /// </summary>
        public new int WriteTimeout
        {
            get
            {
                return m_SerialPort.WriteTimeout;
            }
            set
            {
                m_SerialPort.WriteTimeout = value;
            }
        }

        /// <summary>
        /// PostAsync(ReadAsync), SerialPort.ReadTimeout, Timer's Inteval
        /// 의 타임아웃 설정을 한다.
        /// </summary>
        public new int ReadTimeout
        {
            get
            {
                return m_SerialPort.ReadTimeout;
            }
            set
            {
                if (value > 0)
                    TimeoutTimer.Interval = value;
                m_SerialPort.ReadTimeout = value;
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        private XGTCnetSocket()
        {
        }

        /// <summary>
        /// 소멸자
        /// </summary>
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
            if (!IsConnected())
            {
                m_SerialPort.Open();
                SocketStream = m_SerialPort.BaseStream;
            }
            ConnectionStatusChangedEvent(IsConnected());
            LOG.Debug(Description + " 통신 접속 성공");
            return IsConnected();
        }

        /// <summary>
        /// 끊기
        /// </summary>
        /// <returns> 통신 끊기 성공 여부 </returns>
        public override void Close()
        {
            if (m_SerialPort != null)
                m_SerialPort.Close();
            ConnectionStatusChangedEvent(IsConnected());
            LOG.Debug(Description + " 통신 접속 해제");
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
        /// 메모리 반환
        /// </summary>
        public override void Dispose()
        {
            if (m_SerialPort != null)
            {
                Close();
                m_SerialPort.Dispose();

                m_SerialPort = null;
                ErrorReceived = null;
                PinChanged = null;
            }
            base.Dispose();
            LOG.Debug(Description + " 메모리 해제");
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 프로토콜 전송
        /// </summary>
        /// <param name="protocol"> XGTCnetProtocol 프로토콜 클래스, 반드시 사전에 AssembleProtocol() 함수 실행해야함</param>
        public override void Send(IProtocol protocol)
        {
            if (m_SerialPort == null)
                return;
            if (!IsConnected())
            {
                if (Connect())
                    throw new Exception("Serial port is not opend");
            }
            AProtocol reqt_p = protocol as AProtocol;
            if (reqt_p == null)
                throw new ArgumentNullException("Argument is not XGTCnetProtocol<T>.");
            byte[] reqt_bytes = reqt_p.ASCIIProtocol;
            if (!ReqPossible) //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(reqt_p);
                return;
            }
            ReqeustProtocolPointer = reqt_p;
            m_SerialPort.DataReceived += OnDataRecieve;
            m_SerialPort.Write(reqt_bytes, 0, reqt_bytes.Length); //데이터 전송
            if (ReadTimeout != -1)
                TimeoutTimer.Start(); //타임아웃 타이머 시작

            SendedProtocolSuccessfullyEvent(reqt_p); //소켓측 데이터 전송 이벤트
            reqt_p.ProtocolRequestedEvent(this, reqt_p); //프로토콜측 데이터 전송 이벤트
            ReqPossible = false;
        }

        /// <summary>
        /// 요청 프로토콜에 의한 응답 프로토콜 이벤트 메서드
        /// </summary>
        /// <param name="sender"> DYSerialPort 객체 </param>
        /// <param name="e"> SerialDataReceivedEventArgs 이벤트 argument </param>
        private void OnDataRecieve(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serial_port = sender as SerialPort;
            if (serial_port.BytesToRead <= 0)
                return; //버퍼가 비워져 있을 때 리턴

            BufIdx += serial_port.Read(Buf, BufIdx, Buf.Length - BufIdx);
            XGTCnetProtocol reqt = ReqeustProtocolPointer as XGTCnetProtocol;
            if (XGTCnetCCType.ETX != (XGTCnetCCType)(Buf[BufIdx - ((bool)reqt.IsExistBCC() ? 2 : 1)]))
                return; //데이터가 모두 전송되지 않음으로 리턴

            if (TimeoutTimer.Enabled)
                TimeoutTimer.Stop();
            else
                return; //TIMEOUT으로 리턴

            byte[] recv_data = new byte[BufIdx];
            Buffer.BlockCopy(Buf, 0, recv_data, 0, recv_data.Length);
            ReportResponseProtocol(reqt, recv_data);

            PrepareTransmission();
            SendNextProtocol();
        }

        /// <summary>
        /// 요청 프로토콜과 ASCII 데이터로 응답 프로토콜을 생성하고 결과를 이벤트로 전달
        /// </summary>
        /// <param name="reqt">요청 프로토콜 객체</param>
        /// <param name="recv_data">ASCII 데이터</param>
        /// <returns></returns>
        private XGTCnetProtocol ReportResponseProtocol(XGTCnetProtocol reqt, byte[] recv_data)
        {
            XGTCnetProtocol resp = reqt.MirrorProtocol as XGTCnetProtocol; //응답 객체 생성 전에 재활용이 가능한지 검토
            if (reqt.MirrorProtocol == null)
                reqt.MirrorProtocol = resp = XGTCnetProtocol.CreateResponseProtocol(recv_data, reqt);
            else
                resp.ASCIIProtocol = recv_data;
            resp.AnalysisProtocol();

            ReceivedProtocolSuccessfullyEvent(resp); //소켓측 응답 이벤트
            reqt.ProtocolReceivedEvent(this, resp);  //프로토콜측 응답 이벤트
            return resp;
        }

        /// <summary>
        /// 시리얼포트 하드웨어 에러 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            if (ErrorReceived != null)
                ErrorReceived(sender, e);
            Close();
            LOG.Error(Description + " 에러: " + e.EventType);
        }

        /// <summary>
        /// 시리얼포트 하드웨어 핀 변경 이벤트(접속이 끊긴 경우 같은)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            if (PinChanged != null)
                PinChanged(sender, e);
            Close();
            LOG.Debug(Description + " 핀 변경 이벤트: " + e.EventType);
        }
    }
}