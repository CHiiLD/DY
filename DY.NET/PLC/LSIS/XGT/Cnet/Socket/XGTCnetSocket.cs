using System;
using System.IO.Ports;
using NLog;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// LSIS XGT Cnet 통신 클래스
    /// </summary>
    public sealed partial class XGTCnetSocket : ASocketCover
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private const string ERROR_SERIALPORT_IS_NULL = "SerialPort instance is null.";
        private SerialPort m_SerialPort;

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
        /// 연결 확인 
        /// </summary>
        /// <returns> 결과 </returns>
        public override bool IsConnected()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIALPORT_IS_NULL);
            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// 접속
        /// </summary>
        /// <returns> 통신 접속 성공 여부 </returns>
        public override bool Connect()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIALPORT_IS_NULL);
            if (!IsConnected())
            {
                m_SerialPort.Open();
                BaseStream = m_SerialPort.BaseStream;
            }
            ConnectionStatusChangedEvent(true);
            LOG.Debug(Description + " 통신 접속 성공");
            return IsConnected();
        }

        /// <summary>
        /// 끊기
        /// </summary>
        /// <returns> 통신 끊기 성공 여부 </returns>
        public override void Close()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIALPORT_IS_NULL);
            m_SerialPort.Close();
            ConnectionStatusChangedEvent(false);
            LOG.Debug(Description + " 통신 접속 해제");
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
            }
            base.Dispose();
            GC.SuppressFinalize(this);
            LOG.Debug(Description + " 메모리 해제");
        }

        /// <summary>
        /// 요청 프로토콜과 ASCII 데이터로 응답 프로토콜을 생성하고 결과를 이벤트로 전달
        /// </summary>
        /// <param name="request">요청 프로토콜 객체</param>
        /// <param name="recv_data">ASCII 데이터</param>
        /// <returns></returns>
        protected override AProtocol ReportResponseProtocol(AProtocol request, byte[] recv_data)
        {
            XGTCnetProtocol resp = request.MirrorProtocol as XGTCnetProtocol; //응답 객체 생성 전에 재활용이 가능한지 검토
            if (request.MirrorProtocol == null)
                request.MirrorProtocol = resp = XGTCnetProtocol.CreateResponseProtocol(recv_data, request as XGTCnetProtocol);
            else
                resp.ASCIIProtocol = recv_data;
            resp.AnalysisProtocol();
            return resp;
        }

        protected override bool DoReadAgain()
        {
            var cnet = ReqeustProtocolPointer as XGTCnetProtocol;
            return !(StreamBuffer[StreamBufferIndex - 1 - (cnet.IsExistBCC() ? 1 : 0)] == XGTCnetCCType.ETX.ToByte());
        }
    }
}