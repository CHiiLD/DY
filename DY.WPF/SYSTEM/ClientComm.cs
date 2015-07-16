using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Timers;
using DY.NET;

namespace DY.WPF.SYSTEM
{
    public class ClientComm : IDisposable
    {
        private const int STATUS_CHECK_INTEVAL = 2000;
        private Timer m_StatusCheckTimer = new Timer(STATUS_CHECK_INTEVAL);

        public IConnect Client { get; internal set; }
        public CommDevice Device { get; internal set; }
        public CommType Type { get; internal set; }

        public EventHandler<ClientStateChangedEventArgs> StateChanged { get; private set; }
        public volatile NotifyPropertyChanged<bool> m_ConnectionStatusNotifyProperty = new NotifyPropertyChanged<bool>(false);
        public NotifyPropertyChanged<bool> StatusNotifyProperty
        {
            get
            {
                return m_ConnectionStatusNotifyProperty;
            }
        }

        public ClientComm(IConnect client, CommDevice device, CommType type)
        {
            Device = device;
            Type = type;
            Client = client;
            StateChanged = OnConnectionStatusChanged;
            m_StatusCheckTimer.Elapsed += OnElapsed;
            m_StatusCheckTimer.Start();
        }

        //public ClientComm(IConnect client)
        //{
        //    Client = client;
        //    StateChanged = OnConnectionStatusChanged;
        //    m_StatusCheckTimer.Elapsed += OnElapsed;
        //    m_StatusCheckTimer.Start();
        //}

        ~ClientComm()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_StatusCheckTimer.Dispose();
            Client.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            var value = Client.IsConnected();
            if (m_ConnectionStatusNotifyProperty.Source != value)
                m_ConnectionStatusNotifyProperty.Source = value;
        }

        private void OnConnectionStatusChanged(object sender, ClientStateChangedEventArgs args)
        {
            IConnect client = sender as IConnect;
            m_ConnectionStatusNotifyProperty.Source = client.IsConnected(); //에러나는 예상 지점
        }
    }
}