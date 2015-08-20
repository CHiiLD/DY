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

namespace DY.WPF
{
    /// <summary>
    /// NumericUpDownWithBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NumericUpDownWithBar : UserControl
    {
        public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(NumericUpDownWithBar),
            new PropertyMetadata("Sample text"));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double?),
            typeof(NumericUpDownWithBar),
            new PropertyMetadata(null));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum",
            typeof(double),
            typeof(NumericUpDownWithBar),
            new PropertyMetadata(double.MinValue));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum",
            typeof(double),
            typeof(NumericUpDownWithBar),
            new PropertyMetadata(double.MaxValue));

        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register(
            "StringFormat",
            typeof(string),
            typeof(NumericUpDownWithBar),
            new PropertyMetadata());

        public string Title
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double? Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        public NumericUpDownWithBar()
        {
            InitializeComponent();
        }
    }
}
