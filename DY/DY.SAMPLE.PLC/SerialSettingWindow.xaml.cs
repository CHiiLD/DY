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

using System.IO.Ports;
using DY.NET.LSIS.XGT;

namespace DY.SAMPLE.PLC
{
    /// <summary>
    /// Interaction logic for ComSettingWindow.xaml
    /// </summary>
    public partial class SerialSettingWindow : Window
    {
        public SerialSettingWindow()
        {
            InitializeComponent();

            for(var v = Parity.None; v <= Parity.Space; v++)
                NParity.Items.Add(v);
            for (var v = StopBits.None; v <= StopBits.OnePointFive; v++)
                NStopBit.Items.Add(v);

            NParity.SelectedItem = Parity.None;
            NStopBit.SelectedItem = StopBits.One;
        }

        private void NOK_Click(object sender, RoutedEventArgs e)
        {
            string portname = NPort.Text;
            string baudrate = NBaud.Text;
            Parity parity = (Parity)NParity.SelectedItem;
            string databit = NDataBit.Text;
            StopBits stopbit = (StopBits)NStopBit.SelectedItem;
            int iBaud, iDatabit;
            var main = Owner as MainWindow;
            if( Int32.TryParse(baudrate, out iBaud) && Int32.TryParse(databit, out iDatabit) )
                main.SetSocket(COM.SERIAL, new XGTCnetSocket.Builder(portname, iBaud).Parity(parity).DataBits(iDatabit).StopBits(stopbit).Build());
            else
                main.SetSocket(COM.SERIAL, null);
            this.Close();
        }

        private void NCancel_Click(object sender, RoutedEventArgs e)
        {
            var main = Owner as MainWindow;
            main.SetSocket(COM.SERIAL, null);
            this.Close();
        }
    }
}
