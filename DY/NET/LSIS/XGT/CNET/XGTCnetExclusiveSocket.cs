/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜을 사용하여 통신하는 소켓 구현 (PLC호스트 PC가 게스트일 때)
 * 주의: Cnet통신은 시리얼통신을 기반으로 합니다. 따라서 시리얼통신을 랩핑합시다.
 *       또한 시리얼통신은 동시에 여러 명령어를 보낼 수 없습니다. (1:1 요청 -> 응답 ->요청 ->응답의 루틴) 
 *       쓰레드에 안전하게 설계되어야 합니다.
 * 날짜: 2015-03-25
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DY.NET.LSIS.XGT
{
    public class XGTCnetExclusiveSocket : DYSocket, IDisposable
    {
        #region var_properties_event

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
        private readonly object _SyncCommunication = new object();
        
        #endregion

        #region method

        public XGTCnetExclusiveSocket(DYSerialPort serialPort) : base()
        {
            Serial = serialPort;
            if (Serial == null)
                throw new ArgumentNullException("SerialSocket", "paramiter value is null");

            //이벤트 설정
            Serial.DataReceived += new SerialDataReceivedEventHandler(OnDataRecieve);
            Serial.ErrorReceived += new SerialErrorReceivedEventHandler(OnErrorReceive);
            Serial.PinChanged += new SerialPinChangedEventHandler(OnPinChanged);
        }

        //통신 연결
        public override bool Connect()
        {
            if (Serial == null)
                throw new NullReferenceException("SerialSocket value is null");
            Serial.Open();
            return Serial.IsOpen;
        }

        //통신 닫기
        public override bool Close()
        {
            if (Serial == null)
                throw new NullReferenceException("SerialSocket value is null");

            Serial.Close();
            return !Serial.IsOpen;
        }

        //요청
        public override void Send(IProtocol iProtocol)
        {
            if (Serial == null)
                return;
            if (!Serial.IsOpen)
                throw new Exception("serial port is not opend");
            if (iProtocol == null)
                throw new ArgumentNullException("protocol argument is null");

            XGTCnetExclusiveProtocol protocol = iProtocol as XGTCnetExclusiveProtocol;
            if (protocol == null)
                throw new ArgumentException("protocol not match XGTCnetExclusiveProtocolFrame type");

            lock (_SyncCommunication)
            {
                if(protocol.BinaryData == null)
                    protocol.AssembleProtocol();
                if (IsWaitACKProtocol == true)   //만일 ack응답이 오지 않았다면 큐에 저장하고 대기
                {
                    ProtocolStandByQueue.Enqueue(protocol);
                    return;
                }
                IsWaitACKProtocol = true;
                Serial.ReqtProtocol = protocol;
                Serial.Write(protocol.BinaryData, 0, protocol.BinaryData.Length);
                protocol.OnDataRequestedEvent(this, new SocketDataReceivedEventArgs(protocol));
                Console.WriteLine(this.GetType().Name + ": SEND PROTOCOL MSG");
            }
        }

        //응답
        protected void OnDataRecieve(object sender, SerialDataReceivedEventArgs e)
        {
            DYSerialPort dySerial = sender as DYSerialPort;
            byte[] recievedData = System.Text.Encoding.ASCII.GetBytes(dySerial.ReadExisting());
            lock (_SyncCommunication)
            {
                XGTCnetExclusiveProtocol receive = dySerial.RecvProtocol as XGTCnetExclusiveProtocol;
                XGTCnetExclusiveProtocol request = dySerial.ReqtProtocol as XGTCnetExclusiveProtocol;
                if (receive == null)
                {
                    receive = XGTCnetExclusiveProtocol.CreateReceiveProtocol(recievedData, request);
                    dySerial.RecvProtocol = receive;
                }
                else
                {
                    byte[] newBytes = new byte[receive.BinaryData.Length + recievedData.Length];
                    Buffer.BlockCopy(receive.BinaryData, 0, newBytes, 0, receive.BinaryData.Length);
                    Buffer.BlockCopy(recievedData, 0, newBytes, receive.BinaryData.Length, recievedData.Length);
                    receive.BinaryData = newBytes;
                }

                if (!receive.IsComeInEXTTail()) //데이터가 끝까지 도착하지 않으면 다음 신호가 올 때까지 컷
                    return;

                Console.WriteLine(this.GetType().Name + ": RECEIVED PROTOCOL MSG");

                receive.AnalysisProtocol();
                if (receive.Error == XGTCnetExclusiveProtocolError.OK)
                    request.OnDataReceivedEvent(this, new SocketDataReceivedEventArgs(receive));  //메인스레드 처리
                else
                    request.OnErrorEvent(this, new SocketDataReceivedEventArgs(receive));         //메인스레드 처리

                dySerial.ProtocolClear();
                IsWaitACKProtocol = false;
                if (ProtocolStandByQueue.Count != 0)
                    Task.Factory.StartNew(() => { Send(ProtocolStandByQueue.Dequeue()); });
            }
        }

        //에러 응답
        protected void OnErrorReceive(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine("Received Error");
        }

        //SerialPort 개체의 직렬 핀 변경 이벤트를 처리할 메서드를 나타냅니다. 참고 (https://msdn.microsoft.com/ko-kr/library/system.io.ports.serialport.pinchanged(v=vs.110).aspx)
        protected void OnPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Console.WriteLine("Received PinChange Alarm");
        }

        //리소스 해제
        public void Dispose()
        {
            if (Serial != null)
            {
                Close();
                Serial.ProtocolClear();
                Serial.Dispose();
            }
            ProtocolStandByQueue.Clear();
        }
        #endregion
    }
}
