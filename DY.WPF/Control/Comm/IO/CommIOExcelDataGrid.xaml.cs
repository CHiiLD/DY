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

namespace DY.WPF
{
    /// <summary>
    /// IOEditMode.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommIOExcelDataGrid : UserControl
    {
        public ObservableCollection<CommIODataGridItem> Items { get; private set; }
        public CommClient Client { get; set; }
        public int DelayTime { get; set; }
        public int ResponseRatencyTime { get; set; }

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
        private async Task Update()
        {
            await Task.Delay(DelayTime);
            IList<ICommIOData> list = Items as IList<ICommIOData>;
            Dictionary<string, DataType> addrs = XGTProtocolHelper.Optimize(list);
            ILookup<DataType, string> lookCollection = addrs.ToLookup(ad => ad.Value, ad => ad.Key);
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