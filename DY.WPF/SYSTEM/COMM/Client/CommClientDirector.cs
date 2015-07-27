using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Concurrent;
using DY.NET;
using System.ComponentModel;
using System.Collections.ObjectModel;
using NLog;
using System.Windows;

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// ClientComm, 통신을 전체 통제하는 클래스
    /// </summary>
    public class CommClientDirector : IDisposable
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private static CommClientDirector THIS;

        private Timer m_TryConnectionTimer = new Timer();

        /// 통신 설정과 관련된 프로퍼티들
        public NotifyPropertyChanged<int> ResponseLatencyProperty { get; private set; }
        public NotifyPropertyChanged<int> ReconnectIntevalProperty { get; private set; }
        public NotifyPropertyChanged<bool> UsableReconnectProperty { get; private set; }

        /// <summary>
        /// 클라이언트 소켓 사전 
        /// </summary>
        public ObservableCollection<CommClient> Clientele
        {
            get;
            private set;
        }

        private CommClientDirector()
        {
            Clientele = new ObservableCollection<CommClient>();
            m_TryConnectionTimer.Elapsed += async (object sender, ElapsedEventArgs args) =>
            {
                await ConnectClientele();
            };

            ResponseLatencyProperty = new NotifyPropertyChanged<int>();
            ReconnectIntevalProperty = new NotifyPropertyChanged<int>();
            ReconnectIntevalProperty.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
            {
                var notifyProperty = sender as NotifyPropertyChanged<int>;
                m_TryConnectionTimer.Interval = notifyProperty.Source;
                LOG.Trace("재연결시도간격 :" + notifyProperty.Source);
            };

            UsableReconnectProperty = new NotifyPropertyChanged<bool>();
            UsableReconnectProperty.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
            {
                var notifyProperty = sender as NotifyPropertyChanged<bool>;
                if (notifyProperty.Source)
                    m_TryConnectionTimer.Start();
                else
                    m_TryConnectionTimer.Stop();
            };

            ResponseLatencyProperty.Source = 1000;
            ReconnectIntevalProperty.Source = 30000;
            UsableReconnectProperty.Source = false;
        }

        ~CommClientDirector()
        {
            Dispose();
        }

        /// <summary>
        /// Clientele 객체 메모리 해제
        /// </summary>
        public void Dispose()
        {
            m_TryConnectionTimer.Dispose();
            foreach (var c in Clientele)
                c.Dispose();
            Clientele = null;
            GC.SuppressFinalize(this);
            LOG.Debug("CommClientManagement 메모리 해제");
        }

        /// <summary>
        /// CommClientManagement 싱글톤 객체 포인터 반환
        /// </summary>
        /// <returns></returns>
        public static CommClientDirector GetInstance()
        {
            if (THIS == null)
                THIS = new CommClientDirector();
            return THIS;
        }

        /// <summary>
        /// UsableReconnectProperty가 True일 때 일정 간격마다 통신 접속 시도
        /// </summary>
        /// <returns></returns>
        private async Task ConnectClientele()
        {
            foreach (var commClient in Clientele)
            {
                IConnect socket = commClient.Socket;
                IConnectAsync connect_async = socket as IConnectAsync;
                bool isConnected = false;
                if (!socket.IsConnected())
                {
                    try
                    {
                        if (connect_async == null)
                            isConnected = socket.Connect();
                        else
                            isConnected = await connect_async.ConnectAsync();
                    }
                    catch (Exception ex)
                    {
                        LOG.Error("접속 에러: " + ex.Message);
                    }
                    commClient.Usable = isConnected;
                }
            }
        }
    }
}