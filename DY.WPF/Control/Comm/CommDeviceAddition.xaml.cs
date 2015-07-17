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

using DY.WPF.SYSTEM;

namespace DY.WPF
{
    /// <summary>
    /// CommDeviceAddition.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommDeviceAddition : UserControl
    {
        public CommDeviceAddition()
        {
            this.InitializeComponent();
            NDevice.ItemSource = Enum.GetValues(typeof(CommDevice));
            NDevice.NComboBox.SelectionChanged += OnSelectionChanged_Device;
            NType.NComboBox.SelectionChanged += OnSelectionChanged_Type;
        }

        private void OnSelectionChanged_Device(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            CommDevice item = (CommDevice)cb.SelectedItem;
            List<CommType> source = new List<CommType>();
            NType.SelectedItem = null;
            NGrid.Children.Clear();
            if (ServiceableDevice.Dic.ContainsKey(item))
            {
                CommType type = ServiceableDevice.Dic[item];
                CommType[] types = (CommType[])Enum.GetValues(typeof(CommType));
                foreach (var t in types)
                    if ((t & type) != 0)
                        source.Add(t);
                NType.ItemSource = source;
            }
        }

        private void OnSelectionChanged_Type(object sender, SelectionChangedEventArgs e)
        {
            NGrid.Children.Clear();
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;
            CommType item = (CommType)cb.SelectedItem;
            switch (item)
            {
                case CommType.ETHERNET:
                    NGrid.Children.Add(new CommEthernetConfig());
                    break;
                case CommType.SERIAL:
                    NGrid.Children.Add(new CommSerialConfig());
                    break;
            }
        }
    }
}