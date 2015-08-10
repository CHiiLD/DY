using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DY.NET.LSIS.XGT
{
    public sealed partial class XGTCnetSocket
    {
        /// <summary>
        /// 비동기적으로 요청프로토콜을 보내고 응답 프로토콜을 받아 리턴.
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public async Task<IProtocol> PostAsync(IProtocol protocol)
        {
            if (m_SerialPort == null)
                return null;

            if (!IsConnected())
            {
                if (!Connect())
                    return null;
            }

            m_SerialPort.DataReceived -= OnDataRecieve;
            XGTCnetProtocol reqt = protocol as XGTCnetProtocol;
            if (reqt == null)
                throw new ArgumentException("Protocol not match AXGTCnetProtocol type");
            await SocketStream.WriteAsync(reqt.ASCIIProtocol, 0, reqt.ASCIIProtocol.Length);
            //execute event
            SendedProtocolSuccessfullyEvent(reqt);
            reqt.ProtocolRequestedEvent(this, reqt);
            BufIdx = 0;
            bool loop;
            do
            {
                int size = await SocketStream.ReadAsync(Buf, BufIdx, BUF_SIZE - BufIdx);
                if (size == 0)
                    break;
                BufIdx += size;
                loop = Buf[BufIdx - 1 - (reqt.IsExistBCC() ? 1 : 0)] == XGTCnetCCType.ETX.ToByte();
            } while (!loop);
            byte[] recv_data = new byte[BufIdx];
            Buffer.BlockCopy(Buf, 0, recv_data, 0, recv_data.Length);
            BufIdx = 0;
            return ReportResponseProtocol(reqt, recv_data);
        }

        /// <summary>
        /// 서버와 통신하여 통신 속도를 측정한다. 
        /// </summary>
        /// <param name="args">프로토콜 생성에 필요한 아규먼트</param>
        /// <returns> 
        /// -1: Disconnect 상태
        /// -2: 타임아웃
        /// -3: 프로토콜 통신 에러
        /// 0>=: 요청 시 응답까지의 속도
        /// </returns>
        public override async Task<long> PingAsync()
        {
            if (!IsConnected())
                return -1;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("%DW0", null); //모든 XGT의 공통으로 가지고 있는 메모리
            XGTCnetProtocol cnet_test_protocol = XGTCnetProtocol.NewRSSProtocol(typeof(ushort), 00, dic);
            Stopwatch watch = Stopwatch.StartNew();
            XGTCnetProtocol result_protocol = await PostAsync(cnet_test_protocol) as XGTCnetProtocol;
            watch.Stop();
            if (result_protocol == null)
            {
                LOG.Debug(Description + " PingPong Timeout(" + ReadTimeout + ")");
                return -2;
            }
            return watch.ElapsedMilliseconds;
        }
    }
}
