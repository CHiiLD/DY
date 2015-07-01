using System;
using System.Net.Sockets;

namespace DY.NET.LSIS.XGT
{
    public class XGTFEnetSocket : ASocketCover
    {
        private string Host;
        private XGTFEnetPort Port;
        private TcpClient Client = new TcpClient();
        public EventHandler<EventArgs> ReceivedSignOff { get; set; } //서버로부터 연결 종료 신호를 받았을 때
        /// <summary>
        /// new 생성 방지
        /// </summary>
        public XGTFEnetSocket(string host, XGTFEnetPort port)
        {
            Host = host;
            Port = port;
        }

        public override bool Connect()
        {
            //비동기 요청
            if (!Client.Connected)
            {
                Client.Connect(Host, (int)Port);
                Client.GetStream().BeginRead(Buf, BufIdx, BUFFER_SIZE, OnRead, Client);
            }
            return Client.Connected;
        }

        public override void Send(IProtocol iProtocol)
        {
            if (Client == null)
                return;
            XGTFEnetProtocol<dynamic> reqt_p = iProtocol as XGTFEnetProtocol<dynamic>;
            if (reqt_p == null)
                throw new ArgumentException("Protocol not match xgt_fene_protocol type");
            if (reqt_p.ASC2Protocol == null)
                reqt_p.AssembleProtocol();
            if (IsWait)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(reqt_p);
                return;
            }
            ReqeustProtocol = reqt_p;
            Client.GetStream().BeginWrite(reqt_p.ASC2Protocol, 0, reqt_p.ASC2Protocol.Length, OnSended, Client);
            IsWait = true;
        }

        private void OnSended(IAsyncResult ar)
        {
            var Client = ar.AsyncState as TcpClient;
            if (Client == null)
                return;
            if (!Client.Connected)
                return;
            Client.GetStream().EndWrite(ar);

            SendedProtocolSuccessfullyEvent(ReqeustProtocol);
            ReqeustProtocol.ProtocolRequestedEvent(this, ReqeustProtocol);

            IsWait = false;
            if (ProtocolStandByQueue.Count != 0)
            {
                IProtocol temp = null;
                if (ProtocolStandByQueue.TryDequeue(out temp))
                    Send(temp);
            }
        }

        public override void Close()
        {
            if (Client != null)
            {
                Client.GetStream().Close();
                Client.Close();
            }
        }

        public override bool IsOpen()
        {
            if (Client == null)
                return false;
            return Client.Connected;
        }

        public virtual new void Dispose()
        {
            if (Client != null)
            {
                Close();
                Client = null;
            }
            base.Dispose();
        }

        private void OnRead(IAsyncResult ar)
        {
            var Client = ar.AsyncState as TcpClient;
            do
            {
                if (Client == null)
                    break;
                if (!Client.Connected)
                    break;
                NetworkStream stream = Client.GetStream();
                try { BufIdx = stream.EndRead(ar); }
                catch (Exception exception) { }
                if (BufIdx == 0) //서버측에서 연결을 끊음
                {
                    if (ReceivedSignOff != null)
                        ReceivedSignOff(this, EventArgs.Empty);
                    break;
                }
                XGTFEnetProtocol<dynamic> reqt_p = ReqeustProtocol as XGTFEnetProtocol<dynamic>;
                if (reqt_p == null)
                    break;

                byte[] buf_temp = new byte[BufIdx];
                Buffer.BlockCopy(Buf, 0, buf_temp, 0, buf_temp.Length);
                var resp_p = XGTFEnetProtocol<dynamic>.CreateXGTFEnetProtocol(buf_temp, reqt_p);
                try
                {
                    resp_p.AnalysisProtocol();  //예외 발생
                }
                catch (Exception exception)
                {
                    resp_p.Error = XGTFEnetProtocolError.EXCEPTION;
                }
                finally
                {
                    ReceivedProtocolSuccessfullyEvent(resp_p);
                    if (resp_p.Error == XGTFEnetProtocolError.OK)
                        reqt_p.ProtocolReceivedEvent(this, resp_p);
                    else
                        reqt_p.ErrorReceivedEvent(this, resp_p);
                    BufIdx = 0;
                    stream.BeginRead(Buf, BufIdx, BUFFER_SIZE, OnRead, Client);
                }
            } while(false);
        }
    }
}