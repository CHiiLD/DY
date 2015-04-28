/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜을 사용하여 통신하는 소켓 구현 (PLC호스트 PC가 게스트일 때)
 * 주의: Cnet통신은 시리얼통신을 기반으로 합니다. 따라서 시리얼통신을 랩핑합시다.
 *       또한 시리얼통신은 동시에 여러 명령어를 보낼 수 없습니다. (1:1 요청 -> 응답 ->요청 ->응답의 루틴) 
 *       쓰레드에 안전하게 설계되어야 합니다.
 * 날짜: 2015-03-25
 */

using System;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGTCnetExclusiveProtocol 과 Serial 통신 사용하기 위해 만들어진 소켓 클래스
    /// </summary>
    public class XGTCnetExclusiveSocket : DYSocket
    {
        #region var_properties_event
        private readonly object _SyncCommunication = new object();
        private readonly object _SyncDYSerialPort = new object();

        private DYSerialPort _DYSerialPort;
        protected DYSerialPort Serial
        {
            get
            {
                lock (_SyncDYSerialPort)
                    return _DYSerialPort;
            }
            set
            {
                lock (_SyncDYSerialPort)
                    _DYSerialPort = value;
            }
        }
        protected bool IsWaitACKProtocol = false;

        #endregion

        #region method

        /// <summary>
        /// XGTCnetExclusiveSocket 생성자
        /// </summary>
        /// <param name="serialPort"> DY시리얼포트 클래스 </param>
        /// <param name="serialError"> 시리얼포트의 에러 핸들러 </param>
        public XGTCnetExclusiveSocket(DYSerialPort serialPort, SerialErrorReceivedEventHandler serialError)
            : base()
        {
            Serial = serialPort;
            if (Serial == null)
                throw new ArgumentNullException("SerialSocket", "paramiter value is null");

            //이벤트 설정
            Serial.DataReceived += new SerialDataReceivedEventHandler(OnDataRecieve);
            Serial.ErrorReceived += new SerialErrorReceivedEventHandler(serialError);
            Serial.PinChanged += new SerialPinChangedEventHandler(OnPinChanged);
        }

        /// <summary>
        /// 접속
        /// </summary>
        /// <returns> 통신 접속 성공 여부 </returns>
        public override bool Connect()
        {
            if (Serial == null)
                throw new NullReferenceException("SerialSocket value is null");
            lock (_SyncCommunication)
                Serial.Open();
            return Serial.IsOpen;
        }

        /// <summary>
        /// 끊기
        /// </summary>
        /// <returns> 통신 끊기 성공 여부 </returns>
        public override bool Close()
        {
            if (Serial == null)
                return false;
            lock (_SyncCommunication)
                Serial.Close();
            return !Serial.IsOpen;
        }

        /// <summary>
        /// 프로토콜 전송
        /// </summary>
        /// <param name="iProtocol"> XGTCnetExclusiveProtocol 프로토콜 클래스 </param>
        public override void Send(IProtocol iProtocol)
        {
            if (Serial == null)
                return;
            if (!Serial.IsOpen)
                throw new Exception("serial port is not opend");
            if (iProtocol == null)
                throw new ArgumentNullException("protocol argument is null");

            XGTCnetExclusiveProtocol exclusiveProtocol = iProtocol as XGTCnetExclusiveProtocol;
            if (exclusiveProtocol == null)
                throw new ArgumentException("protocol not match XGTCnetExclusiveProtocolFrame type");

            lock (_SyncCommunication)
            {
                XGTCnetExclusiveProtocol cpProtocol = new XGTCnetExclusiveProtocol(exclusiveProtocol);
                if (cpProtocol.ProtocolData == null)
                    cpProtocol.AssembleProtocol();
                if (IsWaitACKProtocol)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
                {
                    ProtocolStandByQueue.Enqueue(cpProtocol);
                    return;
                }
                IsWaitACKProtocol = true;
                Serial.ReqtProtocol = cpProtocol;
                Serial.Write(cpProtocol.ProtocolData, 0, cpProtocol.ProtocolData.Length);
                cpProtocol.OnDataRequestedEvent(this, cpProtocol);
            }
        }

        protected void OnDataRecieve(object sender, SerialDataReceivedEventArgs e)
        {
            DYSerialPort serialPort = sender as DYSerialPort;
            if (serialPort == null)
                return;
            if (!serialPort.IsOpen)
                return;

            byte[] recievedData = System.Text.Encoding.ASCII.GetBytes(serialPort.ReadExisting());
            lock (_SyncCommunication)
            {
                XGTCnetExclusiveProtocol recv = serialPort.RecvProtocol as XGTCnetExclusiveProtocol;
                XGTCnetExclusiveProtocol reqt = serialPort.ReqtProtocol as XGTCnetExclusiveProtocol;
                if (recv == null)
                {
                    recv = XGTCnetExclusiveProtocol.CreateReceiveProtocol(recievedData, reqt);
                    serialPort.RecvProtocol = recv;
                }
                else
                {
                    byte[] newBytes = new byte[recv.ProtocolData.Length + recievedData.Length];
                    Buffer.BlockCopy(recv.ProtocolData, 0, newBytes, 0, recv.ProtocolData.Length);
                    Buffer.BlockCopy(recievedData, 0, newBytes, recv.ProtocolData.Length, recievedData.Length);
                    recv.ProtocolData = newBytes;
                }
                if (!recv.IsComeInEXTTail())
                    return;

                try
                {
                    recv.AnalysisProtocol();  //예외 발생
                    if (recv.Error == XGTCnetExclusiveProtocolError.OK)
                        reqt.OnDataReceivedEvent(this, recv);
                    else
                        reqt.OnErrorEvent(this, recv);
                }
                catch (Exception exception)
                {
                    recv.Error = XGTCnetExclusiveProtocolError.EXCEPTION;
                    reqt.OnErrorEvent(this, recv);
#if DEBUG
                    Console.WriteLine(exception.Message);
                    System.Diagnostics.Debug.Assert(false);
#endif
                }
                finally
                {
                    serialPort.ProtocolClear();
                    IsWaitACKProtocol = false;
                    if (ProtocolStandByQueue.Count != 0)
                        Task.Factory.StartNew(new Action(SendNextProtocol));
                }
            }
        }

        protected virtual void SendNextProtocol()
        {
            IProtocol temp = null;
            if (ProtocolStandByQueue.TryDequeue(out temp))
                Send(temp);
        }

        /// <summary>
        /// SerialPort 개체의 직렬 핀 변경 이벤트를 처리할 메서드를 나타냅니다. 
        /// 참고 (https://msdn.microsoft.com/ko-kr/library/system.io.ports.serialport.pinchanged(v=vs.110).aspx) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnPinChanged(object sender, SerialPinChangedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("Received PinChange Alarm");
            System.Diagnostics.Debug.Assert(false);
#endif
        }

        /// <summary>
        /// 메모리 반환
        /// </summary>
        public override void Dispose()
        {
            if (Serial != null)
            {
                Close();
                Serial.ProtocolClear();
                Serial.Dispose();
            }
            base.Dispose();
        }
        #endregion
    }
}
