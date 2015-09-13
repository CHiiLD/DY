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
using DY.NET;

using OxyPlot;
using OxyPlot.Wpf;

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

            NGraph.BottonAxisType = COxyBottonAxisType.TIMESPAN_AXIS;

            NGraph.PlotMinimumX = 0;
            NGraph.PlotMaximumX = 60;
            NGraph.PlotMinimumY = 25;
            NGraph.PlotMaximumY = 75;

            NGraph.RectMinimunX = 10;
            NGraph.RectMaximunX = 50;
            NGraph.RectMinimunY = 40;
            NGraph.RectMaximunY = 60;

            LineSeries series1 = NGraph.CreateLineSeries();
            series1.Color = Colors.Gold;
            var items1 = series1.ItemsSource as Collection<COxyTimeValue>;

            LineSeries series2 = NGraph.CreateLineSeries();
            series2.Color = Colors.Red;
            var items2 = series2.ItemsSource as Collection<COxyTimeValue>;

            const int COUNT = 50;
            var random = new Random();

            for(int i = 0; i <= COUNT; i++)
            {
                long tick = (long) (((double)((NGraph.PlotMaximumX - NGraph.PlotMinimumX) * 1000 * 10000)) * ((double)i / (double)COUNT));
                double value = random.NextDouble() * (NGraph.PlotMaximumY - NGraph.PlotMinimumY) + NGraph.PlotMinimumY;
                items1.Add(new COxyTimeValue(new TimeSpan(tick), value));
                value = random.NextDouble() * (NGraph.PlotMaximumY - NGraph.PlotMinimumY) + NGraph.PlotMinimumY;
                items2.Add(new COxyTimeValue(new TimeSpan(tick), value));
            }
            NGraph.AddSeries(series1);
            NGraph.AddSeries(series2);
        }
    }
}