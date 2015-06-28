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
    public sealed partial class XGTCnetSocket : ASocketCover
    {
        /// <summary>
        /// 빌더 디자인 패턴 (빌더 디자인패턴을 모른다면 Effective Java 2/E P21을 참고) 
        /// </summary>
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
        private const string ERROR_SERIAL_IS_NULL = "XGTCNETEXCLUSIVESOCKET._SERIALPORT VAR IS NULL";

        private volatile bool _IsWait = false;
        private readonly object _SerialLock = new object();
        private volatile SerialPort _SerialPort;
        private XGTCnetProtocol _ReqtProtocol;
        private byte[] _Buffer = new byte[1024];
        private int _BufferIndex;

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

        private XGTCnetSocket()
        {

        }

        /// <summary>
        /// 접속
        /// </summary>
        /// <returns> 통신 접속 성공 여부 </returns>
        public override bool Connect()
        {
            bool result;
            lock (_SerialLock)
            {
                if (_SerialPort == null)
                    throw new NullReferenceException(ERROR_SERIAL_IS_NULL);
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
        public override void Close()
        {
            lock (_SerialLock)
            {
                if (_SerialPort.IsOpen)
                    _SerialPort.Close();
            }
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
                throw new ArgumentNullException("PROTOCOL ARGUMENT IS NULL");
            if (!(iProtocol is XGTCnetProtocol))
                throw new ArgumentException("PROTOCOL NOT MATCH XGTCNETEXCLUSIVEPROTOCOLFRAME TYPE");

            XGTCnetProtocol cpy_p = new XGTCnetProtocol(iProtocol as XGTCnetProtocol);
            if (_IsWait)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
            {
                ProtocolStandByQueue.Enqueue(cpy_p);
                return;
            }
            cpy_p.AssembleProtocol();
            lock (_SerialLock)
            {
                if (_SerialPort == null)
                    return;
                if (!_SerialPort.IsOpen)
                    throw new Exception("SERIAL PORT IS NOT OPEND");
                _ReqtProtocol = cpy_p;
                _SerialPort.Write(cpy_p.ASC2Protocol, 0, cpy_p.ASC2Protocol.Length);
            }
            if (SendedSuccessfully != null)
                SendedSuccessfully(this, new DataReceivedEventArgs(cpy_p));
            cpy_p.OnDataRequested(this, cpy_p);
            _IsWait = true;
        }

        /// <summary>
        /// 요청 프로토콜에 의한 응답 프로토콜 이벤트 메서드
        /// </summary>
        /// <param name="sender"> DYSerialPort 객체 </param>
        /// <param name="e"> SerialDataReceivedEventArgs 이벤트 argument </param>
        private void OnDataRecieve(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_SerialLock)
            {
                SerialPort serialPort = sender as SerialPort;
                if (serialPort == null)
                    return;
                if (!serialPort.IsOpen)
                    return;
                byte[] recv_data = System.Text.Encoding.ASCII.GetBytes(serialPort.ReadExisting());
                Buffer.BlockCopy(recv_data, 0, _Buffer, _BufferIndex, recv_data.Length);
                _BufferIndex += recv_data.Length;
            }
            if (e.EventType != SerialData.Eof)
                return;
            XGTCnetProtocol resp_p = XGTCnetProtocol.CreateReceiveProtocol((byte[])_Buffer.Clone(), _ReqtProtocol);
            try
            {
                resp_p.AnalysisProtocol();  //예외 발생
            }
            finally
            {
                if (ReceivedSuccessfully != null)
                    ReceivedSuccessfully(this, new DataReceivedEventArgs(resp_p));
                if (resp_p.Error == XGTCnetProtocolError.OK)
                    _ReqtProtocol.OnDataReceived(this, resp_p);
                else
                    _ReqtProtocol.OnError(this, resp_p);
                _ReqtProtocol = null;
                _Buffer.Initialize();
                _BufferIndex = 0;
                _IsWait = false;
                if (ProtocolStandByQueue.Count != 0)
                    SendNextProtocol();
            }
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
            lock (_SerialLock)
            {
                if (_SerialPort != null)
                {
                    Close();
                    ErrorReceived = null;
                    PinChanged = null;
                    _SerialPort.Dispose();
                    _SerialPort = null;
                    _ReqtProtocol = null;
                }
            }
            base.Dispose();
        }
        #endregion
    }
}