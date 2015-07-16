using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace DY.WPF
{
	/// <summary>
	/// CommDataGrid.xaml에 대한 상호 작용 논리
	/// </summary>

	public partial class CommDataGrid : UserControl
	{
        public ObservableCollection<CommDataGridItem> DataGridItems
        {
            get;
            private set;
        }

		public CommDataGrid()
		{
            DataGridItems = new ObservableCollection<CommDataGridItem>();
            var item = new CommDataGridItem
            {
                Usable = true,
                Image = CommStateAi.Connected,
                Target = CommDevice.DATALOGIC_MATRIX200,
                Type = CommType.SERIAL,
                Option = "115200-8-N-1",
                Note = "데이타 로직 바코드"
            };
            DataGridItems.Add(item);
			this.InitializeComponent();
		}

        /// <summary>
        /// DataGrid 통신 디바이스 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {

        }

        private void NMI_AddCommDevice_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: 여기에 구현된 이벤트 처리기를 추가하십시오.
        }
	}
}
