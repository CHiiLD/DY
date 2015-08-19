using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Text;

using DY.NET;
using DY.WPF.SYSTEM.COMM;
using MahApps.Metro.Controls;

namespace DY.WPF
{
    /// <summary>
    /// CommScanTester.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommScanTester : UserControl, ICommControlTowerTabItem
    {
        public class LogItem
        {
            public DateTime Time { get; set; }
            public string Log { get; set; }

            public LogItem(string log)
            {
                Time = DateTime.Now;
                Log = log;
            }
        }

        private CommClient m_CClient;
        public CommClient CClient 
        { 
            get
            {
                return m_CClient;
            }
            set
            {
                m_CClient = value;
                SetBinding();
            }
        }
        public EventHandler<EventArgs> Selected { get; set; }
        public EventHandler<EventArgs> Unselected { get; set; }

        public CommScanTester(CommClient cclient)
        {
            InitializeComponent();
            CClient = cclient;
            Unselected += (object sender, EventArgs args) => { NLog.Items.Clear(); };
        }

        private void PushNewLog(IScannerSerialCommAsync scanner, Delivery delivery)
        {
            string log = null;
            switch (delivery.Error)
            {
                case DeliveryError.SUCCESS:
                    log = Encoding.ASCII.GetString(delivery.Package as byte[]);
                    break;
                case DeliveryError.DISCONNECT:
                    log = scanner.Description + ": Disconnected";
                    break;
                case DeliveryError.READ_TIMEOUT:
                    log = scanner.Description + ": Read timeout";
                    break;
                case DeliveryError.WRITE_TIMEOUT:
                    log = scanner.Description + ": Write timeout";
                    break;
            }
            NLog.Items.Add(new LogItem(log));
        }

        private async void NBT_Scan_Click(object sender, RoutedEventArgs e)
        {
            IScannerSerialCommAsync scanner = CClient.Socket as IScannerSerialCommAsync;
            if (!scanner.IsConnected())
            {
                NLog.Items.Add(new LogItem(scanner.Description + ": Disconnected"));
                return;
            }
            Delivery delivery = await scanner.ScanAsync();
            PushNewLog(scanner, delivery);
        }

        private async void NBT_Info_Click(object sender, RoutedEventArgs e)
        {
            IScannerSerialCommAsync scanner = CClient.Socket as IScannerSerialCommAsync;
            if (!scanner.IsConnected())
            {
                NLog.Items.Add(new LogItem(scanner.Description + ": Disconnected"));
                return;
            }
            Delivery delivery = await scanner.GetInfoAsync();
            PushNewLog(scanner, delivery);
        }

        private void SetBinding()
        {
            Binding activation = new Binding("Usable") { Source = m_CClient };
            this.SetBinding(UserControl.IsEnabledProperty, activation);

            Binding write_timeout = new Binding("WriteTimeout") { Source = m_CClient };
            this.NNM_WriteTimeout.NNumeric.SetBinding(NumericUpDown.ValueProperty, write_timeout);

            Binding read_timeout = new Binding("ReadTimeout") { Source = m_CClient };
            this.NNM_ReadTimeout.NNumeric.SetBinding(NumericUpDown.ValueProperty, read_timeout);
        }

        public void Dispose()
        {
        }
    }
}