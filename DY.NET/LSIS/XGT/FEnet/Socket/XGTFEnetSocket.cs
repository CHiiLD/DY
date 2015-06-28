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
        private const int STREAM_BUFFER_SIZE = 2048;
        private byte[] _Buffer = new byte[STREAM_BUFFER_SIZE];
        private int BufferIndex = 0;
        private IPEndPoint _RemoteEP;
        private Socket _FEnetSocket;
        private Stream _NetworkStream;
        protected volatile bool IsWait = true;
        
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
                fenet_skt._RemoteEP = new IPEndPoint(ipAddress, _Port);
                fenet_skt._FEnetSocket = new Socket(_AddressFamily, _SocketType, _ProtocolType);
                return fenet_skt;
            }
        }

        public override bool Connect()
        {
            //비동기 요청
            if (!_FEnetSocket.Connected)
            {
                _FEnetSocket.Connect(_RemoteEP);
                _NetworkStream = new NetworkStream(_FEnetSocket);
                _NetworkStream.BeginRead(_Buffer, BufferIndex, STREAM_BUFFER_SIZE, OnDataRecieve, this);
            }
            return !IsWait;
        }

        public override void Send(IProtocol iProtocol)
        {
            if (_NetworkStream == null)
                return;

            if (IsWait)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(iProtocol);
                return;
            }
            //NetworkStream.BeginWrite(byteData, 0, byteData.Length, OnSended, NetworkStream);
            IsWait = true;
        }

        private void OnSended(IAsyncResult ar)
        {
            var thiz = ar.AsyncState as XGTFEnetSocket;
            Stream stream = thiz._NetworkStream;
            if (stream == null)
                return;
            stream.EndWrite(ar);
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
            if (_NetworkStream != null)
                _NetworkStream.Close();
            if (_FEnetSocket != null)
                _FEnetSocket.Close();
        }

        public override bool IsOpen()
        {
            if (_FEnetSocket != null)
                return _FEnetSocket.Connected;
            return false;
        }

        public virtual new void Dispose()
        {
            if (_NetworkStream != null)
                _NetworkStream.Dispose();
            if (_FEnetSocket != null)
                _FEnetSocket.Dispose();
            _FEnetSocket = null;
            _NetworkStream = null;
            _RemoteEP = null;
            _Buffer = null;
            base.Dispose();
        }

        
        private void OnDataRecieve(IAsyncResult ar)
        {
            var thiz = (XGTFEnetSocket) ar.AsyncState;
            var networkStream = thiz._NetworkStream;
            if (networkStream == null)
                return;
            int len = 0;
            len = networkStream.EndRead(ar) + thiz.BufferIndex;
            if (len == 0) //서버측에서 연결을 끊음
            {
                //ServerDisConnectEvent(this, null);
                return;
            }
            thiz.BufferIndex = 0;
            while (len > 0)
            {
                int messageLength = Array.IndexOf(thiz._Buffer, (byte)'\n', 0, len);
                if (messageLength == -1) // \n 문자가 없다면 -1 을 반환
                {
                    thiz.BufferIndex = len;
                    break;
                }
                messageLength++;
                Array.Copy(thiz._Buffer, messageLength, thiz._Buffer, 0, len - messageLength);
                len -= messageLength;
                var target = thiz._Buffer.Clone();
            }
            //비동기 리딩 재요청
            networkStream.BeginRead(thiz._Buffer, thiz.BufferIndex, STREAM_BUFFER_SIZE - thiz.BufferIndex, OnDataRecieve, thiz);
            thiz.IsWait = false;
        }
    }
}
