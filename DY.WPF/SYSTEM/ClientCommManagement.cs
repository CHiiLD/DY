using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Concurrent;
using DY.NET;

namespace DY.WPF.SYSTEM
{
    public class ClientCommManagement
    {
        private static ClientCommManagement m_Thiz;

        public int ResponseLatencyTime { get; set; } //응답대기시간
        public int RetryConnectingInteval { get; set; } //재연결간격 시간
        public bool IsRetryConnectng { get; set; } //재연결 시도
        
        private Timer m_RetryConnectingTimer = new Timer();

        public ConcurrentDictionary<string, ClientComm> Clientele
        {
            get;
            private set;
        }

        private ClientCommManagement()
        {
            Clientele = new ConcurrentDictionary<string, ClientComm>();
        }

        public static ClientCommManagement GetInstance()
        {
            if (m_Thiz == null)
                m_Thiz = new ClientCommManagement();
            return m_Thiz;
        }

        private async void OnElapse(object sender, ElapsedEventArgs args)
        {
            foreach (var commClient in Clientele)
            {
                var client = commClient.Value.Client;
                if (!client.IsConnected())
                {
                    var clientsync = client as IConnectAsync;
                    if (clientsync == null)
                    {
                        client.Connect();
                    }
                    else
                    {
                        Task task = clientsync.ConnectAsync();
                        if (Task.WhenAny(task, Task.Delay(ResponseLatencyTime)) == task)
                            await task;
                    }
                }
            }
        }

        public string CreateKey()
        {
            var guid = System.Guid.NewGuid();
            return guid.ToString();
        }

        public string SetClinet(IConnect client)
        {
            string key = CreateKey();
            Clientele.TryAdd(key, new ClientComm(client));
            return key;
        }

        public ClientComm GetClient(string key)
        {
            ClientComm ret = null;
            if (Clientele.ContainsKey(key))
                Clientele.TryGetValue(key, out ret);
            return ret;
        }
    }
}