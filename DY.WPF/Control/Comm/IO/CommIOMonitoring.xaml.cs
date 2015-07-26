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
using DY.WPF.SYSTEM.IO;
using PropertyTools.Wpf;

namespace DY.WPF
{
    /// <summary>
    /// CommIOMonitoring.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommIOMonitoring : UserControl, ICommTabControl
    {
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

                //xaml 컨트롤 바인딩
                Binding resp_ratency_t = new Binding("ResponseLatencyTime") { Source = value };
                this.NTB_ResponseRatencyT.NTextBox.SetBinding(TextBox.TextProperty, resp_ratency_t);

                Binding transfer_inteval = new Binding("TransferInteval") { Source = value };
                this.NTB_TransferInteval.NTextBox.SetBinding(TextBox.TextProperty, transfer_inteval);
#if false
                //skt on/off 이벤트 캐치해서 IO모니터링 off
                CClient.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == "Usable")
                    {
                        var cc = sender as CommClient;
                        if (cc.Usable == false && NBT_MonitoringOnOff.IsChecked == true)
                            NBT_MonitoringOnOff.IsChecked = false;
                    }
                };
#endif
            }
        }

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
        }

        private async Task MonitoringStart()
        {
            IList<ICommIOData> items = NDataGrid.Items.Cast<ICommIOData>().ToList();
            if (items == null)
                throw new InvalidCastException("Can't cast ObservableCollection<CommIODataGridItem> to IList<ICommIOData>");
            m_CommIOContext.UpdateProtocols(items);
            await m_CommIOContext.SetLoopAsync(true); //루프 작동 트리거 ON
        }

        private async Task MonitoringStop()
        {
            await m_CommIOContext.SetLoopAsync(false); //루프 작동 트리거 OFF
        }

        /// <summary>
        /// PLC IO 편집 모드 온/오프 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCheckChangedEditMode(object sender, EventArgs e)
        {
            IList<ICommIOData> items = NDataGrid.Items.Cast<ICommIOData>().ToList();
            bool haveException = false;
            string exception_msg = null;
            var toggle = sender as ToggleSwitch;
            bool check = toggle.IsChecked == true ? true : false;
            
            do
            {
                if (check) //편집모드에서 나갈 때 
                    break;
                NDataGrid.RemoveEmtpyCollectionItem();
                if (items.Count == 0)
                    break;
                try
                {
                    m_CommIOContext.UpdateProtocols(items);
                }
                catch (Exception exception)
                {
                    haveException = true;
                    exception_msg = exception.Message;
                }
                if (haveException)
                {
                    MetroWindow metro_win = Window.GetWindow(this) as MetroWindow; //Data preparation
                    await metro_win.ShowMessageAsync("Notice", "Can't save the edited content.\n" + exception_msg);
                    toggle.IsChecked = true;
                    return;
                }
            } while(false);
            NDataGrid.Editable = check;
        }
#if false
        /// <summary>
        /// IO 입출력 모니터링 ON/OFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCheckChangedMonitoring(object sender, EventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            MetroWindow metro_win = Window.GetWindow(this) as MetroWindow; //Data preparation
            if (toggle.IsChecked == true)
            {
                //통신이 연결되어 있지 아니한 상태 or 편집모드일 경우 
                if (CClient.Usable != true)
                {
                    await metro_win.ShowMessageAsync("Notice", "Please connect the communication.");
                    NBT_MonitoringOnOff.IsChecked = false;
                    return;
                }
                if (NDataGrid.Editable)
                {
                    await metro_win.ShowMessageAsync("Notice", "The current edit mode.");
                    NBT_MonitoringOnOff.IsChecked = false;
                    return;
                }
                await MonitoringStart();
            }
            else
            {
                ProgressDialogController progress = await metro_win.ShowProgressAsync("Monitoring stop",
                    "Please wait...", false, null);
                await MonitoringStop();
                await Task.Delay(1000);
                await progress.CloseAsync();
            }
        }
#endif

        private void OnCheckChangedGraphActivation(object sender, EventArgs e)
        {

        }
    }
}
