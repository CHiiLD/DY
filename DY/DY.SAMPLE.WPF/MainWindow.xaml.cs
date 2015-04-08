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
using MahApps.Metro;
using MetroDemo;

namespace DY.SAMPLE.WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public readonly MainWindowViewModel _WinViewModel;

        public MainWindow()
        {
            _WinViewModel = new MainWindowViewModel(); //for data binding
            DataContext = _WinViewModel;

            InitializeComponent();
        }
    }
}
