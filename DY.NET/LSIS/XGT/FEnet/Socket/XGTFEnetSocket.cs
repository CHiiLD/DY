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
        private IPEndPoint RemoteEP;
        private Socket FEnetSocket;
        private Stream NetworkStream;

        /// <summary>
        /// new 생성 방지
        /// </summary>
        protected XGTFEnetSocket()
        {

        }

        public class Builder : AEthernetBuilder
        {
            public Builder(string host, int port)
                : base(host, port)
            {
            }

            public override object Build()
            {
                var fenet_skt = new XGTFEnetSocket();
                IPHostEntry ipHostInfo = Dns.GetHostEntry(_Host);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                fenet_skt.RemoteEP = new IPEndPoint(ipAddress, _Port);
                fenet_skt.FEnetSocket = new Socket(_AddressFamily, _SocketType, _ProtocolType);
                return fenet_skt;
            }
        }

        public override bool Connect()
        {
            //비동기 요청
            if (!FEnetSocket.Connected)
            {
                FEnetSocket.Connect(RemoteEP);
                NetworkStream = new NetworkStream(FEnetSocket);
                NetworkStream.BeginRead(Buf, BufIdx, BUFFER_SIZE, OnRead, this);
            }
            return FEnetSocket.Connected;
        }

        public override void Send(IProtocol iProtocol)
        {
            if (NetworkStream == null)
                return;
            if (!(iProtocol is XGTFEnetProtocol))
                throw new ArgumentException("PROTOCOL NOT MATCH XGT_FENE_PROTOCOL TYPE");
            XGTFEnetProtocol cpy_p = iProtocol as XGTFEnetProtocol;
            if (IsWait)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(cpy_p);
                return;
            }
            cpy_p.AssembleProtocol();
            NetworkStream.BeginWrite(cpy_p.ASC2Protocol, 0, cpy_p.ASC2Protocol.Length, OnSended, cpy_p);
            IsWait = true;
        }

        private void OnSended(IAsyncResult ar)
        {
            Stream stream = NetworkStream;
            var cpy_p = ar.AsyncState as XGTFEnetProtocol;
            if (stream == null)
                return;
            stream.EndWrite(ar);
            if (SendedSuccessfully != null)
                SendedSuccessfully(this, new DataReceivedEventArgs(cpy_p));
            cpy_p.OnDataRequested(this, cpy_p);
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
            if (NetworkStream != null)
                NetworkStream.Close();
            if (FEnetSocket != null)
                FEnetSocket.Close();
        }

        public override bool IsOpen()
        {
            if (FEnetSocket != null)
                return FEnetSocket.Connected;
            return false;
        }

        public virtual new void Dispose()
        {
            if (NetworkStream != null)
                NetworkStream.Dispose();
            if (FEnetSocket != null)
                FEnetSocket.Dispose();
            NetworkStream = null;
            FEnetSocket = null;
            base.Dispose();
        }
        
        private void OnRead(IAsyncResult ar)
        {
            var networkStream = NetworkStream;
            if (networkStream == null)
                return;
            int len = 0;
            len = networkStream.EndRead(ar) + BufIdx;
            if (len == 0) //서버측에서 연결을 끊음
            {
                //ServerDisConnectEvent(this, null);
                return;
            }
            BufIdx = 0;
            while (len > 0)
            {
                int messageLength = Array.IndexOf(Buf, (byte)'\n', 0, len);
                if (messageLength == -1) // Incomplete message
                {
                    BufIdx = len;
                    break;
                }
                messageLength++;
                Array.Copy(Buf, messageLength, Buf, 0, len - messageLength);
                len -= messageLength;
                byte[] recv_asc_data = (byte[]) Buf.Clone();
                XGTFEnetProtocol reqt_p = ar.AsyncState as XGTFEnetProtocol;
                var resp_p = XGTFEnetProtocol.CreateXGTFEnetProtocol(recv_asc_data, reqt_p);
                try
                {
                    resp_p.AnalysisProtocol();  //예외 발생
                }
                finally
                {
                    if (ReceivedSuccessfully != null)
                        ReceivedSuccessfully(this, new DataReceivedEventArgs(resp_p));
                    if (resp_p.Error == XGTFEnetProtocolError.OK) 
                        reqt_p.OnDataReceived(this, resp_p);
                    else 
                        reqt_p.OnError(this, resp_p);
                }
            }
            //비동기 리딩 재요청
            networkStream.BeginRead(Buf, BufIdx, BUFFER_SIZE - BufIdx, OnRead, null);
        }
    }
}
