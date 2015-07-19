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
using DY.WPF.SYSTEM;

using DY.WPF.SYSTEM.COMM;
using DY.WPF.WINDOW;
using DY.NET;

namespace DY.WPF.SAMPLE
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class MainWindow : MetroWindow
    {


        public IEnumerable CommItem { get; private set; }

        NotifyPropertyChanged<bool> booleanProperty = new NotifyPropertyChanged<bool>(true);
        public bool IsChecked1
        {
            set
            {
                booleanProperty.Source = value;
            }
            get
            {
                return booleanProperty.Source;
            }
        }

        public bool IsChecked2
        {
            set
            {
                booleanProperty.Source = value;
            }
            get
            {
                return booleanProperty.Source;
            }
        }

        public MainWindow()
        {
            NLogConfig.Load();

            //var commList = new ObservableCollection<CommDataGridItem>
            //{
            //    new CommDataGridItem 
            //    {
            //        Image = CommStateAi.Connected,
            //        Target = CommDevice.DATALOGIC_MATRIX200,
            //        Type= CommType.ETHERNET
            //    },

            //    new CommDataGridItem 
            //    {
            //        Image = CommStateAi.ConnectFailure,
            //        Target = CommDevice.DATALOGIC_MATRIX200,
            //        Type= CommType.ETHERNET
            //    },

            //    new CommDataGridItem 
            //    {
            //        Image = CommStateAi.Idle,
            //        Target = CommDevice.DATALOGIC_MATRIX200,
            //        Type= CommType.ETHERNET
            //    },
            //};
            //CommItem = commList;
            InitializeComponent();
        }

        private void NB2_Click(object sender, RoutedEventArgs e)
        {
            //if (NB2.IsChecked == true)
            //{
            //    booleanProperty.Source = true;
            //}
            //else
            //{
            //    booleanProperty.Source = false;
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommDeviceSetWindow win = new CommDeviceSetWindow() { Owner = this };
            win.Show();
        }
    }
}