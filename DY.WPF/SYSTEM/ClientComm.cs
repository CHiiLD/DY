using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Timers;
using System.Windows.Shapes;
using DY.NET;

namespace DY.WPF.SYSTEM
{
    public class ClientComm : IDisposable
    {
        private const int STATUS_CHECK_INTEVAL = 2000;
        private Timer m_StatusCheckTimer = new Timer(STATUS_CHECK_INTEVAL);

        public IConnect Client { get; private set; }

        public CommDevice   Device { get; private set; }
        public CommType     DeviceType { get; private set; }
        public bool         Usable { get; set; }
        public string       UserComment { get; set; }
        public Path         VectorImage { get; set; }
        public string       Summary { get; set; }
        public string       Key { get; set; }

        public EventHandler<ClientStateChangedEventArgs> StateChanged { get; private set; }
        public volatile NotifyPropertyChanged<bool> m_ConnectionStatusProperty = new NotifyPropertyChanged<bool>(false);
        public NotifyPropertyChanged<bool> StatusProperty
        {
            get
            {
                return m_ConnectionStatusProperty;
            }
        }

        public ClientComm(IConnect client, CommDevice device, CommType type)
        {
            Device = device;
            DeviceType = type;
            Client = client;
            StateChanged = OnConnectionStatusChanged;
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
            if (m_ConnectionStatusProperty.Source != value)
                m_ConnectionStatusProperty.Source = value;
        }

        private void OnConnectionStatusChanged(object sender, ClientStateChangedEventArgs args)
        {
            IConnect client = sender as IConnect;
            m_ConnectionStatusProperty.Source = client.IsConnected(); //에러나는 예상 지점
        }
    }
}