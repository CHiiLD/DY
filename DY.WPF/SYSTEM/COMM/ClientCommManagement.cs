using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Concurrent;
using DY.NET;
using System.ComponentModel;

namespace DY.WPF.SYSTEM.COMM
{
    public class ClientCommManagement : IDisposable
    {
        private static ClientCommManagement m_Thiz;
        private Timer m_Timer = new Timer();

        public NotifyPropertyChanged<int> ResponseLatencyProperty { get; private set; }
        public NotifyPropertyChanged<int> ReconnectIntevalProperty { get; private set; }
        public NotifyPropertyChanged<bool> UsableReconnectProperty { get; private set; }

        /// <summary>
        /// 클라이언트 소켓 사전 
        /// </summary>
        public ConcurrentDictionary<string, ClientComm> Clientele
        {
            get;
            private set;
        }

        private ClientCommManagement()
        {
            Clientele = new ConcurrentDictionary<string, ClientComm>();
            m_Timer.Elapsed += OnElapse;

            ResponseLatencyProperty = new NotifyPropertyChanged<int>(500);
            ReconnectIntevalProperty = new NotifyPropertyChanged<int>(5000);
            ReconnectIntevalProperty.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
            {
                var notifyProperty = sender as NotifyPropertyChanged<int>;
                m_Timer.Interval = notifyProperty.Source;
            };

            UsableReconnectProperty = new NotifyPropertyChanged<bool>(false);
            UsableReconnectProperty.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
            {
                var notifyProperty = sender as NotifyPropertyChanged<bool>;
                if (notifyProperty.Source)
                    m_Timer.Start();
                else
                    m_Timer.Stop();
            };
        }

        ~ClientCommManagement()
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

        public static ClientCommManagement GetInstance()
        {
            if (m_Thiz == null)
                m_Thiz = new ClientCommManagement();
            return m_Thiz;
        }

        public async void ConnectClientAsync(object client)
        {
            var client_async = client as IConnectAsync;
            Task task = client_async.ConnectAsync();
            if (Task.WhenAny(task, Task.Delay(ResponseLatencyProperty.Source)) == task)
                await task;
        }

        public string CreateKey()
        {
            var guid = System.Guid.NewGuid();
            return guid.ToString();
        }

        public string SetClinet(ClientComm clientComm)
        {
            string key = CreateKey();
            Clientele.TryAdd(key, clientComm);
            return key;
        }

        public ClientComm GetClient(string key)
        {
            ClientComm ret = null;
            if (Clientele.ContainsKey(key))
                Clientele.TryGetValue(key, out ret);
            return ret;
        }

        private void OnElapse(object sender, ElapsedEventArgs args)
        {
            foreach (var commClient in Clientele)
            {
                var client = commClient.Value.Client;
                if (!client.IsConnected())
                {
                    var clientsync = client as IConnectAsync;
                    if (clientsync == null)
                        client.Connect();
                    else
                        Task.Factory.StartNew(ConnectClientAsync, clientsync);
                }
            }
        }
    }
}