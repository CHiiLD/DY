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
        public MainWindow()
        {
            NLogConfig.Load();
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new CommIOExcelWindow() { Owner = this }.ShowDialog();
        }
    }
}