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
using System.Windows.Navigation;
using System.Windows.Shapes;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using DY.WPF.SYSTEM.COMM;
using DY.NET;
using System.Collections.ObjectModel;

namespace DY.WPF
{
    /// <summary>
    /// CommIOMonitoring.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommIOMonitoring : UserControl, ICommTabControl
    {
        private ACommIOMonitoringStrategy m_CommIOContext;
        public CommClient CClient
        {
            get
            {
                return NDataGrid.CClient;
            }
            set
            {
                //xaml 컨트롤 바인딩
                Binding plc_comm_swtich = new Binding("Usable") { Source = value };
                this.NBT_Usable.SetBinding(ToggleSwitch.IsCheckedProperty, plc_comm_swtich);

                Binding resp_ratency_t = new Binding("ResponseLatencyTime") { Source = value };
                this.NTB_ResponseRatencyT.NTextBox.SetBinding(TextBox.TextProperty, resp_ratency_t);

                Binding transfer_inteval = new Binding("TransferInteval") { Source = value };
                this.NTB_TransferInteval.NTextBox.SetBinding(TextBox.TextProperty, transfer_inteval);

                //전략(Strategy) 패턴 초기화
                switch (value.Target)
                {
                    case DYDevice.LSIS_XGT:
                        m_CommIOContext = new XGTCommIOMonitoring(CClient);
                        break;
                }

                NDataGrid.CClient = value;
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public CommIOMonitoring()
        {
            InitializeComponent();

            PlotModel plot_model = new PlotModel();
            plot_model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 100 });
            plot_model.Series.Add(new OxyPlot.Series.LineSeries { LineStyle = LineStyle.Solid });
            NPlotView.Model = plot_model;

            NBT_ExcelEditMode.IsCheckedChanged += OnCheckedChangedEditMode;
            NBT_ResponseIntevalGraph.IsCheckedChanged += OnCheckedChangedGraphActivation;
        }

        /// <summary>
        /// PLC IO 편집 모드 온/오프 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCheckedChangedEditMode(object sender, EventArgs e)
        {
            var btn = sender as ToggleSwitch;
            NDataGrid.Editable = btn.IsChecked == true ? true : false;
            //편집 모드일 경우 
            if (NDataGrid.Editable)
            {
                MetroWindow metro_win = Window.GetWindow(this) as MetroWindow; //Data preparation
                ProgressDialogController progress = await metro_win.ShowProgressAsync("Data preparation",
                "Please wait...", false, null);
                await m_CommIOContext.SetLoopAsync(false); //루프 작동 트리거 OFF
                await Task.Delay(1000);
                await progress.CloseAsync();
            }
            //편집모드를 닫을 경우
            else
            {
                IList<ICommIOData> items = NDataGrid.Items.Cast<ICommIOData>().ToList();
                //var items2 = items as IList<ICommIOData>;
                if (items == null)
                    throw new InvalidCastException("Can't cast ObservableCollection<CommIODataGridItem> to IList<ICommIOData>");
                m_CommIOContext.UpdateProtocols(items);
                await m_CommIOContext.SetLoopAsync(true); //루프 작동 트리거 ON
            }
        }

        private void OnCheckedChangedGraphActivation(object sender, EventArgs e)
        {
        }
    }
}
