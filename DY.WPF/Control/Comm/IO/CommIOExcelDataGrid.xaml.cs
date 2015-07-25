using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using PropertyTools.Wpf;
using DY.WPF.SYSTEM.COMM;
using DY.NET;
using DY.NET.LSIS.XGT;
using NLog;

namespace DY.WPF
{
    /// <summary>
    /// IOEditMode.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommIOExcelDataGrid : UserControl
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public ObservableCollection<CommIODataGridItem> Items { get; private set; }
        public CommClient Client { get; set; }
        public int DelayTime { get; set; }
        public int ResponseRatencyTime { get; set; }
        private List<IProtocol> m_RequestProtocols = null;

        public bool m_IsEditMode;
        /// <summary>
        /// 편집 모드 유무
        /// </summary>
        public bool Editable
        {
            get
            {
                return m_IsEditMode;
            }
            set
            {
                NCO_Type.IsReadOnly = !value;
                NCO_Address.IsReadOnly = !value;
                NCO_Comment.IsReadOnly = !value;
                NDataGrid.CanInsert = value; //셀 추가 가능 여부 설정
                m_IsEditMode = value;
            }
        }

        /// <summary>
        /// IO 쓰기/읽기 작동중 여부
        /// </summary>
        public bool Operable
        {
            get;
            set;
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public CommIOExcelDataGrid()
        {
            Items = new ObservableCollection<CommIODataGridItem>();
            InitializeComponent();
            NDataGrid.ItemsSource = Items;
            Editable = false;
            //AddColumns();
            //여기는 원래 xaml에서 편집해야 맞는 거지만, 에러 아닌 에러가 자꾸 떠서 스트레스로 
            //cs에서 수동으로 생성해서 설정 
        }
         
        /// <summary>
        /// IO Update by async
        /// </summary>
        /// <returns></returns>
        private async Task UpdateIO()
        {
            if (m_RequestProtocols == null || m_RequestProtocols.Count == 0)
                return;
            if (!Client.Socket.IsConnected())
                return;
            IPostAsync post = Client.Socket as IPostAsync;
            if (post == null)
            {
                LOG.Debug("클라이언트 소켓이 IPostAsync을 상속하지 않은 객체입니다.");
                return;
            }

            foreach (var reqt in m_RequestProtocols)
            {
                IProtocol resp = await post.PostAsync(reqt);
                if (resp == null)
                    continue;
                Dictionary<string, object> storage = resp.GetStorage();
                if (storage == null || storage.Count == 0)
                    continue;
                IList<ICommIOData> items = Items as IList<ICommIOData>;
                switch (Client.Target)
                {
                    case DYDevice.LSIS_XGT:
                        XGTProtocolHelper.Fill(storage, items);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// ObservableCollection<CommIODataGridItem> 정보로 프로토콜들을 생성한다
        /// </summary>
        /// <returns></returns>
        private List<IProtocol> CreateProtocols()
        {
            IList<ICommIOData> items = Items as IList<ICommIOData>;
            Dictionary<string, DataType> addrs = XGTProtocolHelper.Optimize(items);
            ILookup<DataType, string> lookCollection = addrs.ToLookup(ad => ad.Value, ad => ad.Key);
            int cnt = 0;
            Dictionary<string, object> datas = new Dictionary<string, object>();
            List<IProtocol> protocols = new List<IProtocol>();

            switch (Client.Target)
            {
                case DYDevice.LSIS_XGT:
                    foreach (IGrouping<DataType, string> group in lookCollection)
                    {
                        foreach (string str in group)
                        {
                            if (cnt % 16 == 0 && cnt != 0)
                            {
                                protocols.Add(CreateXGTProtocol(group.Key, datas));
                                cnt = 0;
                                datas = new Dictionary<string, object>();
                            }
                            datas.Add(str, null);
                            cnt++;
                        }
                        protocols.Add(CreateXGTProtocol(group.Key, datas));
                        cnt = 0;
                        datas = new Dictionary<string, object>();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return protocols;
        }

        /// <summary>
        /// XGT Protocol을 생성한다
        /// </summary>
        /// <param name="type">데이터 타입</param>
        /// <param name="datas">READ 목록</param>
        /// <returns></returns>
        private IProtocol CreateXGTProtocol(DataType type, Dictionary<string, object> datas)
        {
            IProtocol protocol;
            switch (Client.CommType)
            {
                case DYDeviceProtocolType.SERIAL:
                    protocol = XGTCnetProtocol.NewRSSProtocol(type.ToType(), 00, datas);
                    break;
                case DYDeviceProtocolType.ETHERNET:
                    protocol = XGTFEnetProtocol.NewRSSProtocol(type.ToType(), 00, datas);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return protocol;
        }

        /// <summary>
        /// 컬럼 추가
        /// </summary>
        private void AddColumns()
        {
            Collection<PropertyDefinition> columns = NDataGrid.ColumnDefinitions;
            PropertyTools.Wpf.ColumnDefinition item = new PropertyTools.Wpf.ColumnDefinition();
            item.Header = "Type";
            item.PropertyName = "Type";
            item.Width = new GridLength(70, GridUnitType.Pixel);
            columns.Add(item);

            item = new PropertyTools.Wpf.ColumnDefinition();
            item.Header = "Address";
            item.PropertyName = "Address";
            item.Width = new GridLength(100, GridUnitType.Pixel);
            columns.Add(item);

            item = new PropertyTools.Wpf.ColumnDefinition();
            item.Header = "Read";
            item.PropertyName = "Output";
            item.Width = new GridLength(100, GridUnitType.Pixel);
            columns.Add(item);

            item = new PropertyTools.Wpf.ColumnDefinition();
            item.Header = "Write";
            item.PropertyName = "Input";
            item.Width = new GridLength(100, GridUnitType.Pixel);
            columns.Add(item);

            item = new PropertyTools.Wpf.ColumnDefinition();
            item.Header = "Comment";
            item.PropertyName = "Comment";
            item.Width = new GridLength(1, GridUnitType.Star);
            columns.Add(item);
        }
    }
}