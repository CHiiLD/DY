using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DY.WPF.SYSTEM
{
    public class NotifyPropertyChanged<T> : INotifyPropertyChanged
    {
        private T m_Source;
        public event PropertyChangedEventHandler PropertyChanged;
        public NotifyPropertyChanged() { }

        public NotifyPropertyChanged(T value)
        {
            this.m_Source = value;
        }

        public T Source
        {
            get { return m_Source; }
            set
            {
                m_Source = value;
                OnPropertyChanged("Source");
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}