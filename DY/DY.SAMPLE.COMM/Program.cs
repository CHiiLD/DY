/*
 * 작성자: CHILD	
 * 기능: XGT CNET 통신 예제 
 * 날짜: 2015-06-24
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.NET;
using DY.NET.LSIS.XGT;
using System.IO.Ports;

namespace DY.SAMPLE.COMM
{
    /// <summary>
    /// PLC -> D00000 ~ D0000F 까지 각 1 에서 16까지 값을 채워놓은 상태 (모두 워드) 
    ///        D00010 ~ D0001F 까지 각 1 에서 16까지 값을 채워놓은 상태 (D000F1은 비트, D000F2는 바이트, D000F3는 워드, D000F4는 더블워드, D000F5는 라지워드 나머지는 워드)
    /// 
    /// </summary>
    class Program
    {
        const ushort LOCAL_NUMBER = 00;
        static XGTCnetSocket Socket;
        /// <summary>
        /// RSS 응답 프로토콜 데이터 출력
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        static void OnReceived_RSS(object obj, SocketDataReceivedEventArgs args)
        {
            var recv_protocol = (XGTCnetResponseProtocol)args.Protocol;
            recv_protocol.PrintBinaryFrameInfo();
        }

        static void OnReqeusted_RSS(object obj, SocketDataReceivedEventArgs args)
        {
            var recv_protocol = (XGTCnetRequestProtocol)args.Protocol;
            recv_protocol.PrintBinaryFrameInfo();
        }

        /// <summary>
        /// 에러 발생시 호출
        /// </summary>
        /// <param name="obj">XGTCnetSOCKET</param>
        /// <param name="args">Protocol 랩핑 이벤트 클래스</param>
        static void OnError(object obj, SocketDataReceivedEventArgs args)
        {
            System.Diagnostics.Debug.Assert(false);
        }

        static void Main(string[] args)
        {
            Socket = (XGTCnetSocket)new XGTCnetSocket.Builder("COM4", 9600).Build();
            Socket.ErrorReceived += (object serialPortObj, SerialErrorReceivedEventArgs sere) =>
            {
                Console.WriteLine("SerialPort Error : " + sere.EventType.ToString());
            };

            string[] vars_1 = {   "%DW00000", "%DW00001", "%DW00002", "%DW00003",
                                        "%DW00004", "%DW00005", "%DW00006", "%DW00007", 
                                        "%DW00008", "%DW00009", "%DW00010", "%DW00011", 
                                        "%DW00012", "%DW00013", "%DW00014", "%DW00015", };

            string[] vars_2 = {   "%DW00016", "%DX000170", "%DB00017", "%DW00019",
                                        "%DD00032", "%DL00034", "%DW00022", "%DW00023", 
                                        "%DW00024", "%DW00025", "%DW00026", "%DW00027", 
                                        "%DW00028", "%DW00029", "%DW00030", "%DW00031", };

            XGTCnetRequestProtocol rssReqtProtocol1, rssReqtProtocol2, rssReqtProtocol3,
                rsbReqtProtocol1, rsbReqtProtocol2, wsbReqtProtocol3;

            // RSS 1
            {
                List<ReqtDataFmt> datas = new List<ReqtDataFmt>();
                foreach (var var_name in vars_1)
                    datas.Add(new ReqtDataFmt(var_name));
                rssReqtProtocol1 = XGTCnetRequestProtocol.NewRSSProtocol(LOCAL_NUMBER, datas);
                rssReqtProtocol1.DataReceived += OnReceived_RSS;
                rssReqtProtocol1.DataRequested += OnReqeusted_RSS;
                rssReqtProtocol1.ErrorReceived += OnError;
            }

            //RSS 2
            {
                List<ReqtDataFmt> datas = new List<ReqtDataFmt>();
                foreach (var var_name in vars_2)
                    datas.Add(new ReqtDataFmt(var_name));
                rssReqtProtocol2 = XGTCnetRequestProtocol.NewRSSProtocol(LOCAL_NUMBER, datas);
                rssReqtProtocol2.DataReceived += OnReceived_RSS;
                rssReqtProtocol2.ErrorReceived += OnError;
            }

            //RSS 3
            {   //"%DD02000", "%DB08001"
                string[] vars = { "%DL01000" }; //, "%DW00038", "%DW00039", "%DW00040", "%DW00041" 
                List<ReqtDataFmt> datas = new List<ReqtDataFmt>();
                foreach (var var_name in vars)
                    datas.Add(new ReqtDataFmt(var_name));
                rssReqtProtocol3 = XGTCnetRequestProtocol.NewRSSProtocol(LOCAL_NUMBER, datas);
                rssReqtProtocol3.DataReceived += OnReceived_RSS;
                rssReqtProtocol3.DataRequested += OnReqeusted_RSS;
                rssReqtProtocol3.ErrorReceived += OnError;
            }

            {
                string[] vars = { "%DW04003" }; //2004318071
                List<ReqtDataFmt> datas = new List<ReqtDataFmt>();
                foreach (var var_name in vars)
                    datas.Add(new ReqtDataFmt(var_name, 0x1234));
                wsbReqtProtocol3 = XGTCnetRequestProtocol.NewWSSProtocol(LOCAL_NUMBER, datas);
                wsbReqtProtocol3.DataReceived += OnReceived_RSS;
                wsbReqtProtocol3.DataRequested += OnReqeusted_RSS;
                wsbReqtProtocol3.ErrorReceived += OnError;
            }

            //RSB 1
            {
                rsbReqtProtocol1 = XGTCnetRequestProtocol.NewRSBProtocol(LOCAL_NUMBER, vars_1[0], (ushort)vars_1.Length);
                rsbReqtProtocol1.DataReceived += OnReceived_RSS;
                rsbReqtProtocol1.DataRequested += OnReqeusted_RSS;
                rsbReqtProtocol1.ErrorReceived += OnError;
            }

            //RSB 2
            {
                rsbReqtProtocol2 = XGTCnetRequestProtocol.NewRSBProtocol(LOCAL_NUMBER, vars_2[2], 14);
                rsbReqtProtocol2.DataReceived += OnReceived_RSS;
                rsbReqtProtocol2.DataRequested += OnReqeusted_RSS;
                rsbReqtProtocol2.ErrorReceived += OnError;
            }

            if (Socket.Connect())
            {
                //Socket.Send(rssReqtProtocol1);
                //Socket.Send(rssReqtProtocol3);
                Socket.Send(wsbReqtProtocol3);
                //Socket.Send(rsbReqtProtocol1);
                //Socket.Send(rsbReqtProtocol2);
            }
            Console.ReadKey();
        }
    }
}
