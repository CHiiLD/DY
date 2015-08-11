using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using OxyPlot;
using OxyPlot.Axes;
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
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private ACommIOMonitoringStrategy m_CommIOContext;
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
            CClient = aCommIOMonitoringStrategy.CClient;

            //그래프 객체 초기화
            PlotModel plot_model = new PlotModel();
            plot_model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 100 });
            plot_model.Series.Add(new OxyPlot.Series.LineSeries { LineStyle = LineStyle.Solid });
            NPlotView.Model = plot_model;

            //컨트롤 이벤트 설정
            NBT_ExcelEditModeOnOff.IsCheckedChanged += OnCheckChangedEditMode;
            NBT_RespRatencyTOnOff.IsCheckedChanged += OnCheckChangedGraphActivation;
            Selected = OnSelectedAsync;
            Unselected = OnUnselectedAsync;
        }

        public async void OnSelectedAsync(object sender, EventArgs args)
        {
            if (!NDataGrid.Editable) //편집 모드가 아닐 때 모니터링 시작 ..
                await StartMonitoring();
        }

        public async void OnUnselectedAsync(object sender, EventArgs args)
        {
            await StopMonitoring();
        }

        private async Task StartMonitoring()
        {
            if (NDataGrid.Items.Count == 0)
                return;
            LOG.Trace("모니터링 요청");
            IList<ICommIOData> items = NDataGrid.Items.Cast<ICommIOData>().ToList();
            m_CommIOContext.ReplaceICommIOData(items);
            await m_CommIOContext.SetLoopAsync(true); //루프 작동 트리거 ON
        }

        private async Task StopMonitoring()
        {
            if (NDataGrid.Items.Count == 0)
                return;
            await m_CommIOContext.SetLoopAsync(false); //루프 작동 트리거 OFF
            LOG.Trace("모니터링 종료");
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
            bool check = toggle.IsChecked == true ? true : false;
            do
            {
                if (check) //편집모드에서 나갈 때 
                    break;
                NDataGrid.RemoveEmtpyCollectionItem();
                IList<ICommIOData> items = NDataGrid.Items.Cast<ICommIOData>().ToList();
                if (items.Count == 0)
                    break;
                try
                {
                    m_CommIOContext.ReplaceICommIOData(items);
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

            if (check)
                await StopMonitoring();
            else
                await StartMonitoring();
            NDataGrid.Editable = check;
        }

        private void OnCheckChangedGraphActivation(object sender, EventArgs e)
        {

        }
    }
}
