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
        public const string EXTRA_XGT_CNET_LOCALPORT = "LOCAL_PORT";

        #region PRIVATE VARIABLE
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private const int STATUS_CHECK_INTEVAL = 10000; //10초

        private Timer m_CommStatusCheckTimer = new Timer(STATUS_CHECK_INTEVAL);
        private DYDevice m_Target;
        private DYDeviceCommType m_CommType;
        private bool? m_Usable;
        private string m_Comment;
        private Geometry m_ImageData = CommStateAi.ConnectFailure.Data;
        private Brush m_ImageColor = CommStateAi.ConnectFailure.Fill;
        private string m_Summary;
        private int m_TransferInteval;
        private int m_ResponseLatencyTime;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        //___________________COMM_DATAGIRD______________________________________
        /// <summary>
        /// IConnect 객체
        /// </summary>
        public IConnect Socket { get; private set; }
        /// <summary>
        /// 통신 디바이스 
        /// </summary>
        public DYDevice Target { get { return m_Target; } set { m_Target = value; OnPropertyChanged("Target"); } }
        /// <summary>
        /// 통신 타입
        /// </summary>
        public DYDeviceCommType CommType { get { return m_CommType; } set { m_CommType = value; OnPropertyChanged("CommType"); } }
        public bool? Usable
        {
            get
            {
                return m_Usable;
            }
            set
            {
                m_Usable = value;
                if (value == true)
                    m_CommStatusCheckTimer.Start();
                else
                    m_CommStatusCheckTimer.Stop();
                OnPropertyChanged("Usable");
                LOG.Trace("CommClient Usable Property changed: " + value);
            }
        }
        /// <summary>
        /// 유저 코멘트
        /// </summary>
        public string Comment { get { return m_Comment; } set { m_Comment = value; OnPropertyChanged("Comment"); } }
        /// <summary>
        /// 연결 상태 이미지
        /// </summary>
        public Geometry ImageData { get { return m_ImageData; } set { m_ImageData = value; OnPropertyChanged("ImageData"); } }
        /// <summary>
        /// 연결 상태 이미지 컬러
        /// </summary>
        public Brush ImageColor { get { return m_ImageColor; } set { m_ImageColor = value; OnPropertyChanged("ImageColor"); } }
        /// <summary>
        /// 통신 옵션 요약
        /// </summary>
        public string Summary { get { return m_Summary; } set { m_Summary = value; OnPropertyChanged("Summary"); } }

        //___________________EXTRA______________________________________________
        /// <summary>
        /// UUID
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 기타 설정 사항 
        /// XGT: LOCAL PORT
        /// </summary>
        public Dictionary<string, object> ExtraData { get; set; }

        //___________________IO_MONITORING______________________________________
        /// <summary>
        /// 프로토콜 통신 간격
        /// </summary>
        public int TransferInteval { get { return m_TransferInteval; } set { m_TransferInteval = value; OnPropertyChanged("TransferInteval"); } }
        /// <summary>
        /// 요청프로토콜 대기 시간
        /// </summary>
        public int ResponseLatencyTime { get { return m_ResponseLatencyTime; } set { m_ResponseLatencyTime = value; OnPropertyChanged("ResponseLatencyTime"); } }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="device"></param>
        /// <param name="comm_type"></param>
        public CommClient(IConnect socket, DYDevice device, DYDeviceCommType comm_type)
        {
            Socket = socket;
            Target = device;
            m_CommType = comm_type;
            Socket.ConnectionStatusChanged += OnChangedConnectionStatus;
            m_CommStatusCheckTimer.Elapsed += OnElapsed;
            Key = Guid.NewGuid().ToString();

            ResponseLatencyTime = 1000;
            TransferInteval = 100;
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
            Socket.Dispose();
            GC.SuppressFinalize(this);
            LOG.Debug(Summary+ " CommClient 메모리 해제");
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
            ChangedCommStatus(Socket.IsConnected());
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