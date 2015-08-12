using System;
using System.Net.Sockets;
using NLog;
using System.IO;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet 이더넷 클라이언트 소켓 클래스
    /// </summary>
    public sealed partial class XGTFEnetSocket : ASocketCover
    {
        private const string ERROR_TCPCLIENT_IS_NULL = "TcpClient instance is null.";
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private string m_Host;
        private int m_Port;
        private volatile TcpClient m_TcpClient;

        /// <summary>
        /// 생성자
        /// </summary>
        public XGTFEnetSocket(string host, XGTFEnetPort port)
        {
            m_Host = host;
            m_Port = (int)port;
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
        /// 접속 시도
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            if (!IsConnected())
            {
                m_TcpClient = new TcpClient(m_Host, m_Port);
                BaseStream = m_TcpClient.GetStream();
            }
            ConnectionStatusChangedEvent(true);
            LOG.Debug(Description + " 이더넷 통신 동기 접속");
            return IsConnected();
        }

        /// <summary>
        /// 접속 해제
        /// </summary>
        public override void Close()
        {
            if (m_TcpClient != null)
                m_TcpClient.Close();
            ConnectionStatusChangedEvent(false);
            LOG.Debug(Description + " 이더넷 통신 동기 접속 해제");
        }

        /// <summary>
        /// 메모리 해제 및 반환
        /// </summary>
        public override void Dispose()
        {
            if (m_TcpClient != null)
            {
                if (m_TcpClient != null)
                    m_TcpClient.Close();

                m_TcpClient = null;
                m_Host = null;
                m_Port = -1;
            }

            base.Dispose();
            GC.SuppressFinalize(this);
            LOG.Debug(Description + " 이더넷 통신 해제 및 메모리 해제");
        }

        /// <summary>
        /// 헤더부분의 Length 부분과 실제 Instruct Size를 계산하여 일치하는지 계산 
        /// </summary>
        /// <returns>일치할 시(프로토콜을 모두 전송 받았을 때)true, 아니면 false</returns>
        protected override bool DoReadAgain(AProtocol request)
        {
            ushort target = 0, sum = 0;
            if (StreamBufferIndex < XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE)
                return false;
            target = CV2BR.ToValue(new byte[] { StreamBuffer[16], StreamBuffer[17] });
            for (int i = XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE; i < StreamBufferIndex; i++) //바이트의 개수
                sum++;
            return target != sum;
        }

        /// <summary>
        /// 요청 프로토콜과 ASCII 데이터로 응답 프로토콜을 생성하고 결과를 이벤트로 전달
        /// </summary>
        /// <param name="request">요청 프로토콜 객체</param>
        /// <param name="recv_data">ASCII 데이터</param>
        /// <returns></returns>
        protected override AProtocol ReportResponseProtocol(AProtocol request, byte[] recv_data)
        {
            XGTFEnetProtocol resp = request.MirrorProtocol as XGTFEnetProtocol; //응답 객체 생성 전에 재활용이 가능한지 검토
            if (request.MirrorProtocol == null)
                request.MirrorProtocol = resp = XGTFEnetProtocol.CreateResponseProtocol(recv_data, request as XGTFEnetProtocol);
            else
                resp.ASCIIProtocol = recv_data;
            resp.AnalysisProtocol();
            return resp;
        }
    }
}