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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using DY.WPF.SYSTEM.COMM;

namespace DY.WPF
{
    /// <summary>
    /// CommIOMonitoring.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommIOMonitoring : UserControl, ICommMonitoring
    {
        public CommClient Client
        {
            get
            {
                return NDataGrid.Client;
            }
            set
            {
                NDataGrid.Client = value;
            }
        }

        public CommIOMonitoring()
        {
            InitializeComponent();

            PlotModel plot_model = new PlotModel();
            plot_model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 100 });
            plot_model.Series.Add(new OxyPlot.Series.LineSeries { LineStyle = LineStyle.Solid });
            NPlotView.Model = plot_model;

            NBT_ExcelEditMode.IsCheckedChanged += OnCheckedChangedEditMode;
            NBT_ResponseIntevalGraph.IsCheckedChanged += OnCheckedChangedGraphActivation;
        }

        private void OnCheckedChangedEditMode(object sender, EventArgs e)
        {
            var btn = sender as ToggleSwitch;
            NDataGrid.IsEditMode = btn.IsChecked == true ? true : false;
        }

        private void OnCheckedChangedGraphActivation(object sender, EventArgs e)
        {
        }
    }
}
