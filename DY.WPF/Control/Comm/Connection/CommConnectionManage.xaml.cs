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
            CommClientDirector ccmm = CommClientDirector.GetInstance();
            Binding binding = new Binding("Source") { Source = ccmm.ConnectionCheckableProperty };
            this.NBT_ConnectionCheckTimerSwtich.SetBinding(ToggleSwitch.IsCheckedProperty, binding);

            binding = new Binding("Source") { Source = ccmm.ConnectionCheckIntevalProperty };
            this.NTB_ConnectionCheckInteval.NTextBox.SetBinding(TextBox.TextProperty, binding);

            binding = new Binding("Source") { Source = ccmm.ConnectionDelayTimeProperty };
            this.NTB_ConnectionDelayTime.NTextBox.SetBinding(TextBox.TextProperty, binding);
        }
    }
}