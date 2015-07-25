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
using MahApps.Metro.Controls;

namespace DY.WPF
{
    /// <summary>
    /// CommConnectionSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommConnectionManage : UserControl
    {
        public CommConnectionManage()
        {
            this.InitializeComponent();
            CommClientManagement ccmm = CommClientManagement.GetInstance();
            Binding reconnect_swtich_bind = new Binding("Source") { Source = ccmm.UsableReconnectProperty };
            this.NBT_ReconnectSwtich.SetBinding(ToggleSwitch.IsCheckedProperty, reconnect_swtich_bind);

            Binding reconnect_inteval_bind = new Binding("Source") { Source = ccmm.ReconnectIntevalProperty };
            this.NTB_ReconnectInteval.NTextBox.SetBinding(TextBox.TextProperty, reconnect_inteval_bind);

            Binding resp_ratency_bind = new Binding("Source") { Source = ccmm.ResponseLatencyProperty };
            this.NTB_RespLatencyTime.NTextBox.SetBinding(TextBox.TextProperty, resp_ratency_bind);
        }
    }
}