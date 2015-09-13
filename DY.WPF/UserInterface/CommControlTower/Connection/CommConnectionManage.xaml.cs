using System.Windows.Controls;
using System.Windows.Data;

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
            InitializeComponent();
            CommClientDirector director = CommClientDirector.GetInstance();

            NBT_ConnectionCheckTimerSwtich.SetBinding(ToggleSwitch.IsCheckedProperty,
                new Binding("Source") { Source = director.ConnectCheckableProperty, Mode=BindingMode.TwoWay });

            NTB_ConnectionCheckInteval.SetBinding(NumericUpDownWithBar.ValueProperty,
                new Binding("Source") { Source = director.ConnectCheckIntevalProperty, Mode = BindingMode.TwoWay });

            NTB_ConnectionTimeout.SetBinding(NumericUpDownWithBar.ValueProperty,
                new Binding("Source") { Source = director.ConnectTimeoutProperty, Mode = BindingMode.TwoWay });
        }
    }
}