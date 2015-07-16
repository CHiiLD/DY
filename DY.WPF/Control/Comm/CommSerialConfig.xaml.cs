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

using System.IO.Ports;

namespace DY.WPF
{
    /// <summary>
    /// SerialCommConfig.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommSerialConfig : UserControl
    {
        public CommSerialConfig()
        {
            this.InitializeComponent();
        }

        public CommSerialParameter GetSerialCommStruct()
        {
            CommSerialParameter scs = new CommSerialParameter();
            scs.Com = (string)NCom.NComboBox.SelectedItem;
            scs.Bandrate = (int)NBaud.NComboBox.SelectedItem;
            scs.Parity = (Parity)NParity.NComboBox.SelectedItem;
            scs.StopBit = (StopBits)NStopBit.NComboBox.SelectedItem;
            scs.DataBit = (int)NStopBit.NComboBox.SelectedItem;
            return scs;
        }
    }
}