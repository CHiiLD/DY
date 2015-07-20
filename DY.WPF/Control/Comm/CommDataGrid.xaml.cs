using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

using System.Threading.Tasks;
using DY.NET;
using DY.WPF.WINDOW;
using DY.WPF.SYSTEM.COMM;
using System.Collections.Concurrent;
using System.Collections;
using NET.Tools;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;

namespace DY.WPF
{
    /// <summary>
    /// CommDataGrid.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class CommDataGrid : UserControl
    {
        private CommClient m_CurSelectedClient;
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public ObservableDictionary<string, CommClient> ClientCommItems
        {
            get
            {
                return CommClientManagement.GetInstance().Clientele;
            }
        }

        public CommDataGrid()
        {
            this.InitializeComponent();
        }

        private void NMI_AddCommDevice_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new CommDeviceSetWindow() { Owner = Window.GetWindow(this) }.ShowDialog();
        }

        private async void NCB_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            bool? isChecked = cb.IsChecked;
            if (m_CurSelectedClient == null)
                throw new Exception();
            if (isChecked == null)
                return;
            //연결을 끊고자 할 때
            if (isChecked == false)
            {
                m_CurSelectedClient.Client.Dispose();
                return;
            }
            //연결을 하고자 할 때
            int inteval = CommClientManagement.GetInstance().ResponseLatencyProperty.Source;
            bool isConnected = false;
            string message = null;
            MetroWindow metro_win = Window.GetWindow(this) as MetroWindow;
            string title = "Communication connection";
            //프로그레스 온
            ProgressDialogController progress = await metro_win.ShowProgressAsync(title,
                "Please wait... until the connection is completed.", false, null);
            IConnect client = m_CurSelectedClient.Client;
            var client_asycn = client as IConnectAsync;
            Task<bool> task = null;
            if (client_asycn == null)
                task = Task.Factory.StartNew(() => { return client.Connect(); });
            else
                task = client_asycn.ConnectAsync();
            await Task.Delay(2000);
            //통신 연결 시도 ---
            Task whenTask = await Task.WhenAny(task, Task.Delay(inteval));
            if (whenTask == (Task)task)
            {
                isConnected = await task;
                await progress.CloseAsync();
                if (isConnected)
                    message = "It was successfully connected."; //접속 성공
                else
                    message = "Connection failed."; //접속 에러
            }
            else //타임 아웃
            {
                isConnected = false;
                message = "Connection failed.(time out)";
            }
            await progress.CloseAsync();
            await metro_win.ShowMessageAsync(title, message);
            if (!isConnected)
                cb.IsChecked = false;
        }

        private void NDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            IList<DataGridCellInfo> info = e.AddedCells;
            var client = info[0].Item as CommClient;
            m_CurSelectedClient = client;
        }

        private void NDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var hit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
            DependencyObject cell = VisualTreeHelper.GetParent(hit.VisualHit);
            while (cell != null && !(cell is DataGridCell)) 
                cell = VisualTreeHelper.GetParent(cell);
            DataGridCell targetCell = cell as DataGridCell;
            if(targetCell != null)
            {
                var info = new DataGridCellInfo(targetCell);
                var client = info.Item as CommClient;

            }
        }
    }
}