using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MahApps.Metro.Controls;
using System.Collections.ObjectModel;

namespace DY.WPF
{
    /// <summary>
    /// CommIODataGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommIODataGrid : UserControl
    {
        public ObservableCollection<CommIODataGridItem> Items
        {
            get;
            set;
        }

        public CommIODataGrid()
        {
            Items = new ObservableCollection<CommIODataGridItem>();
            Items.Add(new CommIODataGridItem() 
            { 
                Type = PLCVariableType.INT,
                Address="M1000",
                Input=1000,
                Output=1000,
                Comment="하라하라주구"
            });
            InitializeComponent();
        }
    }
}