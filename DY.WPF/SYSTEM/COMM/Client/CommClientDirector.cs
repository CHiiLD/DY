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
using System.Windows.Threading;
using DY.WPF.SYSTEM.JSON;

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// ClientComm, 통신을 전체 통제하는 클래스
    /// </summary>
    public class CommClientDirector : IDisposable, IJson
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private static CommClientDirector THIS;

        /// 자동 연결 체크 타이머
        private DispatcherTimer m_ConnectionCheckTimer;

        /// 통신 설정과 관련된 프로퍼티들
        public NotifyPropertyChanged<int> ConnectionTimeoutProperty { get; private set; }
        public NotifyPropertyChanged<int> ConnectionCheckIntevalProperty { get; private set; }
        public NotifyPropertyChanged<bool> ConnectionCheckableProperty { get; private set; }

        /// 클라이언트 소켓 사전 
        public ObservableCollection<CommClient> Clientele
        {
            get;
            private set;
        }

        private CommClientDirector()
        {
            //collection
            Clientele = new ObservableCollection<CommClient>();
            //notify property initialize
            ConnectionTimeoutProperty = new NotifyPropertyChanged<int>(200);
            ConnectionCheckIntevalProperty = new NotifyPropertyChanged<int>(10000);
            ConnectionCheckIntevalProperty.PropertyChanged += OnConnectionCheckIntevalPropertyChanged;
            ConnectionCheckableProperty = new NotifyPropertyChanged<bool>(false);
            ConnectionCheckableProperty.PropertyChanged += OnConnectionCheckablePropertyPropertyChanged;
            m_ConnectionCheckTimer = new DispatcherTimer(
                new TimeSpan(10000 * ConnectionCheckIntevalProperty.Source),
                DispatcherPriority.Normal,
                OnConnectionCheckTick,
                Application.Current.Dispatcher) { IsEnabled = false };
            ShotDownDirector.GetInstance().AddIDisposable(this);
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
            m_ConnectionCheckTimer.Stop();
            foreach (var c in Clientele)
                c.Dispose();
            Clientele = null;
            GC.SuppressFinalize(this);
            LOG.Debug("CommClientDirector 메모리 해제");
        }

        public void SaveToJson()
        {

        }

        public void LoadByJson()
        {

        }

        /// <summary>
        /// CommClientManagement 싱글톤 객체 포인터 반환
        /// </summary>
        /// <returns></returns>
        public static CommClientDirector GetInstance()
        {
            if (THIS == null)
            {
                if (THIS == null)
                    THIS = new CommClientDirector();
            }
            return THIS;
        }

        /// <summary>
        /// 클라이언트의 통신 연결의 타임아웃 시간이 변경된 경우 호출
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnConnectionCheckIntevalPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            m_ConnectionCheckTimer.Interval = new TimeSpan(ConnectionCheckIntevalProperty.Source * 10000);
            LOG.Trace("클라이언트 접속 상태 체크 타이머의 설정 시간: " + ConnectionCheckIntevalProperty.Source);
        }

        /// <summary>
        /// 클라이언트 통신 접속상태 체크 시스템의 on/off를 변경한 경우 호출 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnConnectionCheckablePropertyPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var ConnectionCheckableProperty = sender as NotifyPropertyChanged<bool>;
            if (ConnectionCheckableProperty.Source)
                m_ConnectionCheckTimer.Start();
            else
                m_ConnectionCheckTimer.Stop();
            LOG.Trace("클라이언트 접속상태 체크 타이머 활성화 여부: " + ConnectionCheckableProperty.Source);
        }

        /// <summary>
        /// 클라이언트 통신 접속상태 체크 타이머
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectionCheckTick(object sender, EventArgs e)
        {
            bool isConnected = false;
            IConnect iConnect;
            CommunicationType commType;
            foreach (var commClient in Clientele)
            {
                if (commClient.Usable != true)
                    continue;
                iConnect = commClient.Socket;
                commType = commClient.CommType;
                try
                {
                    if (!iConnect.IsConnected())
                        isConnected = iConnect.Connect();
                }
                catch (Exception ex)
                {
                    LOG.Error("클라이언트 접속상태 체크 타이머 작동 중, 접속시도에러: " + ex.Message);
                    isConnected = false;
                }
                commClient.ChangedCommStatus(isConnected);
            }
        }
    }
}