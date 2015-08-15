using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class CommClient : IDisposable, INotifyPropertyChanged, ITimeout
    {
        public const string EXTRA_XGT_CNET_LOCALPORT = "LOCAL_PORT";

        public static int UpdateIntevalMinimum { get { return 100; } }
        public static int UpdateIntevalMaximum { get { return 1000; } }
        public static int UpdateIntevalInit { get { return 1000; } }

        public static int ReadTimeoutMinimum { get { return 50; } }
        public static int ReadTimeoutMaximum { get { return 500; } }
        public static int ReadTimeoutInit { get { return 250; } }

        public static int WriteTimeoutMinimum { get { return 50; } }
        public static int WriteTimeoutMaximum { get { return 500; } }
        public static int WriteTimeoutInit { get { return 250; } }

        #region PRIVATE VARIABLE
        private static Logger LOG = LogManager.GetCurrentClassLogger();

        private DYDevice m_Target;
        private DYDeviceCommType m_CommType;
        private bool? m_Usable = false;
        private string m_Comment;
        private Geometry m_ImageData = CommStateAi.ConnectFailure.Data;
        private Brush m_ImageColor = CommStateAi.ConnectFailure.Fill;
        private string m_Summary;
        private int m_IOUpdateInteval;

        #endregion
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
                OnPropertyChanged("Usable");
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
        public int IOUpdateInteval { get { return m_IOUpdateInteval; } set { m_IOUpdateInteval = value; OnPropertyChanged("IOUpdateInteval"); } }
        public int WriteTimeout { get { return Socket.WriteTimeout; } set { Socket.WriteTimeout = value; OnPropertyChanged("WriteTimeout"); } }
        public int ReadTimeout { get { return Socket.ReadTimeout; } set { Socket.ReadTimeout = value; OnPropertyChanged("ReadTimeout"); } }

        public event PropertyChangedEventHandler PropertyChanged;

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
            Key = Guid.NewGuid().ToString();
            IOUpdateInteval = UpdateIntevalInit;
            WriteTimeout = ReadTimeoutInit;
            ReadTimeout = WriteTimeoutInit;
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
            Socket.Dispose();
            GC.SuppressFinalize(this);
            LOG.Debug(Summary + " CommClient 메모리 해제");
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
        public void ChangedCommStatus(bool isConnected)
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