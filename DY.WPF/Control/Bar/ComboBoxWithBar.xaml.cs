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

using DY.NET;

namespace DY.WPF
{
    /// <summary>
    /// TextBoxWithBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ComboBoxWithBar : UserControl, IGetValue
    {
        public int Tag { get; set; }
        public string Description { get; set; }
        public object UserData { get; set; }

        public object GetValue()
        {
            return SelectedItem;
        }

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

        public static readonly DependencyProperty SelectedItemProperty =
              DependencyProperty.Register("SelectedItem", typeof(object), typeof(ComboBoxWithBar), new PropertyMetadata());
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public ComboBoxWithBar()
        {
            this.InitializeComponent();
        }
    }
}