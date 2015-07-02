﻿/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜을 사용하여 통신하는 소켓 구현 (PLC호스트 PC가 게스트일 때)
 * 주의: Cnet통신은 시리얼통신을 기반으로 합니다. 따라서 시리얼통신을 랩핑합시다.
 *       또한 시리얼통신은 동시에 여러 명령어를 보낼 수 없습니다. (1:1 요청 -> 응답 ->요청 ->응답의 루틴) 
 *       쓰레드에 안전하게 설계되어야 합니다.
 * 날짜: 2015-03-25
 */

using System;
using System.IO.Ports;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGTCnetExclusiveProtocol을 이용한 Serial 통신소켓 클래스
    /// </summary>
    public sealed partial class XGTCnetSocket : ASocketCover
    {
        public class Builder : ASerialPortBuilder
        {
            public Builder(string name, int baud)
                : base(name, baud)
            {
            }

            public override object Build()
            {
                var skt = new XGTCnetSocket() { _SerialPort = new SerialPort(_PortName, _BaudRate, _Parity, _DataBits, _StopBits) };
                skt._SerialPort.DataReceived += skt.OnDataRecieve;
                skt._SerialPort.ErrorReceived += skt.OnSerialErrorReceived;
                skt._SerialPort.PinChanged += skt.OnSerialPinChanged;
                return skt;
            }
        }

        #region VAR_PROPERTIES_EVENT
        private const string ERROR_SERIAL_IS_NULL = "_SerialPort is null.";
        private volatile SerialPort _SerialPort;
        /// <summary>
        /// 시리얼포트 에러 이벤트
        /// </summary>
        public event SerialErrorReceivedEventHandler ErrorReceived;
        /// <summary>
        /// 시리얼포트 핀 변경 이벤트
        /// </summary>
        public event SerialPinChangedEventHandler PinChanged;
        #endregion

        #region METHOD

        private XGTCnetSocket()
        {
        }

        /// <summary>
        /// 접속
        /// </summary>
        /// <returns> 통신 접속 성공 여부 </returns>
        public override bool Connect()
        {
            if (_SerialPort == null)
                throw new NullReferenceException(ERROR_SERIAL_IS_NULL);
            if (!_SerialPort.IsOpen)
                _SerialPort.Open();
            return _SerialPort.IsOpen;
        }

        /// <summary>
        /// 끊기
        /// </summary>
        /// <returns> 통신 끊기 성공 여부 </returns>
        public override void Close()
        {
            _SerialPort.Close();
        }

        /// <summary>
        /// 연결 확인 
        /// </summary>
        /// <returns> 결과 </returns>
        public override bool IsOpen()
        {
            return _SerialPort.IsOpen;
        }

        /// <summary>
        /// 프로토콜 전송
        /// </summary>
        /// <param name="iProtocol"> XGTCnetProtocol 프로토콜 클래스, 반드시 사전에 AssembleProtocol() 함수 실행해야함</param>
        public override void Send(IProtocol iProtocol)
        {
            if (_SerialPort == null)
                return;
            if (!_SerialPort.IsOpen)
                throw new Exception("Serial port is not opend");
            AProtocol reqt_p = iProtocol as AProtocol;
            if (reqt_p == null)
                throw new ArgumentNullException("Argument is not XGTCnetProtocol<T>.");
            byte[] reqt_p_asciiprotocol = reqt_p.ASCIIProtocol;
            if (reqt_p_asciiprotocol == null)
            {
                reqt_p.AssembleProtocol();
                reqt_p_asciiprotocol = reqt_p.ASCIIProtocol;
            }
            if (IsWait) //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(reqt_p);
                return;
            }
            ReqeustProtocol = reqt_p;
            _SerialPort.Write(reqt_p_asciiprotocol, 0, reqt_p_asciiprotocol.Length);
            SendedProtocolSuccessfullyEvent(reqt_p);
            reqt_p.ProtocolRequestedEvent(this, reqt_p);
            IsWait = true;
        }

        /// <summary>
        /// 요청 프로토콜에 의한 응답 프로토콜 이벤트 메서드
        /// </summary>
        /// <param name="sender"> DYSerialPort 객체 </param>
        /// <param name="e"> SerialDataReceivedEventArgs 이벤트 argument </param>
        private void OnDataRecieve(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = sender as SerialPort;
            do
            {
                if (serialPort == null)
                    break;
                if (!serialPort.IsOpen)
                    break;
                AXGTCnetProtocol reqt_p = ReqeustProtocol as AXGTCnetProtocol;
                if (reqt_p == null)
                    break;
                Type type_T = ReqeustProtocol.GetType().GenericTypeArguments[0]; //<T>의 Type 얻기
                Type type_pt = typeof(XGTCnetProtocol<>).MakeGenericType(type_T); //XGTCnetProtocol<T> 타입 생성

                byte[] recv_data = System.Text.Encoding.ASCII.GetBytes(serialPort.ReadExisting());
                Buffer.BlockCopy(recv_data, 0, Buf, BufIdx, recv_data.Length);
                BufIdx += recv_data.Length;
                if (XGTCnetCCType.ETX != (XGTCnetCCType)(Buf[BufIdx - ((bool)reqt_p.IsExistBCC() ? 2 : 1)]))
                    break;
                byte[] buf_temp = new byte[BufIdx];
                Buffer.BlockCopy(Buf, 0, buf_temp, 0, buf_temp.Length);
                AXGTCnetProtocol resp_p = type_pt.GetMethod("CreateReceiveProtocol").Invoke(reqt_p, new object[] { buf_temp, reqt_p }) as AXGTCnetProtocol;
                try
                {
                    resp_p.AnalysisProtocol();
                }
                catch (Exception exception)
                {
                    resp_p.Error = XGTCnetProtocolError.EXCEPTION;
                }
                finally
                {
                    ReceivedProtocolSuccessfullyEvent(resp_p);
                    if (resp_p.Error == XGTCnetProtocolError.OK)
                        ReqeustProtocol.ProtocolReceivedEvent(this, resp_p);
                    else
                        ReqeustProtocol.ErrorReceivedEvent(this, resp_p);
                    ReqeustProtocol = null;
                    BufIdx = 0;
                    IsWait = false;
                    if (ProtocolStandByQueue.Count != 0)
                        SendNextProtocol();
                }
            }
            while (false);
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
        }

        private void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            if (PinChanged != null)
                PinChanged(sender, e);
        }

        /// <summary>
        /// 메모리 반환
        /// </summary>
        public override void Dispose()
        {
            if (_SerialPort != null)
            {
                this.Close();
                ErrorReceived = null;
                PinChanged = null;

                _SerialPort.Dispose();

                ReqeustProtocol = null;
                _SerialPort = null;
            }
            base.Dispose();
        }
        #endregion
    }
}