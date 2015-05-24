using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Threading;

namespace DY.SAMPLE.LEAK_TESTER
{
    public partial class MainWindow : Window
    {
        private ItemCollection _ListViewItem;
        private WorkModelItemControl _WorkModelItemControl_L, _WorkModelItemControl_R;

        public MainWindow()
        {
            InitializeComponent();
            _ListViewItem = NTabItem_Work.NListViewControl.NView.Items;
            _WorkModelItemControl_L = NTabItem_Work.WorkModelControl_L;
            _WorkModelItemControl_R = NTabItem_Work.WorkModelControl_R;
        }

        private void NMIModel_Click(object sender, RoutedEventArgs e)
        {
            new ModelSettingWindow() { Owner = this }.ShowDialog();
        }

        private void NMISetup_Click(object sender, RoutedEventArgs e)
        {
            new CommSetupWindow() { Owner = this }.ShowDialog();
        }

        private void NMI_IO_Click(object sender, RoutedEventArgs e)
        {
            new IOTestWindow() { Owner = this }.ShowDialog();
        }

        public void OnLeakResultDateReceived(object sender, LeakResultReceivedEventArgs e)
        {
            if (this.Dispatcher.CheckAccess())
            {
                ModelItemFactory(e.Item);
            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ModelItemFactory(e.Item);
                }));
            }
        }

        private void ModelItemFactory(ModelItem item)
        {
            ModelItem modelItem = item;
            Model settedModel = null;
            SerialNumber settedSerialN = null;
            WorkModelItemControl control = null;
            switch (modelItem.LR)
            {
                case ModelItem.DIRECTION.L:
                    settedModel = _WorkModelItemControl_L.SelectedModel;
                    settedSerialN = _WorkModelItemControl_L.SelectedSerialNumber;
                    control = _WorkModelItemControl_L;
                    break;
                case ModelItem.DIRECTION.R:
                    settedModel = _WorkModelItemControl_R.SelectedModel;
                    settedSerialN = _WorkModelItemControl_R.SelectedSerialNumber;
                    control = _WorkModelItemControl_R;
                    break;
                default:
                    return;
            }
            if (settedModel == null)
                return;
            modelItem.Paste(settedModel);
            int idx = _ListViewItem.Count + 1;
            string model_number = (modelItem.LR == ModelItem.DIRECTION.L ? _WorkModelItemControl_L.NModelNum.SelectedValue : _WorkModelItemControl_R.NModelNum.SelectedValue) as string;
            string serial_number = "";
            string qr_code = "";
            if (modelItem.Result == ModelItem.RESULT.OK)
            {
                serial_number = ModelItem.AssembleSerialNumber(modelItem, settedSerialN);
                qr_code = ModelItem.AssembleQRCode(modelItem, serial_number);
            }
            modelItem.PullRestMember(idx, model_number, serial_number, qr_code);
            _ListViewItem.Add(new ModelItem(modelItem));
            ModelItemDirector.GetInstance().AddItem(new ModelItem(modelItem));
            control.SetModelItemInfo(modelItem);
            // 내리기
            var listview = NTabItem_Work.NListViewControl.NView;
            listview.ScrollIntoView(listview.Items[listview.Items.Count - 1]);
        }

        private void NMI_Init_Click(object sender, RoutedEventArgs e)
        {
            SerialNumberDirector.GetInstance().SerialNumbersInit();
        }

        private void NMI_Test_Click(object sender, RoutedEventArgs e)
        {
            _WorkModelItemControl_L.NModelNum.SelectedItem = _WorkModelItemControl_L.NModelNum.Items[0];
            _WorkModelItemControl_R.NModelNum.SelectedItem = _WorkModelItemControl_R.NModelNum.Items[0];

            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(00, 00, 00, 01, 00);
            Timer.Tick += (object sender1, EventArgs e1) => 
            {
                Random r = new Random();
                double leak = r.NextDouble();
                OnLeakResultDateReceived(null, new LeakResultReceivedEventArgs(new ModelItem(leak, leak < 0.75 ? ModelItem.RESULT.OK : ModelItem.RESULT.NG, ModelItem.DIRECTION.L))); 
            };
            Timer.Start();

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(00, 00, 00, 01, 500);
            Timer.Tick += (object sender1, EventArgs e1) =>
            {
                Random r = new Random();
                double leak = r.NextDouble();
                OnLeakResultDateReceived(null, new LeakResultReceivedEventArgs(new ModelItem(leak, leak < 0.75 ? ModelItem.RESULT.OK : ModelItem.RESULT.NG, ModelItem.DIRECTION.R))); 
            };
            Timer.Start();
        }

        private void NMI_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
