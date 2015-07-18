using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DY.NET.DATALOGIC.MATRIX;
using DY.NET.HONEYWELL.VUQUEST;
using DY.NET.LSIS.XGT;
using DY.NET;

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// 디바이스 별 통신 방법 정의
    /// </summary>
    public static class ServiceableDevice
    {
        public static readonly Dictionary<CommDevice, CommType> Dic = new Dictionary<CommDevice, CommType>
        {
            { CommDevice.LSIS_XGT,                  CommType.SERIAL | CommType.ETHERNET },
            { CommDevice.HONEYWELL_VUQUEST3310G,    CommType.SERIAL },
            { CommDevice.DATALOGIC_MATRIX200,       CommType.SERIAL },
        };

        public static IConnect CreateClient(CommDevice device, CommType type, object commOption)
        {
            IConnect ret = null;
            CommSerialParameter s = commOption as CommSerialParameter;
            CommEthernetParameter e = commOption as CommEthernetParameter;
            if (type == CommType.SERIAL && s == null)
                throw new ArgumentException("Type, commOption mismatch error");
            if (type == CommType.ETHERNET && e == null)
                throw new ArgumentException("Type, commOption mismatch error");

            switch (device)
            {
                case CommDevice.DATALOGIC_MATRIX200:
                    ret = new Matrix200.Builder(s.Com, s.Bandrate).DataBits(s.DataBit)
                        .Parity(s.Parity).StopBits(s.StopBit).Build();
                    break;
                case CommDevice.HONEYWELL_VUQUEST3310G:
                    ret = new Vuquest3310g.Builder(s.Com, s.Bandrate).DataBits(s.DataBit)
                        .Parity(s.Parity).StopBits(s.StopBit).Build();
                    break;
                case CommDevice.LSIS_XGT:
                    switch (type)
                    {
                        case CommType.ETHERNET:
                            ret = new XGTFEnetSocket(e.Host, e.Port);
                            break;
                        case CommType.SERIAL:
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