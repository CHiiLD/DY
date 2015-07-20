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

        public override bool Connect()
        {
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
            if (m_Client == null)
                return null;
            NetworkStream ns = m_Client.GetStream();
            AProtocol reqt_p = protocol as AProtocol;
            if (reqt_p == null)
                throw new ArgumentException("Protocol not match AProtocol type");
            await ns.WriteAsync(reqt_p.ASCIIProtocol, 0, reqt_p.ASCIIProtocol.Length);
            //execute event
            SendedProtocolSuccessfullyEvent(reqt_p);
            reqt_p.ProtocolRequestedEvent(this, reqt_p);
            BufferIdx = 0;
            do
            {
                int size = await ns.ReadAsync(Buffer_, BufferIdx, BUFFER_SIZE);
                if (size == 0)
                {
                    if (SignOffReceived != null)
                        SignOffReceived(this, EventArgs.Empty);
                    return null;
                }
                BufferIdx += size;
            } while (!IsMatchInstructSizeSum());
            byte[] recv_data = new byte[BufferIdx];
            Buffer.BlockCopy(Buffer_, 0, recv_data, 0, recv_data.Length);
            BufferIdx = 0;
            AProtocol resp_p = ReportResponseProtocol(reqt_p, recv_data);
            return resp_p;
        }

        public override void Send(IProtocol protocol)
        {
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
        private bool IsMatchInstructSizeSum()
        {
            ushort instruct_size_sum = 0, instruct_size_cnt = 0;
            if (BufferIdx < XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE)
                return false;
            instruct_size_sum = CV2BR.ToValue(new byte[] { Buffer_[16], Buffer_[17] });
            for (int i = XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE; i < BufferIdx - XGTFEnetHeader.APPLICATION_HEARDER_FORMAT_SIZE; i++)
                instruct_size_cnt += Buffer_[i];
            return instruct_size_sum == instruct_size_cnt;
        }

        private AProtocol ReportResponseProtocol(AProtocol reqt_p, byte[] recv_data)
        {
            Type type_T = reqt_p.GetType().GenericTypeArguments[0]; //<T>의 Type 얻기
            Type type_pt = typeof(XGTFEnetProtocol<>).MakeGenericType(type_T); //XGTCnetProtocol<T> 타입 생성
            AProtocol resp_p = reqt_p.MirrorProtocol as AProtocol; //응답 객체 생성 전에 재활용이 가능한지 검토
            if (reqt_p.MirrorProtocol == null)
            {
                resp_p = type_pt.GetMethod("CreateReceiveProtocol").Invoke(reqt_p, new object[] { recv_data, reqt_p }) as AXGTCnetProtocol;
                reqt_p.MirrorProtocol = resp_p;
            }
            else
            {
                resp_p.ASCIIProtocol = recv_data;
            }
#if !DEBUG
            try
            {
#endif
            resp_p.AnalysisProtocol();  //예외 발생
#if !DEBUG
            }
            catch (Exception exception)
            {
                type_pt.GetProperty("Error").SetValue(resp_p, XGTFEnetProtocolError.EXCEPTION);
            }
            finally
            {
#endif
            ReceivedProtocolSuccessfullyEvent(resp_p);
            if ((XGTFEnetProtocolError)type_pt.GetProperty("Error").GetValue(resp_p) == XGTFEnetProtocolError.OK)
                reqt_p.ProtocolReceivedEvent(this, resp_p);
            else
                reqt_p.ErrorReceivedEvent(this, resp_p);
#if !DEBUG
            }
#endif
            return resp_p;
        }

        private void OnRead(IAsyncResult ar)
        {
            if (m_Client == null)
                return;
            if (!IsConnected())
                return;
            NetworkStream ns = m_Client.GetStream();
            int size = 0;
            try
            {
                size += ns.EndRead(ar);
            }
            catch (Exception ex)
            {
                LOG.Debug(ex.Message);
            }
            if (size == 0) //서버측에서 연결을 끊음
            {
                if (SignOffReceived != null)
                    SignOffReceived(this, EventArgs.Empty);
                //Close();
                LOG.Debug("XGT-FEnet PLC에서 종료신호를 보냄(received byte size = 0)");
                return;
            }
            do
            {
                BufferIdx += size;
                if (!IsMatchInstructSizeSum())
                    break;
                byte[] recv_data = new byte[BufferIdx];
                Buffer.BlockCopy(Buffer_, 0, recv_data, 0, recv_data.Length);
                BufferIdx = 0;
                AProtocol reqt_p = SavePoint_ReqeustProtocol as AProtocol;
                ReportResponseProtocol(reqt_p, recv_data);
                Wait = false;
                if (ProtocolStandByQueue.Count != 0)
                    SendNextProtocol();
            } while (false);
            ns.BeginRead(Buffer_, BufferIdx, BUFFER_SIZE, OnRead, null);
        }

        private void OnSended(IAsyncResult ar)
        {
            if (m_Client == null)
                return;
            if (!IsConnected())
                return;
            m_Client.GetStream().EndWrite(ar);
            AProtocol reqt_p = ar.AsyncState as AProtocol;
            SendedProtocolSuccessfullyEvent(reqt_p);
            reqt_p.ProtocolRequestedEvent(this, reqt_p);
        }

        private void SendNextProtocol()
        {
            IProtocol temp = null;
            if (ProtocolStandByQueue.TryDequeue(out temp))
                Send(temp);
        }
    }
}