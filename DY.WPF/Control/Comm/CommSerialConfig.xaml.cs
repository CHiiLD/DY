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
using DY.WPF.SYSTEM.COMM;

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

            string[] coms = SerialPort.GetPortNames();
            if (coms.Length != 0)
            {
                NCom.ItemSource = coms;
                NCom.SelectedItem = coms[0];
            }

            NBaud.ItemSource = SerialPortOption.BandRate;
            NBaud.SelectedItem = SerialPortOption.BandRate[SerialPortOption.BandRate.Count - 1];

            NDataBit.ItemSource = SerialPortOption.DataBit;
            NDataBit.SelectedItem = SerialPortOption.DataBit[SerialPortOption.DataBit.Count - 1];

            NParity.ItemSource = Enum.GetValues(typeof(Parity));
            NParity.SelectedItem = Parity.None;

            NStopBit.ItemSource = Enum.GetValues(typeof(StopBits));
            NStopBit.SelectedItem = StopBits.One;
        }

        public CommSerialParameter GetCommSerialStruct()
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