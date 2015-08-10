using System;
using System.Net.Sockets;
using NLog;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet 이더넷 클라이언트 소켓 클래스
    /// </summary>
    public sealed partial class XGTFEnetSocket : ASocketCover
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private string m_Host;
        private int m_Port;
        private volatile TcpClient m_TcpClient;
        private IAsyncResult m_WriteAsyncResult;
        private IAsyncResult m_ReadAsyncResult;

        /// <summary>
        /// PostAsync(WriteAsync), TcpClient.GetStream().WriteTimeou
        /// 의 타임아웃 설정을 한다.
        /// </summary>
        public new int WriteTimeout
        {
            get
            {
                return m_TcpClient.GetStream().WriteTimeout;
            }
            set
            {
                m_TcpClient.GetStream().WriteTimeout = value;
            }
        }

        /// <summary>
        /// PostAsync(ReadAsync), TcpClient.GetStream().WriteTimeou, Timer's Inteval
        /// 의 타임아웃 설정을 한다.
        /// </summary>
        public new int ReadTimeout
        {
            get
            {
                return m_TcpClient.GetStream().ReadTimeout;
            }
            set
            {
                if (value > 0)
                    TimeoutTimer.Interval = value;
                m_TcpClient.GetStream().ReadTimeout = value;
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public XGTFEnetSocket(string host, XGTFEnetPort port)
        {
            m_Host = host;
            m_Port = (int)port;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public XGTFEnetSocket(string host, int port)
        {
            m_Host = host;
            m_Port = port;
            Description = "LSIS XGT FEnet(" + m_Host + ":" + m_Port + ")";
        }

        /// <summary>
        /// 소멸자
        /// </summary>
        ~XGTFEnetSocket()
        {
            Dispose();
        }

        /// <summary>
        /// 접속 시도
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            //비동기 요청
            if (!IsConnected())
            {
                m_TcpClient = new TcpClient(m_Host, m_Port);
                SocketStream = m_TcpClient.GetStream();
            }
            ConnectionStatusChangedEvent(IsConnected());
            LOG.Debug(Description + " 이더넷 통신 동기 접속");
            return IsConnected();
        }

        /// <summary>
        /// 접속 해제
        /// </summary>
        public override void Close()
        {
            EndAsync();
            m_TcpClient.Close();
            ConnectionStatusChangedEvent(IsConnected());
        }

        /// <summary>
        /// 접속 상태
        /// </summary>
        /// <returns></returns>
        public override bool IsConnected()
        {
            if (m_TcpClient == null)
                return false;
            if (m_TcpClient.Client == null)
                return false;
            return m_TcpClient.Connected;
        }

        /// <summary>
        /// 메모리 해제 및 반환
        /// </summary>
        public override void Dispose()
        {
            if (m_TcpClient != null)
            {
                EndAsync();

                if (m_TcpClient != null)
                    m_TcpClient.Close();

                ConnectionStatusChangedEvent(IsConnected());

                m_TcpClient = null;
                m_Host = null;
                m_Port = -1;
            }

            base.Dispose();
            GC.SuppressFinalize(this);
            LOG.Debug(Description + " 이더넷 통신 해제 및 메모리 해제");
        }

        /// <summary>
        ///  TcpClient 객체의 비동기 작업들을 종료한다.
        /// </summary>
        private void EndAsync()
        {
            if (m_ReadAsyncResult != null)
                SocketStream.EndRead(m_ReadAsyncResult);
            if (m_WriteAsyncResult != null)
                SocketStream.EndRead(m_WriteAsyncResult);

            m_WriteAsyncResult = null;
            m_ReadAsyncResult = null;
        }

        /// <summary>
        /// 헤더부분의 Length 부분과 실제 Instruct Size를 계산하여 일치하는지 계산 
        /// </summary>
        /// <returns>일치할 시(프로토콜을 모두 전송 받았을 때)true, 아니면 false</returns>
        private bool IsMatchInstructSize()
        {
            ushort target = 0, sum = 0;
            if (BufIdx < XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE)
                return false;
            target = CV2BR.ToValue(new byte[] { Buf[16], Buf[17] });
            for (int i = XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE; i < BufIdx; i++) //바이트의 개수
                sum++;
            return target == sum;
        }

        /// <summary>
        /// 요청 프로토콜과 ASCII 데이터로 응답 프로토콜을 생성하고 결과를 이벤트로 전달
        /// </summary>
        /// <param name="reqt">요청 프로토콜 객체</param>
        /// <param name="recv_data">ASCII 데이터</param>
        /// <returns></returns>
        private XGTFEnetProtocol ReportResponseProtocol(XGTFEnetProtocol reqt, byte[] recv_data)
        {
            XGTFEnetProtocol resp = reqt.MirrorProtocol as XGTFEnetProtocol; //응답 객체 생성 전에 재활용이 가능한지 검토
            if (reqt.MirrorProtocol == null)
                reqt.MirrorProtocol = resp = XGTFEnetProtocol.CreateResponseProtocol(recv_data, reqt);
            else
                resp.ASCIIProtocol = recv_data;
            resp.AnalysisProtocol();

            ReceivedProtocolSuccessfullyEvent(resp);
            reqt.ProtocolReceivedEvent(this, resp);
            return resp;
        }

        /// <summary>
        /// 프로토콜 전송
        /// </summary>
        /// <param name="protocol"></param>
        public override void Send(IProtocol protocol)
        {
            if (m_TcpClient == null)
                return;
            if (!IsConnected())
            {
                if (!Connect())
                {
                    LOG.Debug(Description + " 프로토콜을 보내기 위해 서버와 접속시도를 하였으나 실패");
                    return;
                }
            }

            AProtocol reqt = protocol as AProtocol;
            if (reqt == null)
                throw new ArgumentException("Protocol not match AProtocol type");
            byte[] reqt_bytes = reqt.ASCIIProtocol;
            if (!ReqPossible)
            {
                ProtocolStandByQueue.Enqueue(reqt);
                return;
            }
            ReqPossible = false;
            ReqeustProtocolPointer = reqt;

            m_WriteAsyncResult = SocketStream.BeginWrite(reqt_bytes, 0, reqt_bytes.Length, OnSended, null); //비동기 쓰기 시작
        }

        /// <summary>
        /// TcpClient WriteBuffer에 데이터를 다 올렸을 때 호출되는 콜백함수
        /// </summary>
        /// <param name="ar"></param>
        private void OnSended(IAsyncResult ar)
        {
            if (m_TcpClient == null || !IsConnected())
                return;

            if (m_ReadAsyncResult == null)
                m_ReadAsyncResult = SocketStream.BeginRead(Buf, BufIdx, BUF_SIZE, OnRead, null); //비동기 읽기 시작

            SocketStream.EndWrite(ar); //비동기 쓰기 완료
            SocketStream.Flush();

            if (ReadTimeout != -1)
                TimeoutTimer.Start(); //타이머 측정 시작

            AProtocol reqt = ReqeustProtocolPointer as AProtocol;
            SendedProtocolSuccessfullyEvent(reqt);
            reqt.ProtocolRequestedEvent(this, reqt);
        }

        /// <summary>
        /// 이더넷 통신으로부터 데이터를 받을 경우 호출되는 콜백 메서드
        /// LSIS XGT 설정에 의해 3~255초 동안 입력이 없을 경우 자동적으로 연결을 끊는다.
        /// </summary>
        /// <param name="ar"></param>
        private void OnRead(IAsyncResult ar)
        {
            if (m_TcpClient == null || !IsConnected())
                return;
            int size = 0;
            do
            {
                try
                {
                    size = SocketStream.EndRead(ar); //비동기 읽기 완료
                }
                catch (System.IO.IOException io_exception)
                {
                    m_ReadAsyncResult = null;
                    LOG.Debug(Description + " 대기시간 초과로 서버에서 접속 해제");
                    ConnectionStatusChangedEvent(IsConnected());
                    ReqPossible = true;
                    break; //서버측에서 접속 종료하여 탈출
                }
                BufIdx += size;
                if (!IsMatchInstructSize())
                    break; //데이터가 끝까지 오지 않아 탈출
                if (TimeoutTimer.Enabled)
                    TimeoutTimer.Stop();
                else
                    break; //TIMEOUT으로 탈출
                byte[] recv_data = new byte[BufIdx];
                Buffer.BlockCopy(Buf, 0, recv_data, 0, recv_data.Length);
                XGTFEnetProtocol reqt = ReqeustProtocolPointer as XGTFEnetProtocol;
            } while (false);
            m_ReadAsyncResult = SocketStream.BeginRead(Buf, BufIdx, BUF_SIZE, OnRead, null); //비동기 읽기 시작
            if (ReqPossible)
            {
                PrepareTransmission();
                SendNextProtocol();
            }
        }
    }
}