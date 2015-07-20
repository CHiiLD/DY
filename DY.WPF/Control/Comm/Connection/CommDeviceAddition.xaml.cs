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

using DY.WPF.SYSTEM.COMM;

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
            NDevice.ItemSource = Enum.GetValues(typeof(DYDevice));
            NDevice.NComboBox.SelectionChanged += OnSelectionChanged_Device;
            NType.NComboBox.SelectionChanged += OnSelectionChanged_Type;
        }

        private void OnSelectionChanged_Device(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            DYDevice item = (DYDevice)cb.SelectedItem;
            List<DYDeviceProtocolType> source = new List<DYDeviceProtocolType>();
            NType.SelectedItem = null;
            NGrid.Children.Clear();
            if (ServiceableDevice.Dic.ContainsKey(item))
            {
                DYDeviceProtocolType type = ServiceableDevice.Dic[item];
                DYDeviceProtocolType[] types = (DYDeviceProtocolType[])Enum.GetValues(typeof(DYDeviceProtocolType));
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
            DYDeviceProtocolType item = (DYDeviceProtocolType)cb.SelectedItem;
            switch (item)
            {
                case DYDeviceProtocolType.ETHERNET:
                    NGrid.Children.Add(new CommEthernetConfig());
                    break;
                case DYDeviceProtocolType.SERIAL:
                    NGrid.Children.Add(new CommSerialConfig());
                    break;
            }
        }
    }
}