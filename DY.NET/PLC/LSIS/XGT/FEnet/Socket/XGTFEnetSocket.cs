using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using NLog;

namespace DY.NET.LSIS.XGT
{
    public class XGTFEnetSocket : ASocketCover, IPostAsync, IConnectAsync
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private string m_Host;
        private int m_Port;
        private volatile TcpClient m_Client;

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
        public async Task<bool> ConnectAsync()
        {
            if (!IsConnected())
            {
                m_Client = new TcpClient();
                await m_Client.ConnectAsync(m_Host, m_Port);
                //m_Client.GetStream().BeginRead(Buffer_, BufferIdx, BUFFER_SIZE, OnRead, null);
            }
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs(IsConnected()));
            LOG.Debug("XGT-FEnet 이더넷 통신 비동기 접속");
            return IsConnected();
        }

        /// <summary>
        /// 비동기적으로 요청프로토콜을 보내고 응답 프로토콜을 받아 리턴.
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public async Task<IProtocol> PostAsync(IProtocol protocol)
        {
            if (!IsConnected())
                return null;
            NetworkStream stream = m_Client.GetStream();
            XGTFEnetProtocol reqt = protocol as XGTFEnetProtocol;
            if (reqt == null)
                throw new ArgumentException("Protocol not match AProtocol type");
            await stream.WriteAsync(reqt.ASCIIProtocol, 0, reqt.ASCIIProtocol.Length);
            
            SendedProtocolSuccessfullyEvent(reqt);
            reqt.ProtocolRequestedEvent(this, reqt);
            BufferIdx = 0;
            do
            {
                int size = await stream.ReadAsync(Buffer_, BufferIdx, BUFFER_SIZE);
                if (size == 0)
                {
                    if (SignOffReceived != null)
                        SignOffReceived(this, EventArgs.Empty);
                    return null;
                }
                BufferIdx += size;
            } while (!IsMatchInstructSize());
            byte[] recv_data = new byte[BufferIdx];
            Buffer.BlockCopy(Buffer_, 0, recv_data, 0, recv_data.Length);
            BufferIdx = 0;
            XGTFEnetProtocol resp_p = ReportResponseProtocol(reqt, recv_data);
            return resp_p;
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
                if (m_Client != null)
                    m_Client.Close();
                if (ConnectionStatusChanged != null)
                    ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs(IsConnected()));
                m_Client = null;
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
            if (BufferIdx < XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE)
                return false;
            target = CV2BR.ToValue(new byte[] { Buffer_[16], Buffer_[17] });
            for (int i = XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE; i < BufferIdx; i++) //바이트의 개수
                sum++;
            return target == sum;
        }

        private XGTFEnetProtocol ReportResponseProtocol(XGTFEnetProtocol reqt, byte[] recv_data)
        {
            XGTFEnetProtocol resp = reqt.MirrorProtocol as XGTFEnetProtocol; //응답 객체 생성 전에 재활용이 가능한지 검토
            if (reqt.MirrorProtocol == null)
            {
                resp = XGTFEnetProtocol.CreateResponseProtocol(recv_data, reqt);
                reqt.MirrorProtocol = resp;
            }
            else
            {
                resp.ASCIIProtocol = recv_data;
            }
            resp.AnalysisProtocol();  //예외 발생
            ReceivedProtocolSuccessfullyEvent(resp);
            if (resp.Error == XGTFEnetProtocolError.OK)
                reqt.ProtocolReceivedEvent(this, resp);
            else
                reqt.ErrorReceivedEvent(this, resp);
            return resp;
        }

        private void SendNextProtocol()
        {
            IProtocol temp = null;
            if (ProtocolStandByQueue.TryDequeue(out temp))
                Send(temp);
        }

        public override bool Connect()
        {
            throw new NotImplementedException();
            //비동기 요청
            if (!IsConnected())
            {
                m_Client = new TcpClient(m_Host, m_Port);
                m_Client.GetStream().BeginRead(Buffer_, BufferIdx, BUFFER_SIZE, OnRead, null);
            }
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs(IsConnected()));
            LOG.Debug("XGT-FEnet 이더넷 통신 동기 접속");
            return IsConnected();
        }

        public override void Send(IProtocol protocol)
        {
            throw new NotImplementedException();
            if (m_Client == null)
                return;
            AProtocol reqt_p = protocol as AProtocol;
            if (reqt_p == null)
                throw new ArgumentException("Protocol not match AProtocol type");
            byte[] reqt_data = reqt_p.ASCIIProtocol;
            if (Wait)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(reqt_p);
                return;
            }
            Wait = true;
            SavePoint_ReqeustProtocol = reqt_p;
            m_Client.GetStream().BeginWrite(reqt_data, 0, reqt_data.Length, OnSended, reqt_p);
        }

        private void OnRead(IAsyncResult ar)
        {
            throw new NotImplementedException();
            if (m_Client == null)
                return;
            if (!IsConnected())
                return;
            NetworkStream stream = m_Client.GetStream();
            int size = 0;
            try
            {
                size += stream.EndRead(ar);
            }
            catch (Exception ex)
            {
                LOG.Debug(ex.Message);
            }

            if (size == 0) //서버측에서 연결을 끊음
            {
                if (SignOffReceived != null)
                    SignOffReceived(this, EventArgs.Empty);
                LOG.Debug("XGT-FEnet PLC에서 종료신호를 보냄(received byte size = 0)");
                return;
            }
            do
            {
                BufferIdx += size;
                if (!IsMatchInstructSize())
                    break;
                byte[] recv_data = new byte[BufferIdx];
                Buffer.BlockCopy(Buffer_, 0, recv_data, 0, recv_data.Length);
                BufferIdx = 0;
                XGTFEnetProtocol reqt = SavePoint_ReqeustProtocol as XGTFEnetProtocol;
                ReportResponseProtocol(reqt, recv_data);
                Wait = false;
                if (ProtocolStandByQueue.Count != 0)
                    SendNextProtocol();
            } while (false);
            stream.BeginRead(Buffer_, BufferIdx, BUFFER_SIZE, OnRead, null);
        }

        private void OnSended(IAsyncResult ar)
        {
            throw new NotImplementedException();
            if (m_Client == null)
                return;
            if (!IsConnected())
                return;
            m_Client.GetStream().EndWrite(ar);
            AProtocol reqt_p = ar.AsyncState as AProtocol;
            SendedProtocolSuccessfullyEvent(reqt_p);
            reqt_p.ProtocolRequestedEvent(this, reqt_p);
        }
    }
}