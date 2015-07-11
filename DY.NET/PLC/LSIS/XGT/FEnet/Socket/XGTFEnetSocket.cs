using System;
using System.Net.Sockets;

namespace DY.NET.LSIS.XGT
{
    public class XGTFEnetSocket : ASocketCover
    {
        private string m_Host;
        private XGTFEnetPort m_Port;
        private TcpClient m_Client = new TcpClient();

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
            m_Port = port;
        }

        ~XGTFEnetSocket()
        {
            Dispose();
        }

        public override bool Connect()
        {
            //비동기 요청
            if (!m_Client.Connected)
            {
                m_Client.Connect(m_Host, (int)m_Port);
                m_Client.GetStream().BeginRead(Buf, BufIdx, BUFFER_SIZE, OnRead, m_Client);
            }
            return m_Client.Connected;
        }

        public override void Send(IProtocol protocol)
        {
            if (m_Client == null)
                return;
            AProtocol reqt_p = protocol as AProtocol;
            if (reqt_p == null)
                throw new ArgumentException("Protocol not match xgt_fene_protocol type");
            byte[] reqt_data = reqt_p.ASCIIProtocol;
            if (reqt_data == null)
            {
                reqt_p.AssembleProtocol();
                reqt_data = reqt_p.ASCIIProtocol;
            }
            if (IsWait)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(reqt_p);
                return;
            }
            ReqeustProtocol = reqt_p;
            m_Client.GetStream().BeginWrite(reqt_data, 0, reqt_data.Length, OnSended, m_Client);
            IsWait = true;
        }

        private void OnSended(IAsyncResult ar)
        {
            if (m_Client == null)
                return;
            if (!m_Client.Connected)
                return;
            m_Client.GetStream().EndWrite(ar);

            SendedProtocolSuccessfullyEvent(ReqeustProtocol);
            ReqeustProtocol.ProtocolRequestedEvent(this, ReqeustProtocol);

            IsWait = false;
            if (ProtocolStandByQueue.Count != 0)
                SendNextProtocol();
        }

        private void SendNextProtocol()
        {
            IProtocol temp = null;
            if (ProtocolStandByQueue.TryDequeue(out temp))
                Send(temp);
        }

        public override void Close()
        {
            if (m_Client != null)
                m_Client.Close();
        }

        public override bool IsConnected()
        {
            if (m_Client == null)
                return false;
            return m_Client.Connected;
        }

        public override void Dispose()
        {
            if (m_Client != null)
            {
                Close();
                m_Client.GetStream().Dispose();
            }
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        private void ReportResponseProtocol(byte[] recv_data)
        {
            AProtocol reqt_p = ReqeustProtocol as AProtocol;
            Type type_T = ReqeustProtocol.GetType().GenericTypeArguments[0]; //<T>의 Type 얻기
            Type type_pt = typeof(XGTFEnetProtocol<>).MakeGenericType(type_T); //XGTCnetProtocol<T> 타입 생성
            AProtocol resp_p = type_pt.GetMethod("CreateResponseProtocol").Invoke(reqt_p, new object[] { recv_data, reqt_p }) as AProtocol;
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
        }

        private void OnRead(IAsyncResult ar)
        {
            do
            {
                if (m_Client == null)
                    break;
                if (!m_Client.Connected)
                    break;
                NetworkStream stream = m_Client.GetStream();
                try { BufIdx = stream.EndRead(ar); }
                catch (Exception exception) { }

                if (BufIdx == 0) //서버측에서 연결을 끊음
                {
                    if (SignOffReceived != null)
                        SignOffReceived(this, EventArgs.Empty);
                    break;
                }
                byte[] recv_data = new byte[BufIdx];
                Buffer.BlockCopy(Buf, 0, recv_data, 0, recv_data.Length);
                ReportResponseProtocol(recv_data);
                BufIdx = 0;
                stream.BeginRead(Buf, BufIdx, BUFFER_SIZE, OnRead, m_Client);
            } while (false);
        }
    }
}