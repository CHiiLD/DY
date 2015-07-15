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
        public EventHandler<ClientStateChangedEventArgs> StateChanged { get; private set; }
        public NotifyPropertyChanged<bool> ConnectionStatusNotifyProperty
        {
            get;
            private set;
        }

        public ClientComm(IConnect client)
        {
            Client = client;
            StateChanged = OnConnectionStatusChanged;
            ConnectionStatusNotifyProperty = new NotifyPropertyChanged<bool>(false);
            m_StatusCheckTimer.Elapsed += OnElapsed;
            m_StatusCheckTimer.Start();
        }

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
            if (ConnectionStatusNotifyProperty.Source != value)
                ConnectionStatusNotifyProperty.Source = value;
        }

        private void OnConnectionStatusChanged(object sender, ClientStateChangedEventArgs args)
        {
            IConnect icnn = sender as IConnect;
            ConnectionStatusNotifyProperty.Source = icnn.IsConnected();
        }
    }
}