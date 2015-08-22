using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;

using System.Threading;
using System.Threading.Tasks;
using DY.NET;
using DY.WPF.WINDOW;
using DY.WPF.SYSTEM.COMM;
using System.Collections;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;

namespace DY.WPF
{
    /// <summary>
    /// CommDataGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommConnectionDataGrid : UserControl
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 현재 선택된 셀 아이템 정보 
        /// </summary>
        private CommClient m_CurSelectedCClient;
        public ObservableCollection<CommClient> Items { get { return CommClientDirector.GetInstance().Clientele; } }

        public CommConnectionDataGrid()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 디바이스와 통신을 종료한다.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private bool Disconnect(IConnect client)
        {
            if (client.IsConnected())
                client.Close();
            return client.IsConnected();
        }

        private bool TryConnection(IConnect client)
        {
            bool isSuccess = false;
            try
            {
                isSuccess = client.Connect();
            }
            catch (Exception e)
            {
                LOG.Debug("통신 접속 에러: " + e.Message);
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// 디바이스와 통신 접속을 시도한다. 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private async Task<bool> ConnectAsync(IConnect c)
        {
            if (c == null)
                throw new ArgumentNullException("client");
            if (c.IsConnected())
                return true;
            int inteval = CommClientDirector.GetInstance().ConnectionTimeoutProperty.Source;
            bool isConnected = false;
            string message = null;
            MetroWindow metro_win = Window.GetWindow(this) as MetroWindow;
            string title = "Communication connection";
            ProgressDialogController progress = await metro_win.ShowProgressAsync(title,
                "Please wait... until the connection is completed.", false, null);
            await Task.Delay(1000);
            LOG.Trace("통신 접속 시도");
            Task connect_task = Task.Factory.StartNew(() => { return TryConnection(c); });
            if (await Task.WhenAny(connect_task, Task.Delay(inteval)) == connect_task)
            {
                isConnected = await (Task<bool>)connect_task;
                message = isConnected ? "It was successfully connected." : "Connection failed.";
            }
            else
            {
                isConnected = false;
                message = "Connection failed.(time out)";
            }
            await progress.CloseAsync();
            await metro_win.ShowMessageAsync(title, message);
            LOG.Trace("시도 결과: " + message);
            return isConnected;
        }

        /// <summary>
        /// Use 체크박스 이벤트, 설정한 통신 객체와 접속시도, 접속해제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NCB_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            bool? isChecked = cb.IsChecked;
            bool isConnected;
            if (isChecked == true)
                isConnected = await ConnectAsync(m_CurSelectedCClient.Socket);
            else
                isConnected = Disconnect(m_CurSelectedCClient.Socket);

            if (!isConnected)
                cb.IsChecked = false;
        }

        /// <summary>
        /// 데이타 그리드 셀 포커스 이벤트
        /// 해당 셀의 CommClient 정보를 얻는다 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            IList<DataGridCellInfo> info = e.AddedCells;
            if (info.Count == 0)
            {
                m_CurSelectedCClient = null;
            }
            else
            {
                var client = info[0].Item as CommClient;
                m_CurSelectedCClient = client;
            }
        }

        /// <summary>
        /// 데이타 그리드 마우스 좌클릭 이벤트
        /// 클릭한 비주얼트리를 추적하여 ContextMenu를 호출한다
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var hit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
            DependencyObject cell = VisualTreeHelper.GetParent(hit.VisualHit);
            while (cell != null && !(cell is DataGridCell))
                cell = VisualTreeHelper.GetParent(cell);
            DataGridCell targetCell = cell as DataGridCell;
            //셀 클릭시 
            if (targetCell != null)
            {
                var info = new DataGridCellInfo(targetCell);
                var client = info.Item as CommClient;
                NMI_Add.IsEnabled = false;
                NMI_Remove.IsEnabled = true;
                if (client.Socket.IsConnected())
                {
                    NMI_Connect.IsEnabled = false;
                    NMI_Disconnect.IsEnabled = true;
                }
                else
                {
                    NMI_Connect.IsEnabled = true;
                    NMI_Disconnect.IsEnabled = false;
                }
                m_CurSelectedCClient = client;
            }
            else //셀 클릭이 아닐 시
            {
                NMI_Add.IsEnabled = true;
                NMI_Connect.IsEnabled = false;
                NMI_Disconnect.IsEnabled = false;
                NMI_Remove.IsEnabled = false;
            }
        }

        /// <summary>
        /// 목록에서 셀 추가
        /// CommDeviceSetWindow를 오픈하여 새로운 디바이스 추가0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NMI_Add_Click(object sender, RoutedEventArgs e)
        {
            new CommDeviceSetWindow() { Owner = Window.GetWindow(this) }.ShowDialog();
        }

        /// <summary>
        /// 접속 시도
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NMI_Connect_Click(object sender, RoutedEventArgs e)
        {
            m_CurSelectedCClient.Usable = await ConnectAsync(m_CurSelectedCClient.Socket);
        }

        /// <summary>
        /// 접속 해제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NMI_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            m_CurSelectedCClient.Usable = Disconnect(m_CurSelectedCClient.Socket);
        }

        /// <summary>
        /// 목록에서 해당 셀 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NMI_Remove_Click(object sender, RoutedEventArgs e)
        {
            Disconnect(m_CurSelectedCClient.Socket);
            CommClientDirector.GetInstance().Clientele.Remove(m_CurSelectedCClient);
        }
    }
}