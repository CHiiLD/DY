using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DY.WPF
{
    public class CommIODataGridItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private PLCVariableType m_Type;
        private string m_Address;
        private object m_Input;
        private object m_Output;
        private string m_Comment;

        public PLCVariableType Type { get { return m_Type; } set { m_Type = value; OnPropertyChanged("Type"); } }
        public string Address { get { return m_Address; } set { m_Address = value; OnPropertyChanged("Address"); } }
        public object Input { get { return m_Input; } set { m_Input = value; OnPropertyChanged("Input"); } }
        public object Output { get { return m_Output; } set { m_Output = value; OnPropertyChanged("Output"); } }
        public string Comment { get { return m_Comment; } set { m_Comment = value; OnPropertyChanged("Comment"); } }
    }
}