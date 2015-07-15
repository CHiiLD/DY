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
using System.Collections.ObjectModel;

using MahApps.Metro;
using MahApps.Metro.Controls;
using DY.WPF;
using System.Collections;
using System.ComponentModel;

namespace DY.WPF.SAMPLE
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public IEnumerable CommItem { get; private set; }

        public MainWindow()
        {
            var commList = new ObservableCollection<CommDataGridItem>
            {
                new CommDataGridItem 
                {
                    Image = CommStateAi.Connected,
                    Target = CommDevice.DATALOGIC_MATRIX200,
                    Type= CommType.ETHERNET
                },

                new CommDataGridItem 
                {
                    Image = CommStateAi.ConnectFailure,
                    Target = CommDevice.DATALOGIC_MATRIX200,
                    Type= CommType.ETHERNET
                },

                new CommDataGridItem 
                {
                    Image = CommStateAi.Idle,
                    Target = CommDevice.DATALOGIC_MATRIX200,
                    Type= CommType.ETHERNET
                },
            };
            CommItem = commList;
            InitializeComponent();
        }
    }
}
