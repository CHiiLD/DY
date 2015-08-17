﻿using System;
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

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// ClientComm, 통신을 전체 통제하는 클래스
    /// </summary>
    public class CommClientDirector : IDisposable
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private static CommClientDirector THIS;

        public const int BASIC_INIT_CONNECT_DELAY_TIME = 200;
        public const int BASIC_INIT_CONNECT_CHECK_INTEVAL = 10000;
        public const bool BASIC_INIT_CONNECT_CHECKABLE = false;

        private DispatcherTimer m_ConnectionCheckTimer;

        /// 통신 설정과 관련된 프로퍼티들
        public NotifyPropertyChanged<int> ConnectionDelayTimeProperty { get; private set; }
        public NotifyPropertyChanged<int> ConnectionCheckIntevalProperty { get; private set; }
        public NotifyPropertyChanged<bool> ConnectionCheckableProperty { get; private set; }

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
            //collection
            Clientele = new ObservableCollection<CommClient>();

            m_ConnectionCheckTimer = new DispatcherTimer(
                new TimeSpan(BASIC_INIT_CONNECT_CHECK_INTEVAL * 10000),
                DispatcherPriority.Normal,
                OnConnectionCheckTick,
                Application.Current.Dispatcher) { IsEnabled = false };

            //notify property initialize
            ConnectionDelayTimeProperty = new NotifyPropertyChanged<int>(BASIC_INIT_CONNECT_DELAY_TIME);

            ConnectionCheckIntevalProperty = new NotifyPropertyChanged<int>(BASIC_INIT_CONNECT_CHECK_INTEVAL);
            ConnectionCheckIntevalProperty.PropertyChanged += OnConnectionCheckIntevalPropertyChanged;

            ConnectionCheckableProperty = new NotifyPropertyChanged<bool>(BASIC_INIT_CONNECT_CHECKABLE);
            ConnectionCheckableProperty.PropertyChanged += OnConnectionCheckablePropertyPropertyChanged;

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
        /// 클라이언트의 통신 연결의 타임아웃 시간이 변경된 경우 호출
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnConnectionCheckIntevalPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            NotifyPropertyChanged<int> notifyProperty = sender as NotifyPropertyChanged<int>;
            m_ConnectionCheckTimer.Interval = new TimeSpan(notifyProperty.Source * 10000);
            LOG.Trace("클라이언트 접속 상태 체크 타이머의 설정 시간: " + notifyProperty.Source);
        }

        /// <summary>
        /// 클라이언트 통신 접속상태 체크 시스템의 on/off를 변경한 경우 호출 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnConnectionCheckablePropertyPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var notifyProperty = sender as NotifyPropertyChanged<bool>;
            if (notifyProperty.Source)
                m_ConnectionCheckTimer.Start();
            else
                m_ConnectionCheckTimer.Stop();
            LOG.Trace("클라이언트 접속상태 체크 타이머 활성화 여부: " + notifyProperty.Source);
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
            DyNetCommType commType;
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