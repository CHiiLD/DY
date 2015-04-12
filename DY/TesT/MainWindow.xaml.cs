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
using De.TorstenMandelkow.MetroChart;

namespace TesT
{
    

    public partial class MainWindow : Window
    {
        public TestPageViewModel Model
        {
            get;
            set;
        }

        public MainWindow()
		{
            this.InitializeComponent();

            myChart.Series.Clear();
            ChartSeries series = new ChartSeries();
            series.SeriesTitle = "Errors";
            series.DisplayMember = "Category";
            series.ValueMember = "Number";
            // Important: if you want the graph to update when adding, removing or chaning series, set ItemsSource to null first (this will force it to update)
            series.ItemsSource = null;

            myChart.Series.Add(series);
            var Errors = new ObservableCollection<TestClass>();
            Errors.Add(new TestClass() { Category = "Globalization", Number = 75 });
            Errors.Add(new TestClass() { Category = "Features", Number = 2 });
            Errors.Add(new TestClass() { Category = "ContentTypes", Number = 12 });
            Errors.Add(new TestClass() { Category = "Correctness", Number = 83 });
            Errors.Add(new TestClass() { Category = "Best Practices", Number = 29 });

            series.ItemsSource = Errors;
		}
    }
}
