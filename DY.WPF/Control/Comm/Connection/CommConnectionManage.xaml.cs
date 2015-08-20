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
                new Binding("Source") { Source = director.ConnectionCheckableProperty, Mode=BindingMode.TwoWay });

            NTB_ConnectionCheckInteval.SetBinding(NumericUpDownWithBar.ValueProperty,
                new Binding("Source") { Source = director.ConnectionCheckIntevalProperty, Mode = BindingMode.TwoWay });

            NTB_ConnectionDelayTime.SetBinding(NumericUpDownWithBar.ValueProperty,
                new Binding("Source") { Source = director.ConnectionDelayTimeProperty, Mode = BindingMode.TwoWay });
        }
    }
}