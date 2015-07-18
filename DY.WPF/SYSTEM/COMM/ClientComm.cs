using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Timers;
using System.Windows.Shapes;
using DY.NET;

namespace DY.WPF.SYSTEM.COMM
{
    public class ClientComm : IDisposable
    {
        private const int STATUS_CHECK_INTEVAL = 2000;
        private Timer m_StatusCheckTimer = new Timer(STATUS_CHECK_INTEVAL);

        public IConnect Client { get; private set; }

        public EventHandler<ClientStateChangedEventArgs> StateChanged { get; private set; }

        //public CommDevice Device { get; private set; }
        //public CommType DeviceType { get; private set; }
        //public bool Usable { get; set; }
        //public string UserComment { get; set; }
        //public Path VectorImage { get; set; }
        //public string Summary { get; set; }
        //public string Key { get; set; }

        /// <summary>
        /// 연결 / 비연결 상태 프로퍼티
        /// </summary>
        public NotifyPropertyChanged<bool> StatusProperty { get; private set; }

        /// <summary>
        /// CommDataGrid를 의식해서 만든 프로퍼티
        /// </summary>
        public NotifyPropertyChanged<CommDevice> CommDeviceProperty { get; private set; }
        public NotifyPropertyChanged<CommType> CommTypeProperty { get; private set; }
        public NotifyPropertyChanged<bool> CommUsableProperty { get; private set; }
        public NotifyPropertyChanged<string> CommUserCommentProperty { get; private set; }
        public NotifyPropertyChanged<Path> CommVectorImageProperty { get; private set; }
        public NotifyPropertyChanged<string> CommSummaryProperty { get; private set; }
        public NotifyPropertyChanged<string> KeyProperty { get; private set; }

        public CommDevice Target { get { return CommDeviceProperty.Source; } set { CommDeviceProperty.Source = value; } }
        public CommType CommType { get { return CommTypeProperty.Source; } set { CommTypeProperty.Source = value; } }
        public bool Usable { get { return CommUsableProperty.Source; } set { CommUsableProperty.Source = value; } }
        public string Comment { get { return CommUserCommentProperty.Source; } set { CommUserCommentProperty.Source = value; } }
        public Path VectorImage { get { return CommVectorImageProperty.Source; } set { CommVectorImageProperty.Source = value; } }
        public string Summary { get { return CommSummaryProperty.Source; } set { CommSummaryProperty.Source = value; } }
        public string Key { get { return KeyProperty.Source; } set { KeyProperty.Source = value; } }

        public ClientComm(IConnect client, CommDevice device, CommType type)
        {
            //프로퍼티 객체 초기화
            StatusProperty = new NotifyPropertyChanged<bool>();
            CommDeviceProperty = new NotifyPropertyChanged<CommDevice>(device);
            CommTypeProperty = new NotifyPropertyChanged<CommType>(type);
            CommUsableProperty = new NotifyPropertyChanged<bool>();
            CommUserCommentProperty = new NotifyPropertyChanged<string>();
            CommVectorImageProperty = new NotifyPropertyChanged<Path>();
            CommSummaryProperty = new NotifyPropertyChanged<string>();
            KeyProperty = new NotifyPropertyChanged<string>();

            //Device = device;
            //DeviceType = type;
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
            if (StatusProperty.Source != value)
                StatusProperty.Source = value;
        }

        private void OnConnectionStatusChanged(object sender, ClientStateChangedEventArgs args)
        {
            IConnect client = sender as IConnect;
            StatusProperty.Source = client.IsConnected(); //에러나는 예상 지점
        }
    }
}