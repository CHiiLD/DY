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

namespace DY.SAMPLE.WPF.CTRL
{
    /// <summary>
    /// TextBoxControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TextBlockCustomControl : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text", typeof(string), typeof(TextBlockCustomControl), new PropertyMetadata(""));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                "CornerRadius", typeof(CornerRadius), typeof(TextBlockCustomControl), new PropertyMetadata(new CornerRadius(0, 0, 0, 0)));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        //public static readonly DependencyProperty TextSizeProperty =
        //   DependencyProperty.Register(
        //       "TextSize", typeof(int), typeof(TextBoxControl), new PropertyMetadata(22));

        //public int TextSize
        //{
        //    get { return (int)GetValue(TextSizeProperty); }
        //    set { SetValue(TextSizeProperty, value); }
        //}

        public TextBlockCustomControl()
        {
            this.InitializeComponent();

        }
    }
}