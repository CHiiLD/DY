using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DY.NET;
using DY.NET.LSIS.XGT;
using NLog;

using DY.WPF.SYSTEM.COMM;

namespace DY.WPF.SYSTEM.COMM
{
    public class CommIOMonitoringXGT : ACommIOMonitoringStrategy
    {
        protected new static Logger LOG = LogManager.GetCurrentClassLogger();
        public const int InvokeID = 00;

        public CommIOMonitoringXGT(CommClient cclient)
            : base(cclient)
        {
        }

        /// <summary>
        /// ObservableCollection<CommIODataGridItem> 정보로 프로토콜들을 생성한다
        /// </summary>
        /// <returns></returns>
        public override void ReplaceICommIOData(IList<ICommIOData> io_datas)
        {
            CommIOData = io_datas;
            Dictionary<string, DataType> addrs = XGTProtocolHelper.Optimize(io_datas);
            ILookup<DataType, string> lookCollection = addrs.ToLookup(ad => ad.Value, ad => ad.Key);
            int cnt = 0;
            Protocols.Clear();
            Dictionary<string, object> datas = new Dictionary<string, object>();
            foreach (IGrouping<DataType, string> group in lookCollection)
            {
                foreach (string str in group)
                {
                    if (cnt % 16 == 0 && cnt != 0)
                    {
                        Protocols.Add(CreateProtocol(group.Key, datas));
                        cnt = 0;
                        datas = new Dictionary<string, object>();
                    }
                    datas.Add(str, null);
                    cnt++;
                }
                Protocols.Add(CreateProtocol(group.Key, datas));
                cnt = 0;
                datas = new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// XGT Protocol을 생성한다
        /// </summary>
        /// <param name="type">데이터 타입</param>
        /// <param name="datas">READ 목록</param>
        /// <returns></returns>
        private IProtocol CreateProtocol(DataType type, Dictionary<string, object> datas)
        {
            IProtocol protocol;
            switch (CClient.CommType)
            {
                case DYDeviceCommType.SERIAL:
                    ushort localPort = (ushort)(CClient.ExtraData[CommClient.EXTRA_XGT_CNET_LOCALPORT]);
                    protocol = XGTCnetProtocol.NewRSSProtocol(type.ToType(), localPort, datas);
                    break;
                case DYDeviceCommType.ETHERNET:
                    protocol = XGTFEnetProtocol.NewRSSProtocol(type.ToType(), InvokeID, datas);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return protocol;
        }

        /// <summary>
        /// IO Update by async
        /// </summary>
        /// <returns></returns>
        public override async Task UpdateIOAsync()
        {
            IProtocol response;
            IPostAsync mailBox;
            string error_msg;

            foreach (var request in Protocols)
            {
                //init
                error_msg = null;
                response = null;
                mailBox = CClient.Socket as IPostAsync;
                //post
                try
                {
                    Delivery delivery = await mailBox.PostAsync(request);
                    DeliveryError delivery_err = delivery.Error;
                    switch (delivery_err)
                    {
                        case DeliveryError.SUCCESS:
                            response = delivery.Package as IProtocol;
                            break;
                        case DeliveryError.DISCONNECT:
                            CClient.ChangedCommStatus(false);
                            throw new Exception(delivery_err.ToString());
                        case DeliveryError.WRITE_TIMEOUT:
                        case DeliveryError.READ_TIMEOUT:
                            throw new Exception(delivery_err.ToString());
                    }
                }
                catch (Exception exception)
                {
                    LOG.Debug(CClient.Summary + " UpdateIOAsync 예외처리 메세지는 이하와 같음: " + exception.Message
                        + exception.StackTrace);
                    continue;
                }
                //find error
                switch (CClient.CommType)
                {
                    case DYDeviceCommType.SERIAL:
                        var cnet = response as XGTCnetProtocol;
                        error_msg = cnet.Error == XGTCnetProtocolError.OK ? null : cnet.Error.ToString();
                        break;
                    case DYDeviceCommType.ETHERNET:
                        var fenet = response as XGTFEnetProtocol;
                        error_msg = fenet.Error == XGTFEnetProtocolError.OK ? null : fenet.Error.ToString();
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (!String.IsNullOrEmpty(error_msg))
                {
                    LOG.Debug(CClient.Summary + " 프로토콜 에러 발생: " + error_msg);
                    continue;
                }
                //update excel
                Dictionary<string, object> storage = response.GetStorage();
                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    XGTProtocolHelper.Fill(storage, CommIOData);
                }), null);
            }
        }
    }
}