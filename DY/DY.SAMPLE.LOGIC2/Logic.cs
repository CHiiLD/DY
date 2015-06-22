//#define PRINT_OUT

using System;
using System.Collections.Generic;

using DY.NET.LSIS.XGT;
using DY.NET;

namespace DY.SAMPLE.LOGIC2
{
    public class Logic
    {
        public event EventHandler<DY.SAMPLE.LOGIC.ValueEventArgs> RecvEvent;

        private ushort _LocalPort;
        private string _SwitchVariable;
        private string _StorageVariable;
        public XGTCnetSocket Socket;
        private volatile bool _isExecute = false;

        private const ushort REGISTER_NUMBER_0 = 0x00;
        private const ushort REGISTER_NUMBER_1 = 0x01;
        private const byte ON = 1;
        private const byte OFF = 0;

        public Logic(XGTCnetSocket skt, ushort localport, string switch_var, string storage_var)
        {
            Socket = skt;
            _LocalPort = localport;
            _SwitchVariable = switch_var;
            _StorageVariable = storage_var;
        }

        /// <summary>
        /// XSS 프로토콜을 사용하여 RSS 등록
        /// </summary>
        public void RegisterXSSProtocolToPLC()
        {
            if(Socket.Connect())
            {
                var p = XGTCnetRequestProtocol.NewXSSProtocol(_LocalPort, REGISTER_NUMBER_0, new List<ReqtDataFmt>() { new ReqtDataFmt(_SwitchVariable) });
                p.Description = "ON/OFF보는 XSS 등록";
                p.ReceivedEvent += (object o, SocketDataReceivedEventArgs e) => 
                {
                    ((XGTCnetResponseProtocol)e.Protocol).PrintBinaryFrameInfo(); 
                };
                p.ErrorEvent += OnErrorError;
                Socket.Send(p);

                p = XGTCnetRequestProtocol.NewXSBProtocol(_LocalPort, REGISTER_NUMBER_1, _StorageVariable, 1);
                p.Description = "값 읽는 XSB 등록";
                p.ReceivedEvent += (object o, SocketDataReceivedEventArgs e) =>
                {
                    ((XGTCnetResponseProtocol)e.Protocol).PrintBinaryFrameInfo();
                };
                p.ErrorEvent += OnErrorError;
                Socket.Send(p);
            }
        }

        public void Start()
        {
            _isExecute = true;
            LookSwtichVar();
        }

        public void Stop()
        {
            _isExecute = false;
        }

        /// <summary>
        /// 1. ON / OFF 를 쭉 보다가
        /// </summary>
        public void LookSwtichVar()
        {
            if (_isExecute)
            {
                var p = XGTCnetRequestProtocol.NewYSSProtocol(_LocalPort, REGISTER_NUMBER_0);
                p.Description = "ON/OFF 확인해보기";
                p.ReceivedEvent += OnLookSwtichVar;
                p.ErrorEvent += OnErrorError;
                Socket.Send(p);
            }
        }

        /// <summary>
        /// 2. 만약에 ON 이 되었다면
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnLookSwtichVar(object o, SocketDataReceivedEventArgs e)
        {
#if PRINT_OUT
            ((XGTCnetExclusiveProtocol)e.Protocol).PrintBinaryFrameInfo();
#endif
            var bit = (byte)((XGTCnetResponseProtocol)e.Protocol).RespDatas[0].Data;
            if (bit == ON)
            {
                var p = XGTCnetRequestProtocol.NewYSBProtocol(_LocalPort, REGISTER_NUMBER_1, PLCVarType.WORD);
                p.Description = "읽어들일 값 요청하기";
                p.ReceivedEvent += OnReadStorageVar;
                p.ErrorEvent += OnErrorError;
                Socket.Send(p);
            }
            else
            {
                LookSwtichVar();
            }
        }

        /// <summary>
        /// 3. 지정된 변수에 카운팅된 값을 읽고 다시 스위치를 OFF 로 바꾸자
        /// 그리고 다시 1 로 가서 반복
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public void OnReadStorageVar(object o, SocketDataReceivedEventArgs e)
        {
#if PRINT_OUT
            ((XGTCnetExclusiveProtocol)e.Protocol).PrintBinaryFrameInfo();
#endif
            short storage_value = (short)((XGTCnetResponseProtocol)e.Protocol).RespDatas[0].Data;
            
            if (RecvEvent != null)
                RecvEvent(this, new LOGIC.ValueEventArgs(storage_value));

            var p = XGTCnetRequestProtocol.NewWSSProtocol(_LocalPort, new ReqtDataFmt(_SwitchVariable, OFF));
            p.Description = "다시 OFF 시키자";
            p.ReceivedEvent += (object sender, SocketDataReceivedEventArgs args) => 
            {
#if PRINT_OUT
                ((XGTCnetExclusiveProtocol)e.Protocol).PrintBinaryFrameInfo();
#endif
                LookSwtichVar(); 
            };
            p.ErrorEvent += OnErrorError;
            Socket.Send(p);
        }

        private void OnErrorError(object o, SocketDataReceivedEventArgs e)
        {
            System.Diagnostics.Debug.Assert(false);
        }
    }
}
