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
	/// TextBlockBar.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class TextBlockBar : UserControl
	{
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBlockBar), new PropertyMetadata("Sample text"));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

		public TextBlockBar()
		{
			this.InitializeComponent();
            Text = "Sample Text";
		}
	}
}