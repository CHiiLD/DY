using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using MahApps.Metro.Controls;
using DY.NET;
using DY.WPF.SYSTEM;

namespace DY.WPF.WINDOW
{
    /// <summary>
    /// CommunicationDeviceSettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommDeviceSetWindow : MetroWindow
    {
        public CommDeviceSetWindow()
        {
            InitializeComponent();
            NButton.NCancel.Click += (object sender, RoutedEventArgs e) =>
            {
                Close();
            };

            NButton.NOk.Click += AddNewClient;
        }

        private void AddNewClient(object sender, RoutedEventArgs e)
        {
            do
            {
                CommDeviceAddition device_add = sender as CommDeviceAddition;
                if (device_add.NDevice.SelectedItem == null || device_add.NType.SelectedItem == null)
                    break;
                
                var comm_device = (CommDevice)device_add.NDevice.SelectedItem;
                var comm_type = (CommType)device_add.NType.SelectedItem;
                ISummaryParameter comm_option = null;

                var comm_config = device_add.NGrid.Children[0];
                if (comm_config is CommSerialConfig)
                {
                    var comm_serial = comm_config as CommSerialConfig;
                    comm_option = comm_serial.GetCommSerialStruct();
                }
                else if (comm_config is CommEthernetConfig)
                {
                    var comm_ethernet = comm_config as CommEthernetConfig;
                    comm_option = comm_ethernet.GetCommEthernetStruct();
                }
                if (comm_option == null)
                    break;

                IConnect client = ServiceableDevice.CreateClient(comm_device, comm_type, comm_option);
                ClientComm client_comm = new ClientComm(client, comm_device, comm_type);
                client_comm.Summary = comm_option.GetParameterSummaryString();
                client_comm.Key = ClientCommManagement.GetInstance().SetClinet(client_comm);
            } while (false);
            Close();
        }
    }
}
