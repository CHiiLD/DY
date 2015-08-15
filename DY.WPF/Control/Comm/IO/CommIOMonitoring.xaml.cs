﻿using System;
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

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

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
            public long Value { get; set; }
        }

        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private ACommIOMonitoringStrategy m_CommIOContext;
        private CommClient m_CClient;

        private DispatcherTimer m_PlotTimer;
        private PlotModel m_PlotModel;
        private Collection<DateValue> m_PlotItems { get; set; }
        private object m_DeliveryAccess = new object();
        private Delivery m_CurDelivery;

        private bool EditMode
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

                Binding activation = new Binding("Usable") { Source = m_CClient };
                this.SetBinding(UserControl.IsEnabledProperty, activation);

                Binding write_timeout = new Binding("WriteTimeout") { Source = m_CClient };
                this.NNM_WriteTimeout.NNumeric.SetBinding(NumericUpDown.ValueProperty, write_timeout);

                Binding read_timeout = new Binding("ReadTimeout") { Source = m_CClient };
                this.NNM_ReadTimeout.NNumeric.SetBinding(NumericUpDown.ValueProperty, read_timeout);

                Binding io_update_inteval = new Binding("IOUpdateInteval") { Source = m_CClient };
                this.NNM_UpdateInteval.NNumeric.SetBinding(NumericUpDown.ValueProperty, io_update_inteval);
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
            // MajorGridlineStyle="Solid" MinorGridlineStyle="Dot"
            //그래프 객체 초기화
            m_PlotTimer = new DispatcherTimer(
                new TimeSpan(CommClient.UpdateIntevalMinimum * 10000),
                DispatcherPriority.Normal,
                OnPlotTimerTick,
                Dispatcher);
            PlotModel plot_model = new PlotModel();
            plot_model.Axes.Add(new DateTimeAxis //X축
            {
                StringFormat = "mm:ss",
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle= LineStyle.Dot,
                TickStyle = TickStyle.Inside,
            });
            plot_model.Axes.Add(new LinearAxis //Y축
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                TickStyle = TickStyle.Inside,
                StartPosition = 1,
                EndPosition = 0,
                Title = "Millisecond"
            });
            plot_model.Series.Add(new LineSeries
            {
                ItemsSource = m_PlotItems = new Collection<DateValue>(),
                DataFieldX = "Date",
                DataFieldY = "Value",
                //StrokeThickness = 2,
                //MarkerSize = 3,
                //MarkerStroke = OxyColors.Blue,
                //MarkerFill = OxyColors.Blue,
                //Color = OxyColors.Blue,
                //MarkerType = MarkerType.Circle,
            });
            m_PlotModel = NPlotView.Model = plot_model;

            //컨트롤 이벤트 설정
            NBT_EditModeOnOff.IsCheckedChanged += OnCheckChangedEditMode;
            Selected = OnSelectedAsync;
            Unselected = OnUnselectedAsync;
        }

        ~CommIOMonitoring()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_PlotTimer != null)
                m_PlotTimer.Stop();
            GC.SuppressFinalize(this);
        }

        public async void OnSelectedAsync(object sender, EventArgs args)
        {
            if (!EditMode) //편집 모드가 아닐 때 모니터링 시작 ..
                await StartMonitoring();
        }

        public async void OnUnselectedAsync(object sender, EventArgs args)
        {
            await StopMonitoring();
        }

        private async Task StartMonitoring()
        {
            if (NDataGridA.Items.Count + NDataGridB.Items.Count == 0)
                return;
            LOG.Trace("모니터링 요청");
            UpdateNewIODataList();
            await m_CommIOContext.SetRunAsync(true); //루프 작동 트리거 ON
            m_PlotTimer.Start();
        }

        private async Task StopMonitoring()
        {
            if (NDataGridA.Items.Count == 0)
                return;
            m_PlotTimer.Stop();
            await m_CommIOContext.SetRunAsync(false); //루프 작동 트리거 OFF
            LOG.Trace("모니터링 종료");
        }

        private void UpdateNewIODataList()
        {
            NDataGridA.RemoveEmtpyCollectionItem();
            NDataGridB.RemoveEmtpyCollectionItem();
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
            var toggle = sender as ToggleSwitch;
            MetroWindow metro_win = Window.GetWindow(this) as MetroWindow; //Data preparation
            bool isChecked = toggle.IsChecked == true ? true : false;
            do
            {
                if (isChecked) //편집모드에서 나갈 때 
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
            EditMode = isChecked;
            if (isChecked)
                await StopMonitoring();
            else
                await StartMonitoring();
        }

        private void OnPlotTimerTick(object sender, EventArgs args)
        {
            if (NBT_SpeedMonitorOnOff.IsChecked != true)
                return;
            long ms = 0;
            DeliveryError error = DeliveryError.DISCONNECT;
            lock (m_DeliveryAccess)
            {
                if (m_CurDelivery == null)
                {
                    return;
                }
                else
                {
                    ms = m_CurDelivery.DelivaryTime.ElapsedMilliseconds;
                    error = m_CurDelivery.Error;
                    m_CurDelivery = null;
                }
            }
            switch (error)
            {
                case DeliveryError.DISCONNECT:
                    ms = 0;
                    break;
                case DeliveryError.WRITE_TIMEOUT:
                case DeliveryError.READ_TIMEOUT:
                    ms = CClient.WriteTimeout + CClient.ReadTimeout;
                    break;
            }
            lock (m_PlotModel.SyncRoot)
            {
                UpdatePlotModel(DateTime.Now, ms);
            }
            m_PlotModel.InvalidatePlot(true);
        }

        private void UpdatePlotModel(DateTime signal_time, long milliseconds)
        {
            m_PlotItems.Add(new DateValue()
            {
                Date = signal_time,
                Value = milliseconds
            });
            if (m_PlotItems.Count >= 40)
                m_PlotItems.RemoveAt(0);
        }

        private void OnDeliveryArrived(object sender, DeliveryArrivalEventArgs args)
        {
            lock (m_DeliveryAccess)
            {
                m_CurDelivery = args.Delivery;
            }
        }
    }
}