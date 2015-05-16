using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DY.NET;
using DY.NET.LSIS.XGT;
using System.IO.Ports;

namespace DY.SAMPLE.LOGIC
{
    public class SwitchLoopLogic
    {
        private const ushort PLC_LOCAL_PORT = 00;
        private const string PLC_SWITCH_VAL = "%MX00010";
        private const string PLC_READ_VAL = "%DW00100";
        private readonly object _lock4skt = new object();
        private XGTCnetExclusiveSocket _CnetExclusiveSocket;
        private bool _IsExecute = false;
        public event EventHandler<IntegerReceivedToPlcEventArgs> OnReceivedValueEvent;

        public XGTCnetExclusiveSocket CnetExclusiveSocket
        {
            get
            {
                lock (_lock4skt)
                    return _CnetExclusiveSocket;
            }
        }
         
        ~SwitchLoopLogic()
        {
            if (CnetExclusiveSocket != null)
                CnetExclusiveSocket.Dispose();
            _CnetExclusiveSocket = null;
        }

        public SwitchLoopLogic(XGTCnetExclusiveSocket xgtskt)
        {
            _CnetExclusiveSocket = xgtskt;
        }

        public void CheckStart()
        {
            _IsExecute = true;
            if (CnetExclusiveSocket.Connect())
                RepeatExecute();
        }

        public void CheckStop()
        {
            _IsExecute = false;
        }

        private void RepeatExecute()
        {
            if (_IsExecute)
                CnetExclusiveSocket.Send(CreateRSSP4SwitchValue());
        }

        private XGTCnetExclusiveProtocol CreateRSSP4SwitchValue()
        {
            var rss_p = XGTCnetExclusiveProtocol.NewRSSProtocol(PLC_LOCAL_PORT, new ENQDataFormat(PLC_SWITCH_VAL));
            rss_p.ReceivedEvent += OnDataReceiveFromSwitchVar;
            rss_p.ErrorEvent += OnRSSProtocolError;
            return rss_p;
        }

        private XGTCnetExclusiveProtocol CreateRSSP4GetValue()
        {
            var rss_p = XGTCnetExclusiveProtocol.NewRSSProtocol(PLC_LOCAL_PORT, new ENQDataFormat(PLC_READ_VAL));
            rss_p.ReceivedEvent += OnDataReceiveFromReadVar;
            rss_p.ErrorEvent += OnRSSProtocolError;
            return rss_p;
        }

        private XGTCnetExclusiveProtocol CreateWSSP4SwtichValue()
        {
            var rss_p = XGTCnetExclusiveProtocol.NewWSSProtocol(PLC_LOCAL_PORT, new ENQDataFormat(PLC_SWITCH_VAL, 0));
            rss_p.ReceivedEvent += (object sender, SocketDataReceivedEventArgs e) => { RepeatExecute(); };
            rss_p.ErrorEvent += OnRSSProtocolError;
            return rss_p;
        }

        private void OnDataReceiveFromSwitchVar(object sender, SocketDataReceivedEventArgs e)
        {
            var p = e.Protocol as XGTCnetExclusiveProtocol;
            byte recv_data = (byte)p.ACKDatas[0].Data;

            if (recv_data == 1)
            {
                CnetExclusiveSocket.Send(CreateRSSP4GetValue());
            }
            else
            {
                RepeatExecute();
            }
        }

        private void OnDataReceiveFromReadVar(object sender, SocketDataReceivedEventArgs e)
        {
            if (!_IsExecute)
                return;
            var p = e.Protocol as XGTCnetExclusiveProtocol;
            short recv_data = (short)p.ACKDatas[0].Data;
            Console.WriteLine("Value in {0} -> {1}", PLC_READ_VAL, recv_data);
            if (OnReceivedValueEvent != null)
                OnReceivedValueEvent(this, new IntegerReceivedToPlcEventArgs(recv_data));
            CnetExclusiveSocket.Send(CreateWSSP4SwtichValue());
        }

        private void OnRSSProtocolError(object sender, SocketDataReceivedEventArgs e)
        {
            Console.WriteLine("RSSProtocol 통신 중 에러 발생");
            var p = e.Protocol as XGTCnetExclusiveProtocol;
            Console.WriteLine("에러 코드 : " + p.Error.ToString());
        }
    }
}
