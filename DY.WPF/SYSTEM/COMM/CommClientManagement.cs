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

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// ClientComm 객체와 통신을 전체 통제하는 클래스
    /// </summary>
    public class CommClientManagement : IDisposable
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        private static CommClientManagement m_Thiz;
        private Timer m_Timer = new Timer();

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
            m_Timer.Elapsed += OnElapse;

            ResponseLatencyProperty = new NotifyPropertyChanged<int>();
            ReconnectIntevalProperty = new NotifyPropertyChanged<int>();
            ReconnectIntevalProperty.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
            {
                var notifyProperty = sender as NotifyPropertyChanged<int>;
                m_Timer.Interval = notifyProperty.Source;
            };

            UsableReconnectProperty = new NotifyPropertyChanged<bool>();
            UsableReconnectProperty.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
            {
                var notifyProperty = sender as NotifyPropertyChanged<bool>;
                if (notifyProperty.Source)
                    m_Timer.Start();
                else
                    m_Timer.Stop();
            };

            ResponseLatencyProperty.Source = 1000;
            ReconnectIntevalProperty.Source = 30000;
            UsableReconnectProperty.Source = false;
        }

        ~CommClientManagement()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_Timer.Dispose();
            foreach (var c in Clientele)
                c.Value.Client.Dispose();
            Clientele.Clear();
            GC.SuppressFinalize(this);
        }

        public static CommClientManagement GetInstance()
        {
            if (m_Thiz == null)
                m_Thiz = new CommClientManagement();
            return m_Thiz;
        }

#if false
        /// <summary>
        /// Clientele에서 IConnectAsync를 상속한 클래스는 ConnectAsync()를 호출한다
        /// </summary>
        /// <param name="client"></param>
        public async void ConnectClientAsync(object iconnect)
        {
            var client_async = iconnect as IConnectAsync;
            try
            {
                Task task = client_async.ConnectAsync();
                if (Task.WhenAny(task, Task.Delay(ResponseLatencyProperty.Source)) == task)
                    await task;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
#endif

        private string CreateKey()
        {
            var guid = System.Guid.NewGuid();
            return guid.ToString();
        }

        public string SetClinet(CommClient clientComm)
        {
            string key = CreateKey();
            Clientele.Add(key, clientComm);
            return key;
        }

        public CommClient GetClient(string key)
        {
            CommClient ret = null;
            if (Clientele.ContainsKey(key))
                Clientele.TryGetValue(key, out ret);
            return ret;
        }

        /// <summary>
        /// UsableReconnectProperty가 On일 때 ReconnectIntevalProperty에 의해 자동적으로 IConnect 객체에 재접속을 시도
        /// </summary>
        /// <param name="sender">Timer</param>
        /// <param name="args">ElapsedEventArgs</param>
        private void OnElapse(object sender, ElapsedEventArgs args)
        {
            foreach (var client in Clientele)
            {
                if (!client.Value.Usable)
                    continue;
                var iconn = client.Value.Client;
                if (!iconn.IsConnected())
                {
                    try
                    {
                        iconn.Connect();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("클라이언트 연결 실패" + ex.Message);
                    }
                }
            }
        }
    }
}