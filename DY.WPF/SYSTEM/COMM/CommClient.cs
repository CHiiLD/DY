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
using System.Windows.Media;

namespace DY.WPF.SYSTEM.COMM
{
    /// <summary>
    /// DY.NET 통신 객체 관리 클래스
    /// </summary>
    public class CommClient : IDisposable, INotifyPropertyChanged
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private const int STATUS_CHECK_INTEVAL = 10000; //10초
        private Timer m_CommStatusCheckTimer = new Timer(STATUS_CHECK_INTEVAL);
        public IConnect Client { get; private set; }
#if false
        private NotifyPropertyChanged<DYDevice> CommDeviceProperty { get; set; }
        private NotifyPropertyChanged<DYDeviceProtocolType> CommTypeProperty { get; set; }
        private NotifyPropertyChanged<bool> UsableProperty { get; set; }
        private NotifyPropertyChanged<string> UserCommentProperty { get; set; }
        private NotifyPropertyChanged<Geometry> ImageDataProperty { get; set; }
        private NotifyPropertyChanged<Brush> ImageColorProperty { get; set; }
        private NotifyPropertyChanged<string> SummaryProperty { get; set; }
        private NotifyPropertyChanged<string> KeyProperty { get; set; }

        public DYDevice Target { get { return CommDeviceProperty.Source; } set { CommDeviceProperty.Source = value; } }
        public DYDeviceProtocolType CommType { get { return CommTypeProperty.Source; } set { CommTypeProperty.Source = value; } }
        public bool Usable { get { return UsableProperty.Source; } set { UsableProperty.Source = value; } }
        public string Comment { get { return UserCommentProperty.Source; } set { UserCommentProperty.Source = value; } }
        public Geometry ImageData { get { return ImageDataProperty.Source; } set { ImageDataProperty.Source = value; } }
        public Brush ImageColor { get { return ImageColorProperty.Source; } set { ImageColorProperty.Source = value; } }
        public string Summary { get { return SummaryProperty.Source; } set { SummaryProperty.Source = value; } }
        public string Key { get { return KeyProperty.Source; } set { KeyProperty.Source = value; } }
#endif
        private DYDevice m_Target;
        public DYDeviceProtocolType m_CommType;
        public bool m_Usable;
        public string m_Comment;
        public Geometry m_ImageData = CommStateAi.ConnectFailure.Data;
        public Brush m_ImageColor = CommStateAi.ConnectFailure.Fill;
        public string m_Summary;
        public string m_Key;

        public DYDevice Target { get { return m_Target; } set { m_Target = value; OnPropertyChanged("Target"); } }
        public DYDeviceProtocolType CommType { get { return m_CommType; } set { m_CommType = value; OnPropertyChanged("CommType"); } }
        public bool Usable
        {
            get
            {
                return m_Usable;
            }
            set
            {
                m_Usable = value;
                if (value)
                    m_CommStatusCheckTimer.Start();
                else
                    m_CommStatusCheckTimer.Stop();
                OnPropertyChanged("Usable");
                LOG.Trace("CommClient Ssable Property changed: " + value);
            }
        }

        public string Comment { get { return m_Comment; } set { m_Comment = value; OnPropertyChanged("Comment"); } }
        public Geometry ImageData { get { return m_ImageData; } set { m_ImageData = value; OnPropertyChanged("ImageData"); } }
        public Brush ImageColor { get { return m_ImageColor; } set { m_ImageColor = value; OnPropertyChanged("ImageColor"); } }
        public string Summary { get { return m_Summary; } set { m_Summary = value; OnPropertyChanged("Summary"); } }
        public string Key { get { return m_Key; } set { m_Key = value; OnPropertyChanged("Key"); } }

        public CommClient(IConnect client, DYDevice device, DYDeviceProtocolType comm_type)
        {
#if false
            //프로퍼티 객체 초기화
            CommDeviceProperty = new NotifyPropertyChanged<DYDevice>(device);
            CommTypeProperty = new NotifyPropertyChanged<DYDeviceProtocolType>(type);
            UsableProperty = new NotifyPropertyChanged<bool>(false);
            UserCommentProperty = new NotifyPropertyChanged<string>();
            ImageDataProperty = new NotifyPropertyChanged<Geometry>(CommStateAi.ConnectFailure.Data);
            ImageColorProperty = new NotifyPropertyChanged<Brush>(CommStateAi.ConnectFailure.Fill);
            SummaryProperty = new NotifyPropertyChanged<string>();
            KeyProperty = new NotifyPropertyChanged<string>();


            Client = client;
            Client.ConnectionStatusChanged += OnConnectionStatusChanged;
            m_StatusCheckTimer.Elapsed += OnElapsed;
            UsableProperty.PropertyChanged += OnUsablePropertyChanged;
#endif
            Client = client;
            m_CommType = comm_type;
            Client.ConnectionStatusChanged += OnConnectionStatusChanged;
            m_CommStatusCheckTimer.Elapsed += OnElapsed;
        }

        ~CommClient()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_CommStatusCheckTimer.Dispose();
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
                m_CommStatusCheckTimer.Start();
            else
                m_CommStatusCheckTimer.Stop();
        }

        /// <summary>
        /// 주기적으로 통신의 연결상태를 체크
        /// </summary>
        /// <param name="sender">Timer</param>
        /// <param name="args"></param>
        private void OnElapsed(object sender, ElapsedEventArgs args)
        {
            ChangedCommStatus(Client.IsConnected());
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
                Path target_path = isConnected ? CommStateAi.Connected : CommStateAi.ConnectFailure;
                if (ImageData != target_path.Data)
                {
                    ImageData = target_path.Data;
                    ImageColor = target_path.Fill;
                }
            }), null);
        }
    }
}