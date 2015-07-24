﻿/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜을 사용하여 통신하는 소켓 구현 (PLC호스트 PC가 게스트일 때)
 * 주의: Cnet통신은 시리얼통신을 기반으로 합니다. 따라서 시리얼통신을 랩핑합시다.
 *       또한 시리얼통신은 동시에 여러 명령어를 보낼 수 없습니다. (1:1 요청 -> 응답 ->요청 ->응답의 루틴) 
 *       쓰레드에 안전하게 설계되어야 합니다.
 * 날짜: 2015-03-25
 */

using System;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using NLog;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet 시리얼 클라이언트 통신 클래스
    /// </summary>
    public partial class XGTCnetSocket : ASocketCover, IPostAsync
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public class Builder : ASerialPortBuilder
        {
            public Builder(string name, int baud)
                : base(name, baud)
            {
            }

            public override object Build()
            {
                var skt = new XGTCnetSocket() { m_SerialPort = new SerialPort(_PortName, _BaudRate, _Parity, _DataBits, _StopBits) };
                //skt.m_SerialPort.DataReceived += skt.OnDataRecieve;
                skt.m_SerialPort.ErrorReceived += skt.OnSerialErrorReceived;
                skt.m_SerialPort.PinChanged += skt.OnSerialPinChanged;
                return skt;
            }
        }

        #region VAR_PROPERTIES_EVENT
        private const string ERROR_SERIAL_IS_NULL = "m_SerialPort is null.";
        private volatile SerialPort m_SerialPort;
        public event SerialErrorReceivedEventHandler ErrorReceived;
        public event SerialPinChangedEventHandler PinChanged;
        #endregion

        #region METHOD
        private XGTCnetSocket()
        {
        }

        ~XGTCnetSocket()
        {
            Dispose();
        }

        /// <summary>
        /// 접속
        /// </summary>
        /// <returns> 통신 접속 성공 여부 </returns>
        public override bool Connect()
        {
            if (m_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIAL_IS_NULL);
            if (!IsConnected())
                m_SerialPort.Open();
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs(IsConnected()));
            LOG.Debug("XGT-Cnet 시리얼포트 통신 접속");
            return IsConnected();
        }

        /// <summary>
        /// 끊기
        /// </summary>
        /// <returns> 통신 끊기 성공 여부 </returns>
        public override void Close()
        {
            if (m_SerialPort != null)
                m_SerialPort.Close();
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectionStatusChangedEventArgs(IsConnected()));
            LOG.Debug("XGT-Cnet 시리얼포트 통신 해제");
        }

        /// <summary>
        /// 연결 확인 
        /// </summary>
        /// <returns> 결과 </returns>
        public override bool IsConnected()
        {
            if (m_SerialPort == null)
                return false;
            return m_SerialPort.IsOpen;
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
            m_SerialPort.DataReceived -= OnDataRecieve;
            Stream stream = m_SerialPort.BaseStream;
            XGTCnetProtocol reqt = protocol as XGTCnetProtocol;
            if (reqt == null)
                throw new ArgumentException("Protocol not match AXGTCnetProtocol type");
            await stream.WriteAsync(reqt.ASCIIProtocol, 0, reqt.ASCIIProtocol.Length);
            //execute event
            SendedProtocolSuccessfullyEvent(reqt);
            reqt.ProtocolRequestedEvent(this, reqt);
            BufIdx = 0;
            bool loop;
            do
            {
                int size = await stream.ReadAsync(Buf, BufIdx, BUF_SIZE - BufIdx);
                if (size == 0)
                    break;
                BufIdx += size;
                loop = Buf[BufIdx - 1 - (reqt.IsExistBCC() ? 1 : 0)] == XGTCnetCCType.ETX.ToByte();
            } while (!loop);
            byte[] recv_data = new byte[BufIdx];
            Buffer.BlockCopy(Buf, 0, recv_data, 0, recv_data.Length);
            BufIdx = 0;
            XGTCnetProtocol resp = ReportResponseProtocol(reqt, recv_data);
            return resp;
        }

        /// <summary>
        /// 프로토콜 전송
        /// </summary>
        /// <param name="protocol"> XGTCnetProtocol 프로토콜 클래스, 반드시 사전에 AssembleProtocol() 함수 실행해야함</param>
        public override void Send(IProtocol protocol)
        {
            if (m_SerialPort == null)
                return;
            if (!IsConnected())
                throw new Exception("Serial port is not opend");
            AProtocol reqt_p = protocol as AProtocol;
            if (reqt_p == null)
                throw new ArgumentNullException("Argument is not XGTCnetProtocol<T>.");
            byte[] reqt_bytes = reqt_p.ASCIIProtocol;
            if (Wait) //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(reqt_p);
                return;
            }
            SavePoint_ReqeustProtocol = reqt_p;
            m_SerialPort.DataReceived += OnDataRecieve;
            m_SerialPort.Write(reqt_bytes, 0, reqt_bytes.Length);
            SendedProtocolSuccessfullyEvent(reqt_p);
            reqt_p.ProtocolRequestedEvent(this, reqt_p);
            Wait = true;
        }

        /// <summary>
        /// 메모리 반환
        /// </summary>
        public override void Dispose()
        {
            if (m_SerialPort != null)
            {
                ErrorReceived = null;
                PinChanged = null;
                SavePoint_ReqeustProtocol = null;
                Close();
                m_SerialPort.Dispose();
            }
            base.Dispose();
            LOG.Debug("XGT-Cnet 시리얼포트 메모리 해제");
            GC.SuppressFinalize(this);
        }

        private XGTCnetProtocol ReportResponseProtocol(XGTCnetProtocol reqt, byte[] buf_temp)
        {
            XGTCnetProtocol resp = reqt.MirrorProtocol as XGTCnetProtocol; //응답 객체 생성 전에 재활용이 가능한지 검토
            if (reqt.MirrorProtocol == null)
            {
                resp = XGTCnetProtocol.CreateResponseProtocol(buf_temp, reqt);
                reqt.MirrorProtocol = resp;
            }
            else
            {
                resp.ASCIIProtocol = buf_temp;
            }
            resp.AnalysisProtocol();
            ReceivedProtocolSuccessfullyEvent(resp);
            if (resp.Error == XGTCnetProtocolError.OK)
                reqt.ProtocolReceivedEvent(this, resp);
            else
                reqt.ErrorReceivedEvent(this, resp);
            return resp;
        }

        /// <summary>
        /// 요청 프로토콜에 의한 응답 프로토콜 이벤트 메서드
        /// </summary>
        /// <param name="sender"> DYSerialPort 객체 </param>
        /// <param name="e"> SerialDataReceivedEventArgs 이벤트 argument </param>
        private void OnDataRecieve(object sender, SerialDataReceivedEventArgs e)
        {
            if (!IsConnected())
                return;

            SerialPort sp = sender as SerialPort;
            BufIdx += sp.Read(Buf, BufIdx, Buf.Length);
            XGTCnetProtocol reqt = SavePoint_ReqeustProtocol as XGTCnetProtocol;
            if (XGTCnetCCType.ETX != (XGTCnetCCType)(Buf[BufIdx - ((bool)reqt.IsExistBCC() ? 2 : 1)]))
                return;
            byte[] recv_data = new byte[BufIdx];
            Buffer.BlockCopy(Buf, 0, recv_data, 0, recv_data.Length);
            ReportResponseProtocol(reqt, recv_data);
            SavePoint_ReqeustProtocol = null;
            BufIdx = 0;
            Wait = false;
            if (ProtocolStandByQueue.Count != 0)
                SendNextProtocol();
        }

        private void SendNextProtocol()
        {
            IProtocol temp = null;
            if (ProtocolStandByQueue.TryDequeue(out temp))
                Send(temp);
        }

        private void OnSerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            if (ErrorReceived != null)
                ErrorReceived(sender, e);
            Close();
            LOG.Error("XGT-Cnet 시리얼포트 에러: " + e.EventType);
        }

        private void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            if (PinChanged != null)
                PinChanged(sender, e);
            Close();
            LOG.Error("XGT-Cnet 시리얼포트 핀 변경 이벤트: " + e.EventType);
        }
        #endregion
    }
}