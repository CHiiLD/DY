using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace DY.WPF
{
    /// <summary>
    /// TextBoxWithBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TextBoxWithBar : UserControl, IGetContext
    {
        public bool m_IsOnlyNumber;

        public new int Tag { get; set; }
        public string Description { get; set; }
        public object UserData { get; set; }

        public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(TextBoxWithBar),
            new PropertyMetadata("Sample text"));

        public static readonly DependencyProperty TextBoxProperty = DependencyProperty.Register(
           "Text",
           typeof(string),
           typeof(TextBoxWithBar),
           new PropertyMetadata(""));

        public string Title
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextBoxProperty); }
            set { SetValue(TextBoxProperty, value); }
        }
        
        public bool IsNumberOnly
        {
            get
            {
                return m_IsOnlyNumber;
            }
            set
            {
                if (value)
                    NTextBox.PreviewTextInput += PreviewTextInputHandler;
                else
                    NTextBox.PreviewTextInput -= PreviewTextInputHandler;
                m_IsOnlyNumber = value;
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public TextBoxWithBar()
        {
            this.InitializeComponent();
        }

        public object GetContext()
        {
            return NTextBox.Text;
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void PreviewTextInputHandler(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

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