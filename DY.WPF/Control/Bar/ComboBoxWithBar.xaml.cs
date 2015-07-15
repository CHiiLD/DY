using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Collections;

namespace DY.WPF
{
    /// <summary>
    /// TextBoxWithBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ComboBoxWithBar : UserControl
    {
        public static readonly DependencyProperty TitleTextProperty =
                    DependencyProperty.Register("Title", typeof(object), typeof(ComboBoxWithBar), new PropertyMetadata("Sample text"));
        public object Title
        {
            get { return (object)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty =
              DependencyProperty.Register("ItemSource", typeof(IEnumerable), typeof(ComboBoxWithBar), new PropertyMetadata());
        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public ComboBoxWithBar()
        {
            this.InitializeComponent();
        }
    }
}