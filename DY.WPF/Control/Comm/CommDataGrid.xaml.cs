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

            if (isChecked == true)
            {
                int inteval = CommClientManagement.GetInstance().ResponseLatencyProperty.Source;
                bool isConnected = false;
                MetroWindow metro_win = Window.GetWindow(this) as MetroWindow;
                string title = "Communication connection";

                LOG.Trace("프로그레스 로드");
                //프로그레스 다이얼로그 호출
                ProgressDialogController progress = await metro_win.ShowProgressAsync(title,
                    "Please wait... until the connection is completed.", true, null);
                //작업 준비 시작
                LOG.Trace("Task task 준비");
                IConnect client = m_CurSelectedClient.Client;
                var client_asycn = client as IConnectAsync;
                Task<bool> task = null;
                if (client_asycn == null)
                    task = Task.Factory.StartNew(() => { return client.Connect(); });
                else
                    task = client_asycn.ConnectAsync();
                //프로그레스 다이얼로그 취소 버튼 보이기
                LOG.Trace("비동기 딜레이 설정, 딜레이: " + inteval);
                await Task.Delay(2000);
                LOG.Trace("비동기 작업 시작");
                if (await Task.WhenAny(task, Task.Delay(inteval)) == (Task)task)
                {
                    isConnected = await task;
                    //프로그레스 다이얼로그 소멸
                    await progress.CloseAsync();
                    if (isConnected)
                        await metro_win.ShowMessageAsync(title, "It was successfully connected.");
                    else
                        await metro_win.ShowMessageAsync(title, "Connection failed.");
                }
                else
                {
                    isConnected = false;
                    await progress.CloseAsync();
                    await metro_win.ShowMessageAsync(title, "Connection failed.(time out)");
                }
            }
            else if (isChecked == false)
            {
                m_CurSelectedClient.Client.Dispose();
            }
        }

        private void NDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            IList<DataGridCellInfo> info = e.AddedCells;
            var client = info[0].Item as CommClient;
            m_CurSelectedClient = client;
        }
    }
}