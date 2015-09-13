using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DY.NET.HONEYWELL.VUQUEST;
using DY.NET.LSIS.XGT;
using DY.NET;

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// 디바이스 별, 통신방법 정의
    /// </summary>
    public static class ServiceableDevice
    {
        public static readonly Dictionary<NetDevice, CommunicationType> Service = new Dictionary<NetDevice, CommunicationType>
        {
            { NetDevice.LSIS_XGT,                  CommunicationType.SERIAL | CommunicationType.ETHERNET },
            { NetDevice.HONEYWELL_VUQUEST3310G,    CommunicationType.SERIAL },
#if false
            { DYDevice.DATALOGIC_MATRIX200,       DYDeviceCommType.SERIAL },
#endif
        };

        /// <summary>
        /// DY.NET 라이브러리에서 지원하는 클라이언트 통신 객체를 생성한다
        /// </summary>
        /// <param name="device"></param>
        /// <param name="comm_type"></param>
        /// <param name="summayParameter"></param>
        /// <returns></returns>
        public static IConnect CreateClient(NetDevice device, CommunicationType comm_type, ISummary summayParameter)
        {
            IConnect ret = null;
            CommSerialPortElement s = summayParameter as CommSerialPortElement;
            CommEthernetElement e = summayParameter as CommEthernetElement;
            if (comm_type == CommunicationType.SERIAL && s == null)
                throw new ArgumentException("device, comm_type mismatch error");
            if (comm_type == CommunicationType.ETHERNET && e == null)
                throw new ArgumentException("device, comm_type mismatch error");

            switch (device)
            {
                case NetDevice.HONEYWELL_VUQUEST3310G:
                    ret = (Vuquest3310g) new Vuquest3310g.Builder(s.PortName, s.BaudRate).DataBits(s.DataBit)
                        .Parity(s.ParityBit).StopBits(s.StopBit).Build();
                    break;
                case NetDevice.LSIS_XGT:
                    switch (comm_type)
                    {
                        case CommunicationType.ETHERNET:
                            ret = new XGTFEnetSocket(e.Host, (XGTFEnetPort)e.Port);
                            break;
                        case CommunicationType.SERIAL:
                            ret = (IConnect)new XGTCnetSocket.Builder(s.PortName, s.BaudRate, s.LocalPort).DataBits(s.DataBit)
                        .Parity(s.ParityBit).StopBits(s.StopBit).Build();
                            break;
                    }
                    break;
            }
            return ret;
        }
    }
}