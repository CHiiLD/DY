using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using NLog;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT FEnet 이더넷 클라이언트 소켓 클래스
    /// </summary>
    public class XGTFEnetSocket : ASocketCover, IPostAsync
#if false
        , IConnectAsync
#endif
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private string m_Host;
        private int m_Port;
        private volatile TcpClient m_Client;
        private volatile NetworkStream m_NetworkStream;
        private IAsyncResult m_WriteAsyncResult;
        private IAsyncResult m_ReadAsyncResult;

        /// <summary>
        /// 서버로부터 연결 종료 신호를 받았을 때 이벤트 발생
        /// </summary>
        public EventHandler<EventArgs> SignOffReceived { get; set; }

        /// <summary>
        /// new 생성 방지
        /// </summary>
        public XGTFEnetSocket(string host, XGTFEnetPort port)
        {
            m_Host = host;
            m_Port = (int) port;
        }

        public XGTFEnetSocket(string host, int port)
        {
            m_Host = host;
            m_Port = port;
        }

        ~XGTFEnetSocket()
        {
            Dispose();
        }

        private void EndAsync()
        {
            if (m_ReadAsyncResult != null)
                m_NetworkStream.EndRead(m_ReadAsyncResult);
            if (m_WriteAsyncResult != null)
                m_NetworkStream.EndRead(m_WriteAsyncResult);

            m_WriteAsyncResult = null;
            m_ReadAsyncResult = null;
        }

        /// <summary>
        /// 비동기적으로 요청프로토콜을 보내고 응답 프로토콜을 받아 리턴.
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public async Task<IProtocol> PostAsync(IProtocol protocol)
        {
            if (!IsConnected())
            {
                if (!Connect())
                {
                    LOG.Debug(m_Host + ":" + m_Port + " 프로토콜을 보내기 위해 서버와 접속시도를 하였으나 실패");
                    return null;
                }
            }

            EndAsync();
            IProtocol temp;
            while (!ProtocolStandByQueue.IsEmpty)
                ProtocolStandByQueue.TryDequeue(out temp);

            XGTFEnetProtocol reqt = protocol as XGTFEnetProtocol;
            if (reqt == null)
                throw new ArgumentException("Protocol not match XGTFEnetProtocol type");
            await m_NetworkStream.WriteAsync(reqt.ASCIIProtocol, 0, reqt.ASCIIProtocol.Length);
            
            SendedProtocolSuccessfullyEvent(reqt);
            reqt.ProtocolRequestedEvent(this, reqt);
            BufIdx = 0;
            do
            {
                int size = await m_NetworkStream.ReadAsync(Buf, BufIdx, BUF_SIZE - BufIdx);
                if (size == 0)
                {
                    if (SignOffReceived != null)
                        SignOffReceived(this, EventArgs.Empty);
                    return null;
                }
                BufIdx += size;
            } while (!IsMatchInstructSize());
            byte[] recv_data = new byte[BufIdx];
            Buffer.BlockCopy(Buf, 0, recv_data, 0, recv_data.Length);
            BufIdx = 0;
            return ReportResponseProtocol(reqt, recv_data);
        }

        public override bool Connect()
        {
            //throw new NotImplementedException();
            //비동기 요청
            if (!IsConnected())
            {
                m_Client = new TcpClient(m_Host, m_Port);
                m_NetworkStream = m_Client.GetStream();
            }
            ConnectionStatusChangedEvent(IsConnected());
            LOG.Debug("XGT-FEnet 이더넷 통신 동기 접속");
            return IsConnected();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override bool IsConnected()
        {
            if (m_Client == null)
                return false;
            if (m_Client.Client == null)
                return false;
            return m_Client.Connected;
        }

        public override void Dispose()
        {
            if (m_Client != null)
            {
                EndAsync();

                if (m_Client != null)
                    m_Client.Close();

                ConnectionStatusChangedEvent(IsConnected());

                m_NetworkStream = null;
                m_Client = null;
                m_Host = null;
                m_Port = -1;
            }

            base.Dispose();
            GC.SuppressFinalize(this);
            LOG.Debug("XGT-FEnet 이더넷 통신 해제 및 메모리 해제");
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

        public override void Send(IProtocol protocol)
        {
            if (m_Client == null)
                return;

            if (!IsConnected())
            {
                if (!Connect())
                {
                    LOG.Debug(m_Host + ":" + m_Port + " 프로토콜을 보내기 위해 서버와 접속시도를 하였으나 실패");
                    return;
                }
            }

            AProtocol reqt = protocol as AProtocol;
            if (reqt == null)
                throw new ArgumentException("Protocol not match AProtocol type");
            byte[] reqt_bytes = reqt.ASCIIProtocol;
            if (IsWait)
            {
                ProtocolStandByQueue.Enqueue(reqt);
                return;
            }
            IsWait = true;
            ReqeustProtocolPointer = reqt;
            
            if (m_ReadAsyncResult == null)
                m_ReadAsyncResult = m_NetworkStream.BeginRead(Buf, BufIdx, BUF_SIZE, OnRead, null); //비동기 읽기 시작
            /********** byte data write to buffer **********/
            m_WriteAsyncResult = m_NetworkStream.BeginWrite(reqt_bytes, 0, reqt_bytes.Length, OnSended, null);
        }

        private void OnSended(IAsyncResult ar)
        {
            //throw new NotImplementedException();
            if (m_Client == null || !IsConnected())
                return;
            /********** buffer data send to plc **********/
            m_NetworkStream.EndWrite(ar);
            m_NetworkStream.Flush();

            IProtocol reqt = ReqeustProtocolPointer;
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
            //throw new NotImplementedException();
            if (m_Client == null || !IsConnected())
                return;
            int size;
            try
            {
                size = m_NetworkStream.EndRead(ar);
            }
            catch(System.IO.IOException io_exception)
            {
                m_ReadAsyncResult = null;
                LOG.Debug(m_Host + ":" + m_Port + " 대기시간 초과로 서버에서 접속 해제");
                return;
            }

            //test code
            if (size <= 0) //서버측에서 연결을 끊음
            {
                if (SignOffReceived != null)
                    SignOffReceived(this, EventArgs.Empty);
                LOG.Debug("XGT-FEnet PLC에서 종료(또는 에러)신호를 보냄 EndRead(ar) return: " + size);
                m_NetworkStream.Flush();
#if DEBUG
                System.Diagnostics.Debug.Assert(false);
#endif
            }
            BufIdx += size;
            do
            {
                if (!IsMatchInstructSize())
                    break;
                byte[] recv_data = new byte[BufIdx];
                Buffer.BlockCopy(Buf, 0, recv_data, 0, recv_data.Length);
                XGTFEnetProtocol reqt = ReqeustProtocolPointer as XGTFEnetProtocol;
                ReportResponseProtocol(reqt, recv_data);
                BufIdx = 0;
                IsWait = false;
            } while (false);
            
            m_ReadAsyncResult = m_NetworkStream.BeginRead(Buf, BufIdx, BUF_SIZE, OnRead, null);
            if (!IsWait)
            {
                if (ProtocolStandByQueue.Count != 0)
                    SendNextProtocol();
            }
        }

        private void SendNextProtocol()
        {
            IProtocol temp = null;
            if (ProtocolStandByQueue.TryDequeue(out temp))
                Send(temp);
        }
    }
}