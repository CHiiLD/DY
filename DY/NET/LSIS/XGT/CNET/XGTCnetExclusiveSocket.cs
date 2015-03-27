/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜을 사용하여 통신하는 소켓 구현 (PLC호스트 PC가 게스트일 때)
 * 첨부: Cnet통신은 시리얼통신을 기반으로 합니다. 따라서 시리얼통신을 랩핑합시다.
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
    public class XGTCnetExclusiveSocket : ISocket, IDisposable
    {
        #region var_properties_event

        public event SocketDataReceivedEventHandler DataReceivedEvent;
       
        public int Tag
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public object UserData
        {
            get;
            set;
        }

        public SerialPort SerialSocket
        {
            get;
            private set;
        }

        protected XGTCnetExclusiveProtocolFrame ReqtFrame; //요청 프레임
        protected XGTCnetExclusiveProtocolFrame RecvFrame; //응답 프레임

        #endregion

        #region method

        public XGTCnetExclusiveSocket(SerialPort serialPort)
        {
            SerialSocket = serialPort;
            if (SerialSocket == null)
                throw new ArgumentNullException("SerialSocket", "paramiter value is null");

            //이벤트 설정
            SerialSocket.DataReceived += new SerialDataReceivedEventHandler(OnDataRecieve);
            SerialSocket.ErrorReceived += new SerialErrorReceivedEventHandler(OnErrorReceive);
            SerialSocket.PinChanged += new SerialPinChangedEventHandler(OnPinChanged);
        }

        //통신 연결
        public bool Connect()
        {
            if (SerialSocket == null)
                throw new NullReferenceException("SerialSocket value is null");

            SerialSocket.Open();
            return SerialSocket.IsOpen;
        }

        //통신 닫기
        public bool Close()
        {
            if (SerialSocket == null)
                throw new NullReferenceException("SerialSocket value is null");

            SerialSocket.Close();
            return !SerialSocket.IsOpen;
        }

        //요청
        public void Send(IProtocol protocolFrame)
        {

        }

        //응답
        protected void OnDataRecieve(object sender, SerialDataReceivedEventArgs e)
        {

        }

        //에러 응답
        protected void OnErrorReceive(object sender, SerialErrorReceivedEventArgs e)
        {

        }

        //SerialPort 개체의 직렬 핀 변경 이벤트를 처리할 메서드를 나타냅니다. 참고 (https://msdn.microsoft.com/ko-kr/library/system.io.ports.serialport.pinchanged(v=vs.110).aspx)
        protected void OnPinChanged(object sender, SerialPinChangedEventArgs e)
        {

        }

        //리소스 해제
        public void Dispose()
        {
            if (SerialSocket != null)
            {
                SerialSocket.Dispose();
                SerialSocket = null;
            }
        }
        #endregion
    }
}
