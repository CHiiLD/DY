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
        #region PRIVATE VARIABLE
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private const int STATUS_CHECK_INTEVAL = 10000; //10초

        private Timer m_CommStatusCheckTimer = new Timer(STATUS_CHECK_INTEVAL);
        private DYDevice m_Target;
        private DYDeviceProtocolType m_CommType;
        private bool m_Usable;
        private string m_Comment;
        private Geometry m_ImageData = CommStateAi.ConnectFailure.Data;
        private Brush m_ImageColor = CommStateAi.ConnectFailure.Fill;
        private string m_Summary;
        private string m_Key;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public IConnect Client { get; private set; }
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
            Client = client;
            Target = device;
            m_CommType = comm_type;
            Client.ConnectionStatusChanged += OnChangedConnectionStatus;
            m_CommStatusCheckTimer.Elapsed += OnElapsed;
            Key = Guid.NewGuid().ToString();
        }

        ~CommClient()
        {
            Dispose();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            m_CommStatusCheckTimer.Dispose();
            Client.Dispose();
            GC.SuppressFinalize(this);
            LOG.Debug("CommClient 메모리 해제");
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
        private void OnChangedConnectionStatus(object sender, ConnectionStatusChangedEventArgs args)
        {
            ChangedCommStatus(args.IsConnected);
        }

        /// <summary>
        /// 통신 상태에 따라 프로퍼티를 변경
        /// </summary>
        /// <param name="isConnected">연결 상태</param>
        private void ChangedCommStatus(bool isConnected)
        {
            if (Application.Current == null)
                return;

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