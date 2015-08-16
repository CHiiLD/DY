using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

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
            Dictionary<string, object> read_storage = new Dictionary<string, object>();
            foreach (IGrouping<DataType, string> group in lookCollection)
            {
                foreach (string str in group)
                {
                    if (cnt % 16 == 0 && cnt != 0)
                    {
                        Protocols.Add(CreateReadProtocol(group.Key, read_storage));
                        cnt = 0;
                        read_storage = new Dictionary<string, object>();
                    }
                    read_storage.Add(str, null);
                    cnt++;
                }
                Protocols.Add(CreateReadProtocol(group.Key, read_storage));
                cnt = 0;
                read_storage = new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// XGT Protocol을 생성한다
        /// </summary>
        /// <param name="type">데이터 타입</param>
        /// <param name="datas">READ 목록</param>
        /// <returns></returns>
        private IProtocol CreateReadProtocol(DataType type, Dictionary<string, object> datas)
        {
            IProtocol protocol;
            switch (CClient.CommType)
            {
                case DyNetCommType.SERIAL:
                    ushort localPort = (ushort)(CClient.ExtraData[CommClient.EXTRA_XGT_CNET_LOCALPORT]);
                    protocol = XGTCnetProtocol.NewRSSProtocol(type.ToType(), localPort, datas);
                    break;
                case DyNetCommType.ETHERNET:
                    protocol = XGTFEnetProtocol.NewRSSProtocol(type.ToType(), InvokeID, datas);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return protocol;
        }

        private async Task<IProtocol> Post(IPostAsync mailBox, IProtocol request)
        {
            IProtocol response = null;
            Delivery delivery = null;
            try
            {
                delivery = await mailBox.PostAsync(request);
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
                LOG.Debug(CClient.Summary + " UpdateIOAsync 예외처리 메세지는 이하와 같음: "
                    + exception.Message
                    + exception.StackTrace);
                response = null;
            }
            if (delivery.Error != DeliveryError.DISCONNECT && DeliveryArrived != null)
                DeliveryArrived(this, new DeliveryArrivalEventArgs(delivery));
            return response;
        }

        private bool HasError(IProtocol response)
        {
            string error_msg = null;
            switch (CClient.CommType)
            {
                case DyNetCommType.SERIAL:
                    var cnet = response as XGTCnetProtocol;
                    error_msg = cnet.Error == XGTCnetProtocolError.OK ? null : cnet.Error.ToString();
                    break;
                case DyNetCommType.ETHERNET:
                    var fenet = response as XGTFEnetProtocol;
                    error_msg = fenet.Error == XGTFEnetProtocolError.OK ? null : fenet.Error.ToString();
                    break;
            }
            if (!String.IsNullOrEmpty(error_msg))
            {
                LOG.Debug(CClient.Summary + " 프로토콜 에러 발생: " + error_msg);
                return true;
            }
            return false;
        }

        /// <summary>
        /// IO Update by async
        /// </summary>
        /// <returns></returns>
        public override async Task UpdateIOAsync(CancellationToken token)
        {
            IPostAsync mailBox = CClient.Socket as IPostAsync;
            foreach (var request in Protocols)
            {
                if (token.IsCancellationRequested)
                    break;
                //post
                IProtocol response = await Post(mailBox, request);
                if (response == null)
                    continue;
                //find error
                if (HasError(response))
                    continue;
                //update excel
                XGTProtocolHelper.Fill(response.GetStorage(), CommIOData);
            }
        }
    }
}