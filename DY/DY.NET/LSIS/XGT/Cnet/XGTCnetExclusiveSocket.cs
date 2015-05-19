/*
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
    public partial class XGTCnetExclusiveSocket : DYSocket
    {
        /// <summary>
        /// 빌더 디자인 패턴 (빌더 디자인패턴을 모른다면 Effective Java 2/E P21을 참고) 
        /// </summary>
        public class Builder : SerialPortBuilder
        {
            public Builder(string name, int baud)
                : base(name, baud)
            {
            }

            public override object Build()
            {
                var skt = new XGTCnetExclusiveSocket() { _SerialPort = new SerialPort(_PortName, _BaudRate, _Parity, _DataBits, _StopBits) };
                skt._SerialPort.DataReceived += skt.OnDataRecieve;
                skt._SerialPort.ErrorReceived += skt.OnSerialErrorReceived;
                skt._SerialPort.PinChanged += skt.OnSerialPinChanged;
                return skt;
            }
        }

        #region var_properties_event
        private const string _SerialNullError = "XGTCnetExclusiveSocket._SerialPort var is null";
        protected volatile bool IsWaitACKProtocol = false;
        protected readonly object _SerialLock = new object();
        protected volatile SerialPort _SerialPort;
        protected IProtocol ReqtProtocol;
        protected IProtocol RecvProtocol;

        /// <summary>
        /// 시리얼포트 에러 이벤트
        /// </summary>
        public event SerialErrorReceivedEventHandler ErrorReceived;
        /// <summary>
        /// 시리얼포트 핀 변경 이벤트
        /// </summary>
        public event SerialPinChangedEventHandler PinChanged;
        #endregion

        #region method

        protected XGTCnetExclusiveSocket()
        {

        }

        /// <summary>
        /// 접속
        /// </summary>
        /// <returns> 통신 접속 성공 여부 </returns>
        public override bool Connect()
        {
            bool result;
            lock(_SerialLock)
            {
                if (_SerialPort == null)
                    throw new NullReferenceException(_SerialNullError);
                if (!_SerialPort.IsOpen)
                    _SerialPort.Open();
                result = _SerialPort.IsOpen;
            }
            return result;
        }

        /// <summary>
        /// 끊기
        /// </summary>
        /// <returns> 통신 끊기 성공 여부 </returns>
        public override bool Close()
        {
            bool result;
            lock (_SerialLock)
            {
                if (_SerialPort == null)
                    return false;
                if (_SerialPort.IsOpen)
                    _SerialPort.Close();
                result = !_SerialPort.IsOpen;
            }
            return result;
        }

        /// <summary>
        /// 연결 확인 
        /// </summary>
        /// <returns> 결과 </returns>
        public override bool IsOpen()
        {
            bool result;
            lock (_SerialLock)
                result = _SerialPort.IsOpen;
            return result;
        }

        /// <summary>
        /// 프로토콜 전송
        /// </summary>
        /// <param name="iProtocol"> XGTCnetExclusiveProtocol 프로토콜 클래스 </param>
        public override void Send(IProtocol iProtocol)
        {
            if (iProtocol == null)
                throw new ArgumentNullException("protocol argument is null");
            if (!(iProtocol is XGTCnetExclusiveProtocol))
                throw new ArgumentException("protocol not match XGTCnetExclusiveProtocolFrame type");
            
            XGTCnetExclusiveProtocol exProtocol = iProtocol as XGTCnetExclusiveProtocol;
            XGTCnetExclusiveProtocol copy = new XGTCnetExclusiveProtocol(exProtocol);
            if (copy.ProtocolData == null)
                copy.AssembleProtocol();
            if (IsWaitACKProtocol)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(copy);
                return;
            }
            lock(_SerialLock)
            {
                if (_SerialPort == null)
                    return;
                if (!_SerialPort.IsOpen)
                    throw new Exception("serial port is not opend");
                RecvProtocol = null;
                ReqtProtocol = copy;
                _SerialPort.Write(copy.ProtocolData, 0, copy.ProtocolData.Length);
            }
            IsWaitACKProtocol = true;
            if (OnSendedSuccessfully != null)
                OnSendedSuccessfully(this, EventArgs.Empty);
            copy.OnDataRequested(this, copy);
        }

        /// <summary>
        /// 메모리 반환
        /// </summary>
        public override void Dispose()
        {
            lock (_SerialLock)
            {
                if (_SerialPort != null)
                {
                    Close();

                    ErrorReceived = null;
                    PinChanged = null;

                    _SerialPort.Dispose();
                    _SerialPort = null;
                    
                    ReqtProtocol = null;
                    RecvProtocol = null;
                }
            }
            base.Dispose();
        }

        /// <summary>
        /// 요청 프로토콜에 의한 응답 프로토콜 이벤트 메서드
        /// </summary>
        /// <param name="sender"> DYSerialPort 객체 </param>
        /// <param name="e"> SerialDataReceivedEventArgs 이벤트 argument </param>
        protected void OnDataRecieve(object sender, SerialDataReceivedEventArgs e)
        {
            XGTCnetExclusiveProtocol recv, reqt;
            lock (_SerialLock)
            {
                SerialPort serialPort = sender as SerialPort;
                if (serialPort == null)
                    return;
                if (!serialPort.IsOpen)
                    return;
                recv = this.RecvProtocol as XGTCnetExclusiveProtocol;
                reqt = this.ReqtProtocol as XGTCnetExclusiveProtocol;
                byte[] recv_data = System.Text.Encoding.ASCII.GetBytes(serialPort.ReadExisting());
                if (recv == null)
                {
                    recv = XGTCnetExclusiveProtocol.CreateReceiveProtocol(recv_data, reqt);
                    this.RecvProtocol = recv;
                }
                else
                {
                    byte[] newBytes = new byte[recv.ProtocolData.Length + recv_data.Length];
                    Buffer.BlockCopy(recv.ProtocolData, 0, newBytes, 0, recv.ProtocolData.Length);
                    Buffer.BlockCopy(recv_data, 0, newBytes, recv.ProtocolData.Length, recv_data.Length);
                    recv.ProtocolData = newBytes;
                }
            }

            if (!recv.IsComeInEXTTail())
                return;
            else if (OnReceivedSuccessfully != null)
                OnReceivedSuccessfully(this, EventArgs.Empty);

            try
            {
                recv.AnalysisProtocol();  //예외 발생
            }
            catch (Exception exception)
            {
                recv.Error = XGTCnetExclusiveProtocolError.EXCEPTION;
#if DEBUG
                Console.WriteLine(exception.Message);
                System.Diagnostics.Debug.Assert(false);
#endif
            }
            finally
            {
                if (recv.Error == XGTCnetExclusiveProtocolError.OK)
                    reqt.OnDataReceived(this, recv);
                else
                    reqt.OnError(this, recv);

                
                IsWaitACKProtocol = false;
                if (ProtocolStandByQueue.Count != 0)
                    SendNextProtocol(); //Task.Factory.StartNew(new Action(SendNextProtocol));
            }
        }

        protected virtual void SendNextProtocol()
        {
            IProtocol temp = null;
            if (ProtocolStandByQueue.TryDequeue(out temp))
                Send(temp);
        }

        protected void OnSerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            if (ErrorReceived != null)
                ErrorReceived(sender, e);
        }

        protected void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            if (PinChanged != null)
                PinChanged(sender, e);
        }
        #endregion
    }
}