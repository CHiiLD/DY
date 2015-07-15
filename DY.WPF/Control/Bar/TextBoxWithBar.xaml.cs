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

namespace DY.WPF
{
	/// <summary>
	/// TextBoxWithBar.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class TextBoxWithBar : UserControl
	{
        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TextBoxWithBar), new PropertyMetadata("Sample text"));
        public string Title
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBoxWithBar), new PropertyMetadata("TextBox"));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
 
		public TextBoxWithBar()
		{
			this.InitializeComponent();
		}
	}
}