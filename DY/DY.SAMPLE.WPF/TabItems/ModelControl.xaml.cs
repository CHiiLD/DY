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
using De.TorstenMandelkow.MetroChart;

namespace DY.SAMPLE.WPF
{
    public class OperationForceInspection
    {
        public string Category { get; set; }
        public int Value { get; set; }
    }

    public partial class ModelControl : UserControl
    {
        public ModelControl()
        {
            this.InitializeComponent();

            //LOG ITEMS INIT
            List<Log> LogList = new List<Log>();
            LogList.Add(new Log() { Time = "12:18:54:862", Context = "Connection Port 8080" });
            LogList.Add(new Log() { Time = "12:18:54:898", Context = "start port execute(192.168.23.61:8081)" });
            LogList.Add(new Log() { Time = "12:18:54:912", Context = "192.168.23.61 try connect(8081)..." });
            LogList.Add(new Log() { Time = "12:18:54:952", Context = "Connection Port 8081" });
            LogList.Add(new Log() { Time = "12:18:54:967", Context = "192.168.23.61 try connect(8081)..." });
            LogList.Add(new Log() { Time = "12:18:54:987", Context = "Connection Port 8082" });
            LogList.Add(new Log() { Time = "12:18:55:862", Context = "Connection Port 8080" });
            LogList.Add(new Log() { Time = "12:18:55:898", Context = "start port execute(192.168.23.61:8081)" });
            LogList.Add(new Log() { Time = "12:18:55:912", Context = "192.168.23.61 try connect(8081)..." });
            LogList.Add(new Log() { Time = "12:18:55:952", Context = "Connection Port 8081" });
            LogList.Add(new Log() { Time = "12:18:55:967", Context = "192.168.23.61 try connect(8081)..." });
            LogList.Add(new Log() { Time = "12:18:55:987", Context = "Connection Port 8082" });
            
            NLogListView.ItemsSource = LogList;

            //CHART DATA BINDING 
            NOFIChart.Series.Clear();

            var chart_item_srcs = new ObservableCollection<OperationForceInspection>();
            chart_item_srcs.Add(new OperationForceInspection() { Category = "Globalization", Value = 75 });
            chart_item_srcs.Add(new OperationForceInspection() { Category = "Features", Value = 2 });
            chart_item_srcs.Add(new OperationForceInspection() { Category = "ContentTypes", Value = 12 });
            chart_item_srcs.Add(new OperationForceInspection() { Category = "Correctness", Value = 83 });
            chart_item_srcs.Add(new OperationForceInspection() { Category = "Best Practices", Value = 29 });

            ChartSeries series = new ChartSeries();
            series.SeriesTitle = "ChartDatas";
            series.DisplayMember = "Category";
            series.ValueMember = "Value";
            
            series.ItemsSource = null;
            series.ItemsSource = chart_item_srcs;
            NOFIChart.Series.Add(series);
        }
    }
}