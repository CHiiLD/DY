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
        private class CommPackage
        {
            public int BufferIndex = 0;
            public IPEndPoint RemoteEP;
            public Socket FEnetSocket;
            public Stream NetworkStream;
            public byte[] Buffer = new byte[STREAM_BUFFER_SIZE];
            public volatile bool IsWait = true;
        }

        private const int STREAM_BUFFER_SIZE = 2048;
        private volatile CommPackage CP = new CommPackage();

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
                fenet_skt.CP.RemoteEP = new IPEndPoint(ipAddress, _Port);
                fenet_skt.CP.FEnetSocket = new Socket(_AddressFamily, _SocketType, _ProtocolType);
                return fenet_skt;
            }
        }

        public override bool Connect()
        {
            //비동기 요청
            if (!CP.FEnetSocket.Connected)
            {
                CP.FEnetSocket.Connect(CP.RemoteEP);
                CP.NetworkStream = new NetworkStream(CP.FEnetSocket);
                CP.NetworkStream.BeginRead(CP.Buffer, CP.BufferIndex, STREAM_BUFFER_SIZE, OnRead, this);
            }
            return CP.FEnetSocket.Connected;
        }

        public override void Send(IProtocol iProtocol)
        {
            if (CP.NetworkStream == null)
                return;
            if (!(iProtocol is XGTFEnetProtocol))
                throw new ArgumentException("PROTOCOL NOT MATCH XGT_FENE_PROTOCOL TYPE");
            XGTFEnetProtocol cpy_p = iProtocol as XGTFEnetProtocol;
            if (CP.IsWait)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(cpy_p);
                return;
            }
            cpy_p.AssembleProtocol();
            CP.NetworkStream.BeginWrite(cpy_p.ASC2Protocol, 0, cpy_p.ASC2Protocol.Length, OnSended, cpy_p);
            CP.IsWait = true;
        }

        private void OnSended(IAsyncResult ar)
        {
            Stream stream = CP.NetworkStream;
            var cpy_p = ar.AsyncState as XGTFEnetProtocol;
            if (stream == null)
                return;
            stream.EndWrite(ar);
            if (SendedSuccessfully != null)
                SendedSuccessfully(this, new DataReceivedEventArgs(cpy_p));
            cpy_p.OnDataRequested(this, cpy_p);
            CP.IsWait = false;
            if (ProtocolStandByQueue.Count != 0)
            {
                IProtocol temp = null;
                if (ProtocolStandByQueue.TryDequeue(out temp))
                    Send(temp);
            }
        }

        public override void Close()
        {
            if (CP.NetworkStream != null)
                CP.NetworkStream.Close();
            if (CP.FEnetSocket != null)
                CP.FEnetSocket.Close();
        }

        public override bool IsOpen()
        {
            if (CP.FEnetSocket != null)
                return CP.FEnetSocket.Connected;
            return false;
        }

        public virtual new void Dispose()
        {
            if (CP.NetworkStream != null)
                CP.NetworkStream.Dispose();
            if (CP.FEnetSocket != null)
                CP.FEnetSocket.Dispose();
            CP = null;
            base.Dispose();
        }
        
        private void OnRead(IAsyncResult ar)
        {
            var networkStream = CP.NetworkStream;
            if (networkStream == null)
                return;
            int len = 0;
            len = networkStream.EndRead(ar) + CP.BufferIndex;
            if (len == 0) //서버측에서 연결을 끊음
            {
                //ServerDisConnectEvent(this, null);
                return;
            }
            CP.BufferIndex = 0;
            while (len > 0)
            {
                int messageLength = Array.IndexOf(CP.Buffer, (byte)'\n', 0, len);
                if (messageLength == -1) // Incomplete message
                {
                    CP.BufferIndex = len;
                    break;
                }
                messageLength++;
                Array.Copy(CP.Buffer, messageLength, CP.Buffer, 0, len - messageLength);
                len -= messageLength;
                byte[] recv_asc_data = (byte[]) CP.Buffer.Clone();
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
            networkStream.BeginRead(CP.Buffer, CP.BufferIndex, STREAM_BUFFER_SIZE - CP.BufferIndex, OnRead, null);
        }
    }
}
