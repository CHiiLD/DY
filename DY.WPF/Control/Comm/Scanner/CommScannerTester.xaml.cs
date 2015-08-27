using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;

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
            public long? Milliseconds { get; set; }
        
            public LogItem(string log, long? millisecond)
            {
                Time = DateTime.Now;
                Log = log;
                Milliseconds = millisecond;
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
        public List<LogItem> Items { get; set; }

        public CommScanTester(CommClient cclient)
        {
            InitializeComponent();
            CClient = cclient;
            Items = new List<LogItem>();
            Unselected += (object sender, EventArgs args) => { NLog.Items.Clear(); };
        }

        private void PushNewLog(IScannerAsync scanner, Delivery delivery)
        {
            string log = null;
            switch (delivery.Error)
            {
                case DeliveryError.SUCCESS:
                    if (delivery.Package != null)
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
            NLog.Items.Add(new LogItem(log, delivery.DelivaryTime.ElapsedMilliseconds));
        }

        private async void NBT_Scan_Click(object sender, RoutedEventArgs e)
        {
            IScannerAsync scanner = CClient.Socket as IScannerAsync;
            if (!scanner.IsConnected())
            {
                NLog.Items.Add(new LogItem(scanner.Description + ": Disconnected", null));
                return;
            }
            Delivery delivery = await scanner.ScanAsync();
            if (delivery != null)
                PushNewLog(scanner, delivery);
        }

        private async void NBT_Info_Click(object sender, RoutedEventArgs e)
        {
            IScannerAsync scanner = CClient.Socket as IScannerAsync;
            if (!scanner.IsConnected())
            {
                NLog.Items.Add(new LogItem(scanner.Description + ": Disconnected", null));
                return;
            }
            Delivery delivery = await scanner.GetInfoAsync();
            if (delivery != null)
                PushNewLog(scanner, delivery);
        }

        private void SetBinding()
        {
            this.SetBinding(UserControl.IsEnabledProperty,
                new Binding("Usable") { Source = m_CClient, Mode = BindingMode.TwoWay });

            NNM_WriteTimeout.SetBinding(NumericUpDownWithBar.ValueProperty,
                new Binding("WriteTimeout") { Source = m_CClient.Socket, Mode = BindingMode.TwoWay });
            NNM_ReadTimeout.SetBinding(NumericUpDownWithBar.ValueProperty,
                new Binding("ReadTimeout") { Source = m_CClient.Socket, Mode = BindingMode.TwoWay });

            NNM_WriteTimeout.SetBinding(NumericUpDownWithBar.MaximumProperty,
                new Binding("WriteTimeoutMaximum") { Source = m_CClient.Socket, Mode = BindingMode.TwoWay });
            NNM_ReadTimeout.SetBinding(NumericUpDownWithBar.MaximumProperty,
                new Binding("ReadTimeoutMaximum") { Source = m_CClient.Socket, Mode = BindingMode.TwoWay });

            NNM_WriteTimeout.SetBinding(NumericUpDownWithBar.MinimumProperty,
                new Binding("WriteTimeoutMinimum") { Source = m_CClient.Socket, Mode = BindingMode.TwoWay });
            NNM_ReadTimeout.SetBinding(NumericUpDownWithBar.MinimumProperty,
                new Binding("ReadTimeoutMinimum") { Source = m_CClient.Socket, Mode = BindingMode.TwoWay });
        }

        public void Dispose()
        {
        }
    }
}