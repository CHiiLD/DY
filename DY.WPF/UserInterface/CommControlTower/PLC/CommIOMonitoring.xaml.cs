using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using OxyPlot;
using OxyPlot.Wpf;

using DY.WPF.SYSTEM.COMM;
using DY.NET;

using PropertyTools.Wpf;

using NLog;


namespace DY.WPF
{
    /// <summary>
    /// CommIOMonitoring.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommIOMonitoring : UserControl, ICommControlTowerTabItem
    {
        public class DateValue
        {
            public DateTime Date { get; set; }
            public double Value { get; set; }
        }

        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private ACommIOMonitoringStrategy m_CommIOContext;
        private CommClient m_CClient;

        private DispatcherTimer m_PlotTimer;
        private Collection<DateValue> m_PlotItems { get; set; }
        private List<Delivery> m_Deliveries = new List<Delivery>();

        private Plot Plot
        {
            get
            {
                return NPlot;
            }
        }

        private bool Editable
        {
            get
            {
                return NDataGridA.Editable;
            }
            set
            {
                NDataGridA.Editable = NDataGridB.Editable = value;
            }
        }

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

        /// <summary>
        /// 초기화
        /// </summary>
        public CommIOMonitoring(ACommIOMonitoringStrategy aCommIOMonitoringStrategy)
        {
            InitializeComponent(); //xaml 컨트롤 초기화
            m_CommIOContext = aCommIOMonitoringStrategy;
            m_CommIOContext.DeliveryArrived += OnDeliveryArrived;
            CClient = aCommIOMonitoringStrategy.CClient;

            InitPlotModel();

            //컨트롤 이벤트 설정
            NBT_LockOnOff.IsCheckedChanged += OnCheckChangedEditMode;
            Selected = OnSelectedAsync;
            Unselected = OnUnselectedAsync;

            NNM_UpdateInteval.NNumeric.ValueChanged += (object sender, RoutedPropertyChangedEventArgs<double?> e) =>
            {
                if (e.NewValue == null)
                    return;
                int int_value = (int)e.NewValue;
                m_PlotTimer.Interval = new TimeSpan(int_value * 10000);
                LOG.Trace("업데이트 간격 변경: " + int_value + "ms");
            };
        }

        ~CommIOMonitoring()
        {
            Dispose();
        }

        public void Dispose()
        {
            StopMonitoring();
            GC.SuppressFinalize(this);
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

            NNM_UpdateInteval.SetBinding(NumericUpDownWithBar.ValueProperty,
                new Binding("IOUpdateInteval") { Source = m_CClient, Mode = BindingMode.TwoWay });
        }

        private void PushLog(string log)
        {
            NLog.Items.Add(log);
        }

        private void InitPlotModel()
        {
            //그래프 객체 초기화
            m_PlotTimer = new DispatcherTimer(new TimeSpan(10000 * CClient.UpdateInteval),
            DispatcherPriority.Normal, OnPlotTimerTick, Dispatcher) { IsEnabled = false };
            LineSeries lineSeries = new LineSeries()
            {
                ItemsSource = m_PlotItems = new Collection<DateValue>(),
                DataFieldX = "Date",
                DataFieldY = "Value",
                MarkerSize = 2,
                MarkerType = MarkerType.Circle,
            };
            lineSeries.SetResourceReference(LineSeries.MarkerStrokeProperty, "HighlightColor");
            lineSeries.SetResourceReference(LineSeries.MarkerFillProperty, "HighlightColor");
            lineSeries.SetResourceReference(LineSeries.ColorProperty, "AccentColor");
            Plot.Series.Add(lineSeries);
        }
        public void OnSelectedAsync(object sender, EventArgs args)
        {
            if (!Editable) //편집 모드가 아닐 때 모니터링 시작 ..
                StartMonitoring();
        }

        public void OnUnselectedAsync(object sender, EventArgs args)
        {
            StopMonitoring();
        }

        private void StartMonitoring()
        {
            if (NDataGridA.Items.Count + NDataGridB.Items.Count == 0)
                return;
            LOG.Trace("모니터링 요청");
            PushLog("Start monitoring.");
            UpdateNewIODataList();
            m_CommIOContext.Activated = true; //루프 작동 트리거 ON
            m_PlotItems.Clear();
            m_PlotTimer.Start();
        }

        private void StopMonitoring()
        {
            if (NDataGridA.Items.Count == 0)
                return;
            m_PlotTimer.Stop();
            PushLog("Exit monitoring.");
            m_CommIOContext.Activated = false; //루프 작동 트리거 OFF
            LOG.Trace("모니터링 종료");
        }

        private void UpdateNewIODataList()
        {
            NDataGridA.RemoveEmtpyAddressCell();
            NDataGridB.RemoveEmtpyAddressCell();
            List<ICommIOData> itemsA = NDataGridA.Items.Cast<ICommIOData>().ToList();
            List<ICommIOData> itemsB = NDataGridB.Items.Cast<ICommIOData>().ToList();
            itemsA.AddRange(itemsB);
            m_CommIOContext.ReplaceICommIOData(itemsA);
        }

        /// <summary>
        /// PLC IO 편집 모드 온/오프 토글스위치버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCheckChangedEditMode(object sender, EventArgs e)
        {
            bool haveException = false;
            string exception_msg = null;
            ToggleSwitch toggle = sender as ToggleSwitch;
            MetroWindow metro_win = Window.GetWindow(this) as MetroWindow; //Data preparation
            bool isLock = toggle.IsChecked == true ? true : false;
            do
            {
                if (!isLock) //편집모드에서 나갈 때 
                    break;
                if (NDataGridA.Items.Count + NDataGridB.Items.Count == 0)
                    break;
                try
                {
                    UpdateNewIODataList();
                }
                catch (Exception exception)
                {
                    haveException = true;
                    exception_msg = exception.Message;
                }
                if (haveException)
                {
                    await metro_win.ShowMessageAsync("Notice", "Can't save the edited content.\n" + exception_msg);
                    toggle.IsChecked = true;
                    return;
                }
            } while (false);
            Editable = !isLock;
            if (Editable)
                StopMonitoring();
            else
                StartMonitoring();
        }

        private void OnPlotTimerTick(object sender, EventArgs args)
        {
            if (NBT_SpeedMonitorOnOff.IsChecked != true)
                return;
            if (m_Deliveries.Count == 0)
                return;
            double ms = 0;
            m_Deliveries.Sort((d1, d2) =>
            {
                if (d1.Error > d2.Error) return 1;
                else if (d1.Error < d2.Error) return -1;
                else return 0;
            });

            switch (m_Deliveries.Last().Error)
            {
                case DeliveryError.SUCCESS:
                    ms = m_Deliveries.Max(delivery => delivery.DelivaryTime.ElapsedMilliseconds);
                    break;
                case DeliveryError.DISCONNECT:
                    ms = 0;
                    PushLog("Not connected to the server.");
                    break;
                case DeliveryError.WRITE_TIMEOUT:
                    ms = CClient.Socket.WriteTimeout + CClient.Socket.ReadTimeout;
                    PushLog("Write timeout error has occurred.");
                    break;
                case DeliveryError.READ_TIMEOUT:
                    ms = CClient.Socket.WriteTimeout + CClient.Socket.ReadTimeout;
                    PushLog("Read timeout error has occurred.");
                    break;
            }
            UpdatePlotModel(DateTime.Now, ms);
            m_Deliveries.Clear();
        }

        private void OnDeliveryArrived(object sender, DeliveryArrivalEventArgs args)
        {
            m_Deliveries.Add(args.Delivery);
        }

        private void UpdatePlotModel(DateTime signal_time, double milliseconds)
        {
            m_PlotItems.Add(new DateValue()
            {
                Date = signal_time,
                Value = milliseconds
            });
            if (m_PlotItems.Count >= 100)
                m_PlotItems.RemoveAt(0);
            Plot.InvalidatePlot(true);
        }
    }
}