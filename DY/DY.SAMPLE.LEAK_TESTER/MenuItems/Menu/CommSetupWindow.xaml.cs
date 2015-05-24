using System.IO.Ports;
using System.Windows;
using System;

namespace DY.SAMPLE.LEAK_TESTER
{
    /// <summary>
    /// CommSetupWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommSetupWindow : Window
    {
        private SetupDirector.HOW_TO_CONNECT _Comm { get; set; }
        private SetupDirector.SetupPackage _Package;

        public CommSetupWindow()
        {
            this.InitializeComponent();
            Init();
        }

        private void Init()
        {
            _Package = new SetupDirector.SetupPackage( SetupDirector.GetInstance().Package );
            _Comm = SetupDirector.GetInstance().Comm;

            // 이더넷
            NXGTNet_TB.Text = _Package.EthernetInfo.PLC;
            NPrintNet_TB.Text = _Package.EthernetInfo.Printer;

            //시리얼 
            NPort.Text = _Package.SerialInfo.PortName;
            NBaud.Text = _Package.SerialInfo.BaudRate.ToString();
            NParity.Text = _Package.SerialInfo.Parity.ToString();
            NDataBit.Text = _Package.SerialInfo.DataBits.ToString();
            NStopBit.Text = _Package.SerialInfo.StopBits.ToString();
            NLocalPort.Text = _Package.SerialInfo.LocalPort.ToString();

            for (var v = Parity.None; v <= Parity.Space; v++)
                NParity.Items.Add(v);
            for (var v = StopBits.None; v <= StopBits.OnePointFive; v++)
                NStopBit.Items.Add(v);

            NParity.SelectedValue = _Package.SerialInfo.Parity;
            NStopBit.SelectedValue = _Package.SerialInfo.StopBits;

            // 시간 
            NDayStartH.Text = _Package.TimeInfo.Day.BeginTime.Hours.ToString();
            NDayStartM.Text = _Package.TimeInfo.Day.BeginTime.Minutes.ToString();
            NDayEndH.Text = _Package.TimeInfo.Day.EndTime.Hours.ToString();
            NDayEndM.Text = _Package.TimeInfo.Day.EndTime.Minutes.ToString();
            NNightStartH.Text = _Package.TimeInfo.Night.BeginTime.Hours.ToString();
            NNightStartM.Text = _Package.TimeInfo.Night.BeginTime.Minutes.ToString();
            NNightEndH.Text = _Package.TimeInfo.Night.EndTime.Hours.ToString();
            NNightEndM.Text = _Package.TimeInfo.Night.EndTime.Minutes.ToString();

            //라디오 박스
            switch (_Comm)
            {
                case SetupDirector.HOW_TO_CONNECT.ETHERNET:
                    OnEthernetSettingComponent();
                    NRadioNet.IsChecked = true;
                    break;
                case SetupDirector.HOW_TO_CONNECT.SERIAL:
                    OnSerialPortSettingComponent();
                    NRadioSerial.IsChecked = true;
                    break;
            }
        }

        private void NOK_Click(object sender, RoutedEventArgs e)
        {
            //시리얼 
            string portname = NPort.Text;
            string baudrate = NBaud.Text;
            Parity parity = (Parity)NParity.SelectedItem;
            string databit = NDataBit.Text;
            string local = NLocalPort.Text;
            StopBits stopbit = (StopBits)NStopBit.SelectedItem;
            int iBaud, iDatabit; 
            ushort iLocalPort;
            if (Int32.TryParse(baudrate, out iBaud) && 
                Int32.TryParse(databit, out iDatabit) && 
                UInt16.TryParse(local, out iLocalPort))
                _Package.SerialInfo = new SerialPortInfo() { PortName = portname,
                                                             BaudRate = iBaud,
                                                             Parity = parity,
                                                             DataBits = iDatabit,
                                                             StopBits = stopbit,
                                                             LocalPort = iLocalPort };
            //이더넷
            _Package.EthernetInfo.PLC = NXGTNet_TB.Text;
            _Package.EthernetInfo.Printer = NPrintNet_TB.Text;
            //시간
            int h, m;
            if (Int32.TryParse(NDayStartH.Text, out h) && Int32.TryParse(NDayStartM.Text, out m))
                _Package.TimeInfo.Day.BeginTime = new TimeSpan(00, h, m, 00, 00);
            if (Int32.TryParse(NDayEndH.Text, out h) && Int32.TryParse(NDayEndM.Text, out m))
                _Package.TimeInfo.Day.EndTime = new TimeSpan(00, h, m, 59, 999);
            if (Int32.TryParse(NNightStartH.Text, out h) && Int32.TryParse(NNightStartM.Text, out m))
                _Package.TimeInfo.Night.BeginTime = new TimeSpan(00, h, m, 00, 00);
            if (Int32.TryParse(NNightEndH.Text, out h) && Int32.TryParse(NNightEndM.Text, out m))
                _Package.TimeInfo.Night.EndTime = new TimeSpan(00, h, m, 59, 999);

            SetupDirector.GetInstance().Package = _Package;
            SetupDirector.GetInstance().Comm = _Comm;

            SetupDirector.GetInstance().SaveToFile();
            this.Close();
        }

        private void NCalcel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnEthernetSettingComponent()
        {
            NSetSerialPanel.IsEnabled = false;
            NSetXGTIPGrid.IsEnabled = true;
            NSetPrintIPGrid.IsEnabled = true;
        }

        private void OnSerialPortSettingComponent()
        {
            NSetSerialPanel.IsEnabled = true;
            NSetXGTIPGrid.IsEnabled = false;
            NSetPrintIPGrid.IsEnabled = false;
        }

        private void NRadioNet_Checked(object sender, RoutedEventArgs e)
        {
            OnEthernetSettingComponent();
            _Comm = SetupDirector.HOW_TO_CONNECT.ETHERNET;
        }

        private void NRadioSerial_Checked(object sender, RoutedEventArgs e)
        {
            OnSerialPortSettingComponent();
            _Comm = SetupDirector.HOW_TO_CONNECT.SERIAL;
        }
    }
}