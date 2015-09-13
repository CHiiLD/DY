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

using System.Net.Sockets;
using DY.WPF.SYSTEM.COMM;

namespace DY.WPF
{
    /// <summary>
    /// SerialCommConfig.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommEthernetConfig : UserControl
    {
        public CommEthernetConfig()
        {
            this.InitializeComponent();
            NType.ItemSource = Enum.GetValues(typeof(ProtocolType));
            NType.SelectedItem = ProtocolType.Tcp;
        }

        public CommEthernetElement GetCommEthernetElement()
        {
            ushort port = 0;
            UInt16.TryParse(NPort.Text, out port);
            CommEthernetElement ethernet_info = new CommEthernetElement(NIP.Text, port, (ProtocolType)NType.NComboBox.SelectedItem);
            return ethernet_info;
        }
    }
}