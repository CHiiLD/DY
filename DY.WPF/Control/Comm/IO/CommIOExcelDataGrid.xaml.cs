using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using PropertyTools.Wpf;

namespace DY.WPF
{
    /// <summary>
    /// IOEditMode.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommIOExcelDataGrid : UserControl
    {
        public ObservableCollection<CommIODataGridItem> Items { get; private set; }

        public CommIOExcelDataGrid()
        {
            Items = new ObservableCollection<CommIODataGridItem>();
            InitializeComponent();
            NDataGrid.ItemsSource = Items;
            AddColumns();
            //여기는 원래 xaml에서 편집해야 맞는 거지만, 에러 아닌 에러가 자꾸 떠서 스트레스로 
            //cs에서 수동으로 생성해서 설정 
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
            item.Width = new GridLength(70, GridUnitType.Pixel);
            columns.Add(item);

            item = new PropertyTools.Wpf.ColumnDefinition();
            item.Header = "Read";
            item.PropertyName = "Read";
            item.Width = new GridLength(100, GridUnitType.Pixel);
            columns.Add(item);

            item = new PropertyTools.Wpf.ColumnDefinition();
            item.Header = "Write";
            item.PropertyName = "Write";
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