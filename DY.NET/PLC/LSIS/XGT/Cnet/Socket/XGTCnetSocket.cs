using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Collections.Generic;
using NLog;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet - XGTCnetProtocol 통신 클래스
    /// IConnect - ISocketCover - ASocketCover - XGTCnetSocket
    /// </summary>
    public partial class XGTCnetSocket : ASocketCover
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
        /// 통신 가능 여부를 파악한다.
        /// </summary>
        /// <returns>통신 가능 여부</returns>
        public override bool IsConnected()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIALPORT_IS_NULL);
            try
            {
                BaseStream.Write(EMPTY_BYTE, 0, 0);
            }
            catch (Exception e)
            {
                return false;
            }
            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// 시리얼통신을 통해 서버와 접속 시도
        /// </summary>
        /// <returns>접속 시도 결과</returns>
        public override bool Connect()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIALPORT_IS_NULL);
            m_SerialPort.Open(); //접속 시도
            BaseStream = m_SerialPort.BaseStream;
            //RS232(USB 컨버트)케이블과 물리적으로 연결되지 않아도 연결이 되는 경우가 있음을 대비하여, 
            //Zero신호를 보내 연결됨을 확인한다.
            if (!IsConnected())
            {
                Close();
                return false;
            }
            ConnectionStatusChangedEvent(true);
            LOG.Debug(Description + " 통신 접속 성공");
            return IsConnected();
        }

        /// <summary>
        /// 시리얼통신 연결 해제
        /// </summary>
        public override void Close()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIALPORT_IS_NULL);
            m_SerialPort.Close();
            ConnectionStatusChangedEvent(false);
            LOG.Debug(Description + " 통신 접속 해제");
        }

        /// <summary>
        /// 메모리 해제 및 반환
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
        /// XGTCnetProtocol 응답 프로토콜 생성
        /// </summary>
        /// <param name="request">요청 프로토콜</param>
        /// <param name="recv_data">ASCII 데이터</param>
        /// <returns></returns>
        protected override AProtocol CreateResponseProtocol(AProtocol request, byte[] recv_data)
        {
            XGTCnetProtocol resp = request.MirrorProtocol as XGTCnetProtocol; //응답 객체 생성 전에 재활용이 가능한지 검토
            if (request.MirrorProtocol == null)
                request.MirrorProtocol = resp = XGTCnetProtocol.CreateResponseProtocol(recv_data, request as XGTCnetProtocol);
            else
                resp.ASCIIProtocol = recv_data;
            resp.AnalysisProtocol();
            return resp;
        }

        /// <summary>
        /// 서버로부터 수신할 데이터가 더 있는지 검사한다.
        /// </summary>
        /// <param name="request">요청 프로토콜</param>
        /// <returns>수신할 데이터가 더 있다면 true, 아니면 false</returns>
        protected override bool DoReadAgain(AProtocol request, int idx)
        {
            var cnet = request as XGTCnetProtocol;
            return !(BaseBuffer[idx - 1 - (cnet.HasBCC() ? 1 : 0)] == XGTCnetCCType.ETX.ToByte());
        }

    }
}