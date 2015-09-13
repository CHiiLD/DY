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
    public class CommClient : IDisposable, INotifyPropertyChanged, IJson
    {
        public const string EXTRA_XGT_CNET_LOCALPORT = "LOCAL_PORT";

        #region PRIVATE VARIABLE
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        private NetDevice m_Target;
        private CommunicationType m_CommType;
        private bool? m_Usable = false;
        private string m_Comment;
        private Geometry m_ImageData = CommStateAi.ConnectFailure.Data;
        private Brush m_ImageColor = CommStateAi.ConnectFailure.Fill;
        private string m_Summary;
        private int m_UpdateInteval;
        #endregion

        #region MAIN DATA
        public IConnect Socket { get; private set; }
        public NetDevice Target { get { return m_Target; } set { m_Target = value; OnPropertyChanged("Target"); } }
        public CommunicationType CommType { get { return m_CommType; } set { m_CommType = value; OnPropertyChanged("CommType"); } }
        #endregion

        #region COMMUNICATION STATE INFO DATA 
        public bool? Usable { get { return m_Usable; } set { m_Usable = value; OnPropertyChanged("Usable"); } }
        public string Comment { get { return m_Comment; } set { m_Comment = value; OnPropertyChanged("Comment"); } }
        public Geometry ImageData { get { return m_ImageData; } set { m_ImageData = value; OnPropertyChanged("ImageData"); } }
        public Brush ImageColor { get { return m_ImageColor; } set { m_ImageColor = value; OnPropertyChanged("ImageColor"); } }
        public string Summary { get { return m_Summary; } set { m_Summary = value; OnPropertyChanged("Summary"); } }
        #endregion

        #region EXTRA DATA
        public string UUID { get; set; }
        public Dictionary<string, object> ExtraData { get; set; }
        #endregion

        #region COMM DEVICE TEST DATA
        //___________________IO_MONITORING______________________________________
        /// <summary>
        /// 프로토콜 통신 간격
        /// </summary>
        public int UpdateInteval { get { return m_UpdateInteval; } set { m_UpdateInteval = value; OnPropertyChanged("UpdateInteval"); } }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="device"></param>
        /// <param name="comm_type"></param>
        public CommClient(IConnect socket, NetDevice device, CommunicationType comm_type)
        {
            Socket = socket;
            Target = device;
            m_CommType = comm_type;
            Socket.ConnectionStatusChanged += OnChangedConnectionStatus;
            UUID = Guid.NewGuid().ToString();
            UpdateInteval = 200;
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

        public void SaveToJson()
        {

        }

        public void LoadByJson()
        {

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