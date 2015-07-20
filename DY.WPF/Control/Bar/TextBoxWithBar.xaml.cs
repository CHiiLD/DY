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
using System.Text.RegularExpressions;

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

        public bool m_IsOnlyNumber;
        public bool IsNumberOnly
        {
            get
            {
                return m_IsOnlyNumber;
            }
            set
            {
                if (value)
                {
                    NTextBox.PreviewTextInput += PreviewTextInputHandler;
                }
                else
                {
                    NTextBox.PreviewTextInput -= PreviewTextInputHandler;
                }
                m_IsOnlyNumber = value;
            }
        }

        public TextBoxWithBar()
        {
            this.InitializeComponent();
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        // Use the PreviewTextInputHandler to respond to key presses 
        private void PreviewTextInputHandler(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        // Use the DataObject.Pasting Handler  
        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }
    }
}