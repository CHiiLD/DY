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
        public static readonly Dictionary<DyNetDevice, DyNetCommType> Service = new Dictionary<DyNetDevice, DyNetCommType>
        {
            { DyNetDevice.LSIS_XGT,                  DyNetCommType.SERIAL | DyNetCommType.ETHERNET },
            { DyNetDevice.HONEYWELL_VUQUEST3310G,    DyNetCommType.SERIAL },
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
        public static IConnect CreateClient(DyNetDevice device, DyNetCommType comm_type, ISummaryParameter summayParameter)
        {
            IConnect ret = null;
            CommSerialParameter s = summayParameter as CommSerialParameter;
            CommEthernetParameter e = summayParameter as CommEthernetParameter;
            if (comm_type == DyNetCommType.SERIAL && s == null)
                throw new ArgumentException("device, comm_type mismatch error");
            if (comm_type == DyNetCommType.ETHERNET && e == null)
                throw new ArgumentException("device, comm_type mismatch error");

            switch (device)
            {
#if false
                case DYDevice.DATALOGIC_MATRIX200:
                    ret = new Matrix200.Builder(s.Com, s.Bandrate).DataBits(s.DataBit)
                        .Parity(s.Parity).StopBits(s.StopBit).Build();
                    break;
#endif
                case DyNetDevice.HONEYWELL_VUQUEST3310G:
                    ret = (Vuquest3310g) new Vuquest3310g.Builder(s.Com, s.Bandrate).DataBits(s.DataBit)
                        .Parity(s.Parity).StopBits(s.StopBit).Build();
                    break;
                case DyNetDevice.LSIS_XGT:
                    switch (comm_type)
                    {
                        case DyNetCommType.ETHERNET:
                            ret = new XGTFEnetSocket(e.Host, (XGTFEnetPort)e.Port);
                            break;
                        case DyNetCommType.SERIAL:
                            ret = (IConnect)new XGTCnetSocket.Builder(s.Com, s.Bandrate).DataBits(s.DataBit)
                        .Parity(s.Parity).StopBits(s.StopBit).Build();
                            break;
                    }
                    break;
            }
            return ret;
        }
    }
}