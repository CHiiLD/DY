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
using NET.Tools;
using NLog;
using System.Windows;

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// ClientComm, 통신을 전체 통제하는 클래스
    /// </summary>
    public class CommClientManagement : IDisposable
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private static CommClientManagement THIS;

        private Timer m_TryConnectionTimer = new Timer();
        /// 통신 설정과 관련된 프로퍼티들
        public NotifyPropertyChanged<int> ResponseLatencyProperty { get; private set; }
        public NotifyPropertyChanged<int> ReconnectIntevalProperty { get; private set; }
        public NotifyPropertyChanged<bool> UsableReconnectProperty { get; private set; }

        /// <summary>
        /// 클라이언트 소켓 사전 
        /// </summary>
        public ObservableDictionary<string, CommClient> Clientele
        {
            get;
            private set;
        }

        private CommClientManagement()
        {
            Clientele = new ObservableDictionary<string, CommClient>();
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
            ReconnectIntevalProperty.Source = 5000;
            UsableReconnectProperty.Source = false;
        }

        ~CommClientManagement()
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
                c.Value.Client.Dispose();
            Clientele.Clear();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// CommClientManagement 싱글톤 객체 포인터 반환
        /// </summary>
        /// <returns></returns>
        public static CommClientManagement GetInstance()
        {
            if (THIS == null)
                THIS = new CommClientManagement();
            return THIS;
        }

        /// <summary>
        /// Dictionary에 사용할 키 생성
        /// </summary>
        /// <returns></returns>
        private string CreateKey()
        {
            var guid = System.Guid.NewGuid();
            return guid.ToString();
        }

        /// <summary>
        /// Clientele에 CommClient객체를 추가
        /// </summary>
        /// <param name="clientComm"></param>
        /// <returns></returns>
        public string SetClinet(CommClient clientComm)
        {
            string key = CreateKey();
            Clientele.Add(key, clientComm);
            return key;
        }

        /// <summary>
        /// 키 값으로 Clientele에서 CommClient객체를 찾아 반환
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CommClient GetClient(string key)
        {
            CommClient ret = null;
            if (Clientele.ContainsKey(key))
                Clientele.TryGetValue(key, out ret);
            return ret;
        }

        /// <summary>
        /// UsableReconnectProperty가 True일 때 일정 간격마다 통신 접속 시도
        /// </summary>
        /// <returns></returns>
        private async Task ConnectClientele()
        {
            foreach (var client in Clientele)
            {
                if (!client.Value.Usable)
                    continue;
                IConnect iconn = client.Value.Client;
                if (!iconn.IsConnected())
                {
                    try
                    {
                        var iconn_async = iconn as IConnectAsync;
                        if (iconn_async != null) //페러렐 비동기(tcp client)
                        {
                            Task task = iconn_async.ConnectAsync();
                            if (task == Task.WhenAny(task, Task.Delay(ResponseLatencyProperty.Source)))
                                await task;
                        }
                        else //동기(serialport comm)
                        {
                            iconn.Connect();
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.Error("통신 디바이스 접속 중 에러 발생: " + ex.Message);
                    }
                }
            }
        }
    }
}