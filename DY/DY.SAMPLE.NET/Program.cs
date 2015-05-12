/* DY.SAMPLE.NET
 * 
 * 작성자: CHILD	
 * 
 * 기능: DY.NET Library Sample Test App.
 * PLC 에서는 2초 마다 한번씩 D00100 변수에서 값이 1씩 증가합니다.
 * PC 에서 D00100 변수에 저장된 값을 읽어 Value 변수에 저장합니다. 
 * 현재 D00100 변수에 저장된 값이 Value 값과 비교하여 값이 증가되었다면, 콘솔창에 변화를 출력합니다. 
 * 콘솔 창에 정수 값을 넣으면 WORD값 범위 내에서, D00100 변수를 지정된 정수값으로 Write 합니다.
 * 
 * 날짜: 2015-05-02
 */

using System;
using System.Text;
using System.Threading.Tasks;
using DY.NET;
using DY.NET.LSIS.XGT;
using System.IO.Ports;
using System.Threading;

namespace DY.SAMPLE.NET
{
    class Program
    {
        /// <summary>
        /// PLC 네트워크 국번
        /// </summary>
        const ushort PLC_LOCAL_PORT = 00;
        /// <summary>
        /// PLC 에서 값을 Read/Write 할 변수
        /// </summary>
        const string PLC_CHECK_VAL = "%DW00100";
        /// <summary>
        /// 읽은 값을 저장할 변수
        /// </summary>
        static object Value = new short();
        /// <summary>
        /// PLC LSIS XGT Cnet Socekt 객체
        /// </summary>
        static XGTCnetExclusiveSocket CnetExclusiveSocket;

        /// <summary>
        /// 메인 함수
        /// </summary>
        /// <param name="args"> 실행 인자 </param>
        static void Main(string[] args)
        {
            //소켓 생성 하기
            CnetExclusiveSocket = new XGTCnetExclusiveSocket("COM3", 9600, Parity.None, 8, StopBits.One);

            //통신 연결 하기
            if (CnetExclusiveSocket.Connect())
            {
                Task.Factory.StartNew(new Action(RepeatExecute));
                Loop();
            }
            else
            {
                Console.WriteLine("시리얼 통신 연결 실패!");
            }
            //통신 연결 끊기
            CnetExclusiveSocket.Close();
            //메모리 반환
            CnetExclusiveSocket.Dispose();
        }

        static void Loop()
        {
            int integer;
            string line;

            while (true)
            {
                Console.WriteLine("1. {0} 변수의 값을 새로 Write 하고 싶다면 정수값을 입력해주세요.(N)\n2. 프로그램 종료하시겠습니까?(Q || q)");
                line = Console.ReadLine();
                if (line == "q" || line == "Q")
                {
                    break;
                }
                else if (Int32.TryParse(line, out integer))
                {
                    var wss = XGTCnetExclusiveProtocol.GetWSSProtocol(PLC_LOCAL_PORT, new ENQDataFormat(PLC_CHECK_VAL, (short)integer));
                    wss.ErrorEvent += OnRSSProtocolError;
                    wss.ReceivedEvent += (object sender, SocketDataReceivedEventArgs e) => { Console.WriteLine("WRITE에 성공하였습니다."); };
                    CnetExclusiveSocket.Send(wss);
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                }
            }
        }

        static void RepeatExecute()
        {
            CnetExclusiveSocket.Send(CreateRSSProtocolObject());
        }

        static XGTCnetExclusiveProtocol CreateRSSProtocolObject()
        {
            var rss_p = XGTCnetExclusiveProtocol.GetRSSProtocol(PLC_LOCAL_PORT, new ENQDataFormat(PLC_CHECK_VAL));
            rss_p.ReceivedEvent += OnRSSProtocolDataReceive;
            rss_p.ErrorEvent += OnRSSProtocolError;
            return rss_p;
        }

        static void OnRSSProtocolDataReceive(object sender, SocketDataReceivedEventArgs e)
        {
            var p = e.Protocol as XGTCnetExclusiveProtocol;
            short recv_data = (short)p.ACKDatas[0].Data;

            lock (Value)
            {
                if ((short)Value != recv_data)
                {
                    Console.WriteLine("{0} Current Value => {1}", PLC_CHECK_VAL, recv_data);
                    Value = recv_data;
                }
            }
            RepeatExecute();
        }

        static void OnRSSProtocolError(object sender, SocketDataReceivedEventArgs e)
        {
            Console.WriteLine("RSSProtocol 통신 중 에러 발생");
            var p = e.Protocol as XGTCnetExclusiveProtocol;
            Console.WriteLine("에러 코드 : " + p.Error.ToString());
        }

        /// <summary>
        /// SerialPort 객체 자체에서 에러가 발생한 경우
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnSerialPortErrorRecived(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine("시리얼 포트 에러 발생! EVENT TYPE :" + e.EventType.ToString());
        }
    }
}