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
using DY.SAMPLE.LOGIC;
using DY.NET.LSIS.XGT;
using DY.NET;
using System.ComponentModel;

namespace DY.SAMPLE.PLC
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        class ListViewLogItem
        {
            public string Time { get; set; }
            public string Log { get; set; }
            public string Value { get; set; }
        }

        struct DataBox<T>
        {
            public T Sum;
            public uint Count;
            public T Max;
            public T Min;
        }

        private SwitchLoopLogic _SwitchLoopLogic;
        private Win32.HiPerfTimer _timer;

        private DataBox<double> _SpeedBox = new DataBox<double>();
        private DataBox<int> _ValueBox = new DataBox<int>();
        private int _Old = int.MinValue;

        private ListSortDirection _ListSortDirection = ListSortDirection.Ascending;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void NPLC_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;
            PLC plc = (PLC)cb.SelectedItem;
            switch (plc)
            {
                case
                PLC.LSIS_XGT: NComCB.Items.Add(COM.SERIAL);
                    break;
                default:
                    break;
            }
        }

        private void NComCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;
            COM com = (COM)cb.SelectedItem;
            switch (com)
            {
                case COM.SERIAL:
                    var serialWin = new SerialSettingWindow();
                    serialWin.Owner = this;
                    serialWin.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        public void Ready()
        {
            NCheckStartBtn.IsEnabled = true;
            NCheckStopBtn.IsEnabled = true;
            NDataSaveBtn.IsEnabled = true;
        }

        public void Initialize()
        {
            NCheckStopBtn_Click(null, null);

            NCheckStartBtn.IsEnabled = false;
            NCheckStopBtn.IsEnabled = false;
            NDataSaveBtn.IsEnabled = false;

            NStateTB.Text = "위 콤보박스에 측정할 기기와 통신방법을 설정해주세요.";
            NRetLV.Items.Clear();
            NComCB.Items.Clear();
            NPLC_CB.Items.Clear();
            NPLC_CB.Items.Add(PLC.LSIS_XGT);
            if (_SwitchLoopLogic != null)
                _SwitchLoopLogic.CheckStop();
            _SwitchLoopLogic = null;

            NCurRecvSP.Content = "0";
            NAvrRecvSP.Content = "0";
            NMinRecvSP.Content = "0";
            NMaxRecvSP.Content = "0";

            NCurLoseV.Content = "0";
            NAvrLoseV.Content = "0";
            NMinLoseV.Content = "0";
            NMaxLoseV.Content = "0";

            _SpeedBox = new DataBox<double>();
            _ValueBox = new DataBox<int>();
            _Old = int.MinValue;
        }

        public void SetPort(COM com, object port)
        {
            if (port == null)
            {
                Initialize();
                return;
            }
            switch (com)
            {
                case COM.SERIAL:
                    DYSerialPort sp = port as DYSerialPort;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(sp.PortName + " ");
                    sb.Append(sp.BaudRate + " ");
                    sb.Append("Parity-" + sp.Parity + " ");
                    sb.Append("DataBits-" + sp.DataBits + " ");
                    sb.Append("StopBits-" + sp.StopBits + " ");
                    NStateTB.Text = sb.ToString();

                    _SwitchLoopLogic = new SwitchLoopLogic(sp);
                    _SwitchLoopLogic.OnReceivedValueEvent += OnRecvValueToPLC;
                    _SwitchLoopLogic.CnetExclusiveSocket.OnSendedSuccessfully += OnSendTiming;
                    _SwitchLoopLogic.CnetExclusiveSocket.OnReceivedSuccessfully += OnRecvTiming;
                    Ready();
                    break;
                default:
                    return;
            }
        }

        private void OnRecvValueToPLC(object sender, IntegerReceivedToPlcEventArgs e)
        {
            NRetLV.Dispatcher.BeginInvoke(new Action(() =>
            {
                int i = (int)(short)e.Value;
                WriteValueRecord(i);
                DateTime dt = DateTime.Now;
                ListViewLogItem item = new ListViewLogItem()
                {
                    Time = dt.Hour + ":" + dt.Minute + ":" + dt.Second + "." + dt.Millisecond,
                    Value = e.Value.ToString(),
                    Log = "프로토콜 데이터 출력"
                };
                NRetLV.Items.Add(item);
                NRetLV.ScrollIntoView(NRetLV.Items[NRetLV.Items.Count - 1]);
            }), null);
        }

        private void NInitBtn_Click(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        private void OnSendTiming(object sender, EventArgs e)
        {
            _timer = new Win32.HiPerfTimer();
            _timer.Start();
        }

        private void OnRecvTiming(object sender, EventArgs e)
        {
            _timer.Stop();
            var duration = _timer.Duration * 1000;

            NRetLV.Dispatcher.BeginInvoke(new Action(() =>
            {
                WriteDurationRecord(duration);
                DateTime dt = DateTime.Now;
                ListViewLogItem item = new ListViewLogItem()
                {
                    Time = dt.Hour + ":" + dt.Minute + ":" + dt.Second + "." + dt.Millisecond,
                    Value = "",
                    Log = "초정밀 측정 " + duration.ToString() + " ms"
                };
                NRetLV.Items.Add(item);
                NRetLV.ScrollIntoView(NRetLV.Items[NRetLV.Items.Count - 1]);
            }), null);

            _timer = null;
        }

        private void NCheckStartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_SwitchLoopLogic != null)
                _SwitchLoopLogic.CheckStart();
        }

        private void NCheckStopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_SwitchLoopLogic != null)
                _SwitchLoopLogic.CheckStop();
            _Old = int.MinValue;
        }

        private void NDataSaveBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WriteDurationRecord(double duration)
        {
            NCurRecvSP.Content = string.Format("{0:F4} ms", duration);
            _SpeedBox.Count++;
            _SpeedBox.Sum += duration;
            NAvrRecvSP.Content = string.Format("{0:F4} ms", _SpeedBox.Sum / _SpeedBox.Count);
            if (_SpeedBox.Max < duration)
            {
                _SpeedBox.Max = duration;
                NMinRecvSP.Content = string.Format("{0:F4} ms", duration);
            }
            if (_SpeedBox.Min > duration || _SpeedBox.Min == 0)
            {
                _SpeedBox.Min = duration;
                NMaxRecvSP.Content = string.Format("{0:F4} ms", duration);
            }
        }

        private void WriteValueRecord(int value)
        {
            if (_Old == int.MinValue)
            {
                _Old = value;
                _ValueBox.Min = int.MaxValue;
                return;
            }
            int ret = value - _Old - 1;
            NCurLoseV.Content = string.Format("{0}", ret);
            _ValueBox.Count++;
            _ValueBox.Sum += ret;
            double avr = (double)_ValueBox.Sum / _ValueBox.Count;
            NAvrLoseV.Content = string.Format("{0:F2}", avr);
            if (_ValueBox.Max < ret)
            {
                _ValueBox.Max = ret;
                NMaxLoseV.Content = string.Format("{0}", ret);
            }
            if (_ValueBox.Min > ret)
            {
                _ValueBox.Min = ret;
                NMinLoseV.Content = string.Format("{0}", ret);
            }
            _Old = value;
        }

        private void NRetLV_Click(object sender, RoutedEventArgs e)
        {
            ListView lv = sender as ListView;
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked == null)
                return;
            string header = headerClicked.Column.Header as string;
            List<ListViewLogItem> list = lv.Items.OfType<ListViewLogItem>().ToList<ListViewLogItem>();
            _ListSortDirection = _ListSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

            switch (header)
            {
                case "시간":
                    list.Sort(new Comparison<ListViewLogItem>((ListViewLogItem ll1, ListViewLogItem ll2) =>
                    {
                        return _ListSortDirection == ListSortDirection.Ascending ? ll1.Time.CompareTo(ll2.Time) : -ll1.Time.CompareTo(ll2.Time);
                    }));
                    break;
                case "값":
                    list.Sort(new Comparison<ListViewLogItem>((ListViewLogItem ll1, ListViewLogItem ll2) =>
                    {
                        return _ListSortDirection == ListSortDirection.Ascending ? ll1.Value.CompareTo(ll2.Value) : -ll1.Value.CompareTo(ll2.Value);
                    }));
                    break;
            }
            lv.Items.Clear();
            foreach (var v in list)
                lv.Items.Add(v);
            NRetLV.UpdateLayout();
        }
    }
}
