using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

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
            foreach (var io in CommIOData)
            {
                io.PropertyChanged -= OnInputPropertyChanged;
                io.PropertyChanged += OnInputPropertyChanged;
            }
            Protocols.Clear();
            Protocols.AddRange(XGTProtocolHelper.Manufacture(CommIOData, CreateRssProtocol));
        }

        /// <summary>
        /// IO Update by async
        /// </summary>
        /// <returns></returns>
        public override async Task UpdateIOAsync(CancellationTokenSource cts)
        {
            ASocketCover mailBox = CClient.Socket as ASocketCover;
            foreach (IProtocol request in Protocols)
            {
                if (cts.IsCancellationRequested)
                    break;
                //post
                AProtocol response = await Post(mailBox, (AProtocol)request);
                if (response == null)
                    continue;
                //find error
                if (HasError(response))
                    continue;
                //update excel only read
                XGTProtocolHelper.Fill(response.DrawTickets(), CommIOData);
            }
        }

        /// <summary>
        /// XGT Protocol을 생성한다
        /// </summary>
        /// <param name="type">데이터 타입</param>
        /// <param name="datas">READ 목록</param>
        /// <returns></returns>
        private AProtocol CreateRssProtocol(DataType type, Dictionary<string, object> datas)
        {
            AProtocol protocol = null;
            if (CClient.Socket is XGTCnetSocket)
            {
                XGTCnetSocket cnet_socket = CClient.Socket as XGTCnetSocket;
                protocol = XGTCnetProtocol.NewRSSProtocol(type.ToType(), cnet_socket.LocalPort, datas);
            }
            else if (CClient.Socket is XGTFEnetSocket)
            {
                protocol = XGTFEnetProtocol.NewRSSProtocol(type.ToType(), InvokeID, datas);
            }
            return protocol;
        }

        private async Task<AProtocol> Post(ASocketCover mailBox, AProtocol request)
        {
            AProtocol response = null;
            Delivery delivery = null;
            try
            {
                DateTime date = DateTime.Now;
                delivery = await mailBox.PostAsync(request);
                response = delivery.Package as AProtocol;
                if (delivery.Error == DeliveryError.DISCONNECT)
                {
                    CClient.ChangedCommStatus(false);
                    throw new Exception(delivery.Error.ToString());
                }
                else if (delivery.Error == DeliveryError.WRITE_TIMEOUT || delivery.Error == DeliveryError.READ_TIMEOUT)
                {
                    throw new Exception(delivery.Error.ToString());
                }
            }
            catch (Exception exception)
            {
                LOG.Trace(CClient.Summary + " UpdateIOAsync 예외처리 메세지는 이하와 같음: " + exception.Message + "\n" + exception.StackTrace);
                request.Print();
                if (response != null)
                    response.Print();
                response = null;
            }
            if (DeliveryArrived != null && delivery != null)
                DeliveryArrived(this, new DeliveryArrivalEventArgs(delivery));
            return response;
        }

        private bool HasError(AProtocol response)
        {
            string error_msg = null;
            switch (CClient.CommType)
            {
                case CommunicationType.SERIAL:
                    var cnet = response as XGTCnetProtocol;
                    error_msg = cnet.Error == XGTCnetProtocolError.OK ? null : cnet.Error.ToString();
                    break;
                case CommunicationType.ETHERNET:
                    var fenet = response as XGTFEnetProtocol;
                    error_msg = fenet.Error == XGTFEnetProtocolError.OK ? null : fenet.Error.ToString();
                    break;
            }
            if (!String.IsNullOrEmpty(error_msg))
            {
                LOG.Debug(CClient.Summary + " 프로토콜 에러 발생: " + error_msg);
                response.MirrorProtocol.Print();
                response.Print();
                return true;
            }
            return false;
        }

        protected override async void OnInputPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            ICommIOData io_data = sender as ICommIOData;
            if (io_data.Input == null || args.PropertyName != "Input" || !Activated)
                return;
            object value = io_data.Type.ToValue(io_data.Input as string);
            if (value == null)
                return;
            string glopa = XGTProtocolHelper.ToGlopa(io_data.Type, io_data.Address);
            Dictionary<string, object> tickets = new Dictionary<string, object>() { { glopa, value } };
            AProtocol request = null;
            AProtocol response = null;
            if (CClient.Socket is XGTCnetSocket)
            {
                XGTCnetSocket cnet_socket = CClient.Socket as XGTCnetSocket;
                request = XGTCnetProtocol.NewWSSProtocol(io_data.Type.ToType(), cnet_socket.LocalPort, tickets);
                response = await Post(CClient.Socket as ASocketCover, request) as XGTCnetProtocol;
            }
            else if (CClient.Socket is XGTFEnetSocket)
            {
                request = XGTFEnetProtocol.NewWSSProtocol(io_data.Type.ToType(), InvokeID, tickets);
                response = await Post(CClient.Socket as ASocketCover, request) as XGTFEnetProtocol;
            }
            if (HasError(response))
                return;
            io_data.Input = null;
        }
    }
}