using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Timers;
using System.Windows.Shapes;
using System.Windows;
using DY.NET;
using NLog;

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// DY.NET 통신 객체 관리 클래스
    /// </summary>
    public class CommClient : IDisposable
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        private const int STATUS_CHECK_INTEVAL = 10000; //10초
        private Timer m_StatusCheckTimer = new Timer(STATUS_CHECK_INTEVAL);
        public IConnect Client { get; private set; }
        //public EventHandler<ConnectionStatusChangedEventArgs> ConnectionStateChanged { get; private set; }

        private NotifyPropertyChanged<DYDevice> CommDeviceProperty { get; set; }
        private NotifyPropertyChanged<DYDeviceProtocolType> CommTypeProperty { get; set; }
        private NotifyPropertyChanged<bool> UsableProperty { get; set; }
        private NotifyPropertyChanged<string> UserCommentProperty { get; set; }
        private NotifyPropertyChanged<Path> VectorImageProperty { get; set; }
        private NotifyPropertyChanged<string> SummaryProperty { get; set; }
        private NotifyPropertyChanged<string> KeyProperty { get; set; }

        public DYDevice Target { get { return CommDeviceProperty.Source; } set { CommDeviceProperty.Source = value; } }
        public DYDeviceProtocolType CommType { get { return CommTypeProperty.Source; } set { CommTypeProperty.Source = value; } }
        public bool Usable { get { return UsableProperty.Source; } set { UsableProperty.Source = value; } }
        public string Comment { get { return UserCommentProperty.Source; } set { UserCommentProperty.Source = value; } }
        public Path VectorImage { get { return VectorImageProperty.Source; } set { VectorImageProperty.Source = value; } }
        public string Summary { get { return SummaryProperty.Source; } set { SummaryProperty.Source = value; } }
        public string Key { get { return KeyProperty.Source; } set { KeyProperty.Source = value; } }

        public CommClient(IConnect client, DYDevice device, DYDeviceProtocolType type)
        {
            //프로퍼티 객체 초기화
            CommDeviceProperty = new NotifyPropertyChanged<DYDevice>(device);
            CommTypeProperty = new NotifyPropertyChanged<DYDeviceProtocolType>(type);
            UsableProperty = new NotifyPropertyChanged<bool>(false);
            UserCommentProperty = new NotifyPropertyChanged<string>();
            VectorImageProperty = new NotifyPropertyChanged<Path>(CommStateAi.ConnectFailure);
            SummaryProperty = new NotifyPropertyChanged<string>();
            KeyProperty = new NotifyPropertyChanged<string>();

            Client = client;
            Client.ConnectionStatusChanged += OnConnectionStatusChanged;
            m_StatusCheckTimer.Elapsed += OnElapsed;
            UsableProperty.PropertyChanged += OnUsablePropertyChanged;
        }

        ~CommClient()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_StatusCheckTimer.Dispose();
            Client.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// UsableProperty-PropertyChanged 이벤트 정의
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUsablePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var notify = sender as NotifyPropertyChanged<bool>;
            if (notify.Source)
                m_StatusCheckTimer.Start();
            else
                m_StatusCheckTimer.Stop();
        }

        /// <summary>
        /// 주기적으로 통신의 연결상태를 체크
        /// </summary>
        /// <param name="sender">Timer</param>
        /// <param name="args"></param>
        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            var isConnected = Client.IsConnected();
            ChangedCommStatus(isConnected);
        }

        /// <summary>
        /// DY.NET 통신 객체의 연결 상태 콜백 함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs args)
        {
            ChangedCommStatus(args.IsConnected);
        }

        /// <summary>
        /// 통신 상태에 따라 프로퍼티를 변경
        /// </summary>
        /// <param name="isConnected">연결 상태</param>
        private void ChangedCommStatus(bool isConnected)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var target_path = isConnected ? CommStateAi.Connected : CommStateAi.ConnectFailure;
                if (VectorImage != target_path)
                    VectorImage = target_path;
            }), null);
        }
    }
}