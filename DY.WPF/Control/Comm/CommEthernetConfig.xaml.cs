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

        public CommEthernetParameter GetCommEthernetStruct()
        {
            CommEthernetParameter ecs = new CommEthernetParameter();
            ecs.Host = NIP.Text;
            int port;
            Int32.TryParse(NPort.Text, out port);
            ecs.Port = port;
            ecs.Type = (ProtocolType)NType.NComboBox.SelectedItem;
            return ecs;
        }
    }
}