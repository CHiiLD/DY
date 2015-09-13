using System;
using System.Windows;
using System.Collections.Generic;

using System.Net.Sockets;
using System.Windows.Controls;
using System.Threading.Tasks;

using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using DY.NET;
using DY.WPF.SYSTEM.COMM;


namespace DY.WPF
{
    /// <summary>
    /// CommunicationDeviceSettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommDeviceSetWindow : MetroWindow
    {
        private static string EXTRA_LOCALPORT = "LOCALPORT_0000";

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
                        localbox.UserData = EXTRA_LOCALPORT;
                        localbox.Title = "Local Port";
                        localbox.Text = "00";
                        NSetBox.NExtra.Children.Add(localbox);
                    }
                    break;
            }
        }

        private async Task<ISummary> GetCommElementData(UserControl comm_config)
        {
            ISummary element = null;
            do
            {
                //시리얼
                if (comm_config is CommSerialConfig)
                {
                    CommSerialConfig comm_serial = comm_config as CommSerialConfig;
                    CommSerialPortElement serialport_element = comm_serial.GetCommSerialPortElement();
                    if (String.IsNullOrEmpty(serialport_element.PortName))
                    {
                        await this.ShowMessageAsync("Error", "Please select com port.");
                        break;
                    }
                    element = serialport_element;
                }
                //이더넷 
                else if (comm_config is CommEthernetConfig)
                {
                    CommEthernetConfig comm_ethernet = comm_config as CommEthernetConfig;
                    CommEthernetElement ethernet_element = comm_ethernet.GetCommEthernetElement();
                    if (String.IsNullOrEmpty(ethernet_element.Host))
                    {
                        await this.ShowMessageAsync("Error", "Please write host ip.");
                        break;
                    }
                    if (ethernet_element.Type != ProtocolType.Tcp)
                    {
                        await this.ShowMessageAsync("Error", "It current supported tcp protocol only.");
                        break;
                    }
                    element = ethernet_element;
                }
            } while (false);
            return element;
        }

        private async Task<Dictionary<string, object>> GetExtraData(ISummary comm_element)
        {
            CommDeviceSelection comm_ds = NSetBox as CommDeviceSelection;
            NetDevice net_device = (NetDevice)comm_ds.NDevice.SelectedItem;
            CommunicationType comm_type = (CommunicationType)comm_ds.NType.SelectedItem;
            Dictionary<string, object> extra_data = comm_ds.ExtraData;

            if (net_device == NetDevice.LSIS_XGT && comm_type == CommunicationType.SERIAL)
            {
                if (extra_data.Count == 0)
                {
                    await this.ShowMessageAsync("Error", "Local port text is empty.");
                    return null;
                }
                ushort localport;
                if (!UInt16.TryParse(extra_data[EXTRA_LOCALPORT] as string, out localport))
                {
                    await this.ShowMessageAsync("Error", "Invalid local port number.");
                    return null;
                }
                extra_data[EXTRA_LOCALPORT] = localport;
                CommSerialPortElement comm_sp_element = comm_element as CommSerialPortElement;
                comm_sp_element.LocalPort = localport;
            }
            return extra_data;
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
                CommDeviceSelection comm_ds = NSetBox as CommDeviceSelection;
                if (comm_ds.NDevice.SelectedItem == null || comm_ds.NType.SelectedItem == null)
                {
                    await this.ShowMessageAsync("Error", "Please select Communication Device, Type");
                    break;
                }
                NetDevice net_device = (NetDevice)comm_ds.NDevice.SelectedItem;
                CommunicationType comm_type = (CommunicationType)comm_ds.NType.SelectedItem;
                UIElement comm_config = comm_ds.NGrid.Children[0];
                ISummary comm_element = null;
                comm_element = await GetCommElementData(comm_config as UserControl);
                if (comm_element == null)
                    break;
                Dictionary<string, object> extra_data = await GetExtraData(comm_element); //추가데이터 (국번 등)
                if (extra_data == null)
                    break;
                IConnect client = ServiceableDevice.CreateClient(net_device, comm_type, comm_element);
                CommClient cclient = new CommClient(client, net_device, comm_type, comm_element)
                {
                    Summary = comm_element.GetSummary(),
                    ExtraData = extra_data
                };
                CommClientDirector.GetInstance().Clientele.Add(cclient);
                Close();
            } while (false);
        }
    }
}