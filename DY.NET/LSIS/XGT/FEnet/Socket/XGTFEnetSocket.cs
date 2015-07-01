using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;

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
                Client.GetStream().BeginRead(Buf, BufIdx, BUFFER_SIZE - BufIdx, OnRead, Client);
            }
            return Client.Connected;
        }

        public override void Send(IProtocol iProtocol)
        {
            if (Client == null)
                return;
            if (!(iProtocol is XGTFEnetProtocol))
                throw new ArgumentException("Protocol not match xgt_fene_protocol type");
            XGTFEnetProtocol cpy_p = iProtocol as XGTFEnetProtocol;
            if (IsWait)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(cpy_p);
                return;
            }
            cpy_p.AssembleProtocol();
            ReqeustProtocol = cpy_p;
            Client.GetStream().BeginWrite(cpy_p.ASC2Protocol, 0, cpy_p.ASC2Protocol.Length, OnSended, Client);
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
            if (SendedSuccessfully != null)
                SendedSuccessfully(this, new DataReceivedEventArgs(ReqeustProtocol));
            ReqeustProtocol.OnDataRequested(this, ReqeustProtocol);
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
            if (Client == null)
                return;
            if (!Client.Connected)
                return;
            NetworkStream stream = Client.GetStream();
            try
            {
                BufIdx = stream.EndRead(ar);
            }
            catch (Exception e)
            {
            }
            if (BufIdx == 0) //서버측에서 연결을 끊음
            {
                if (ReceivedSignOff != null)
                    ReceivedSignOff(this, EventArgs.Empty);
                return;
            }
            byte[] buf_temp = new byte[BufIdx];
            Buffer.BlockCopy(Buf, 0, buf_temp, 0, buf_temp.Length);
            XGTFEnetProtocol reqt_p = ReqeustProtocol as XGTFEnetProtocol;
            var response_protocol = XGTFEnetProtocol.CreateXGTFEnetProtocol(buf_temp, reqt_p);
            try
            {
                response_protocol.AnalysisProtocol();  //예외 발생
            }
            finally
            {
                if (ReceivedSuccessfully != null)
                    ReceivedSuccessfully(this, new DataReceivedEventArgs(response_protocol));
                if (response_protocol.Error == XGTFEnetProtocolError.OK)
                    reqt_p.OnDataReceived(this, response_protocol);
                else
                    reqt_p.OnError(this, response_protocol);
                //stream.BeginRead(Buf, BufIdx, BUFFER_SIZE - BufIdx, OnRead, Client);
            }
        }
    }
}