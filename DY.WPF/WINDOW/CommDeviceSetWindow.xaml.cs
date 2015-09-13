using System;
using System.Windows;
using System.Collections.Generic;

using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using DY.NET;
using DY.WPF.SYSTEM.COMM;
using System.Net.Sockets;
using System.Windows.Controls;

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
            NSetBox.CommTypeComboBoxChanged += OnCommTypeComboBoxChanged;
        }

        private void OnCommTypeComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;
            CommunicationType type = (CommunicationType)cb.SelectedItem;
            NetDevice device = (NetDevice)NSetBox.NDevice.SelectedItem;

            switch (type)
            {
                case CommunicationType.ETHERNET:
                    break;
                case CommunicationType.SERIAL:
                    //국번 옵션 추가
                    if(NetDevice.LSIS_XGT == device)
                    {
                        TextBoxWithBar localbox = new TextBoxWithBar();
                        localbox.UserData = CommClient.EXTRA_XGT_CNET_LOCALPORT;
                        localbox.Title = "Local Port";
                        localbox.Text = "00";
                        NSetBox.NExtra.Children.Add(localbox);
                    }
                    break;
            }
        }

        /// <summary>
        /// 사용자가 설정한 값들로 클라이언트 소켓 객체 생성한 뒤, 매지니먼트에 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                NetDevice comm_device = (NetDevice)device_add.NDevice.SelectedItem;
                CommunicationType comm_type = (CommunicationType)device_add.NType.SelectedItem;
                ISummaryParameter comm_option = null;
                var comm_config = device_add.NGrid.Children[0];
                //시리얼
                if (comm_config is CommSerialConfig)
                {
                    var comm_serial = comm_config as CommSerialConfig;
                    CommSerialPortParameter comm_parameter = comm_serial.GetCommSerialStruct();
                    if (String.IsNullOrEmpty(comm_parameter.Com))
                    {
                        await this.ShowMessageAsync("Error", "Please select com port.");
                        break;
                    }
                    comm_option = comm_parameter;
                }
                //이더넷 
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
                //엑스트라 검사
                Dictionary<string, object> extra_data = device_add.ExtraData;
                if (comm_device == NetDevice.LSIS_XGT && comm_type == CommunicationType.SERIAL)
                {
                    if (extra_data.Count == 0)
                    {
                        await this.ShowMessageAsync("Error", "Local port text is empty.");
                        break;
                    }
                    ushort local;
                    if (!UInt16.TryParse(extra_data[CommClient.EXTRA_XGT_CNET_LOCALPORT] as string, out local))
                    {
                        await this.ShowMessageAsync("Error", "Invalid local port number.");
                        break;
                    }
                    extra_data[CommClient.EXTRA_XGT_CNET_LOCALPORT] = local;
                }
                IConnect client = ServiceableDevice.CreateClient(comm_device, comm_type, comm_option);
                CommClient client_comm = new CommClient(client, comm_device, comm_type);
                client_comm.Summary = comm_option.GetParameterSummaryString();
                client_comm.ExtraData = extra_data;
                CommClientDirector.GetInstance().Clientele.Add(client_comm);
                Close();
            } while (false);
        }
    }
}