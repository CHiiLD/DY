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

namespace DY.WPF
{
    /// <summary>
    /// TitleForBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TitleForBar : UserControl
    {
        public static readonly DependencyProperty TitleTextProperty =
                    DependencyProperty.Register("TitleText", typeof(string), typeof(TitleForBar), new PropertyMetadata("Main Text"));
        public string TitleText
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        public static readonly DependencyProperty SubTextProperty =
                    DependencyProperty.Register("SubText", typeof(string), typeof(TitleForBar), new PropertyMetadata("please write sub text string"));
        public string SubText
        {
            get { return (string)GetValue(SubTextProperty); }
            set { SetValue(SubTextProperty, value); }
        }

        public TitleForBar()
        {
            InitializeComponent();
        }
    }
}