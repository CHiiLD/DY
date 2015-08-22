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
using OxyPlot.Series;
using NLog;

namespace DY.WPF
{
    /// <summary>
    /// COxyRectangleGraphType01.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class COxyRectangleGraphType01 : UserControl, ICOxyPlotRange, ICOxyRectangleRange
    {
        //Plot range
        public double PlotMaximumX 
        { 
            get; 
            set; 
        }

        public double PlotMaximumY 
        { 
            get; 
            set; 
        }

        public double PlotMinimumX 
        { 
            get; 
            set; 
        }

        public double PlotMinimumY 
        {
            get; 
            set; 
        }

        //Rectangle range
        public double RectMinimunX 
        { 
            get; 
            set; 
        }

        public double RectMinimunY 
        { 
            get; 
            set; 
        }

        public double RectMaximunX 
        { 
            get; 
            set; 
        }

        public double RectMaximunY 
        { 
            get; 
            set; 
        }

        public COxyBottonAxesType BottonAxesType
        {
            get;
            set;
        }

        public COxyRectangleGraphType01()
        {
            InitializeComponent();
        }
    }
}