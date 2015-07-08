﻿using System;
using System.Net.Sockets;

namespace DY.NET.LSIS.XGT
{
    public class XGTFEnetSocket : ASocketCover
    {
        private string Host;
        private XGTFEnetPort Port;
        private TcpClient Client = new TcpClient();

        /// <summary>
        /// 서버로부터 연결 종료 신호를 받았을 때 이벤트 발생
        /// </summary>
        public EventHandler<EventArgs> ReceivedSignOff { get; set; }
        
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
            AProtocol reqt_p = iProtocol as AProtocol;
            if (reqt_p == null)
                throw new ArgumentException("Protocol not match xgt_fene_protocol type");
            byte[] reqt_p_asciiprotocol = reqt_p.ASCIIProtocol;
            if (reqt_p_asciiprotocol == null)
            {
                reqt_p.AssembleProtocol();
                reqt_p_asciiprotocol = reqt_p.ASCIIProtocol;
            }
            if (IsWait)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(reqt_p);
                return;
            }
            ReqeustProtocol = reqt_p;
            Client.GetStream().BeginWrite(reqt_p_asciiprotocol, 0, reqt_p_asciiprotocol.Length, OnSended, Client);
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
                AProtocol reqt_p = ReqeustProtocol as AProtocol;
                if (reqt_p == null)
                    break;
                Type type_T = ReqeustProtocol.GetType().GenericTypeArguments[0]; //<T>의 Type 얻기
                Type type_pt = typeof(XGTFEnetProtocol<>).MakeGenericType(type_T); //XGTCnetProtocol<T> 타입 생성

                byte[] buf_temp = new byte[BufIdx];
                Buffer.BlockCopy(Buf, 0, buf_temp, 0, buf_temp.Length);
                AProtocol resp_p = type_pt.GetMethod("CreateResponseProtocol").Invoke(reqt_p, new object[] { buf_temp, reqt_p }) as AProtocol;
                try
                {
                    resp_p.AnalysisProtocol();  //예외 발생
                }
                catch (Exception exception)
                {
                    type_pt.GetProperty("Error").SetValue(resp_p, XGTFEnetProtocolError.EXCEPTION);
                }
                finally
                {
                    ReceivedProtocolSuccessfullyEvent(resp_p);
                    if ((XGTFEnetProtocolError)type_pt.GetProperty("Error").GetValue(resp_p) == XGTFEnetProtocolError.OK)
                        reqt_p.ProtocolReceivedEvent(this, resp_p);
                    else
                        reqt_p.ErrorReceivedEvent(this, resp_p);
                    BufIdx = 0;
                    stream.BeginRead(Buf, BufIdx, BUFFER_SIZE, OnRead, Client);
                }
            } while (false);
        }
    }
}