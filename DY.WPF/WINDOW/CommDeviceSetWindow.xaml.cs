using System;
using System.Windows;

using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using DY.NET;
using DY.WPF.SYSTEM.COMM;
using System.Net.Sockets;

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
            MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            NButton.NCancel.Click += (object sender, RoutedEventArgs e) => { Close(); };
            NButton.NOk.Click += AddNewClient;
        }

        private async void AddNewClient(object sender, RoutedEventArgs e)
        {
            do
            {
                CommDeviceSelection device_add = NSetBox as CommDeviceSelection;
                if (device_add.NDevice.SelectedItem == null || device_add.NType.SelectedItem == null)
                {
                    await this.ShowMessageAsync("Error", "Please select Communication Device, Type");
                    break;
                }
                var comm_device = (DYDevice)device_add.NDevice.SelectedItem;
                var comm_type = (DYDeviceProtocolType)device_add.NType.SelectedItem;
                ISummaryParameter comm_option = null;

                var comm_config = device_add.NGrid.Children[0];
                if (comm_config is CommSerialConfig)
                {
                    var comm_serial = comm_config as CommSerialConfig;
                    CommSerialParameter comm_parameter = comm_serial.GetCommSerialStruct();
                    if (String.IsNullOrEmpty(comm_parameter.Com))
                    {
                        await this.ShowMessageAsync("Error", "Please select com port.");
                        break;
                    }
                    comm_option = comm_parameter;
                }
                else if (comm_config is CommEthernetConfig)
                {
                    var comm_ethernet = comm_config as CommEthernetConfig;
                    CommEthernetParameter comm_parameter = comm_ethernet.GetCommEthernetStruct();
                    if (String.IsNullOrEmpty(comm_parameter.Host))
                    {
                        await this.ShowMessageAsync("Error", "Please write host ip.");
                        break;
                    }
                    if (comm_parameter.Type != ProtocolType.Tcp)
                    {
                        await this.ShowMessageAsync("Error", "It current supported tcp protocol only.");
                        break;
                    }
                    comm_option = comm_parameter;
                }
                IConnect client = ServiceableDevice.CreateClient(comm_device, comm_type, comm_option);
                CommClient client_comm = new CommClient(client, comm_device, comm_type);
                client_comm.Summary = comm_option.GetParameterSummaryString();
                CommClientManagement.GetInstance().Clientele.Add(client_comm);
                Close();
            } while (false);
        }
    }
}