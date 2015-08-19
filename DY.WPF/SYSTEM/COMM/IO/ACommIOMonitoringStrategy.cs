using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;

using DY.NET;
using NLog;

namespace DY.WPF.SYSTEM.COMM
{
    public abstract class ACommIOMonitoringStrategy
    {
        protected static Logger LOG = LogManager.GetCurrentClassLogger();

        public CommClient CClient { get; protected set; }

        private CancellationTokenSource m_UpdateTokenSource;
        private DispatcherTimer m_IOTimer;
        private bool m_Activated;
        public EventHandler<DeliveryArrivalEventArgs> DeliveryArrived;
        protected List<IProtocol> Protocols { get; set; }
        protected IList<ICommIOData> CommIOData { get; set; }
        public bool Activated
        { 
            get 
            { 
                return m_Activated; 
            }
            set
            {
                m_Activated = value;
                if(value)
                {
                    if (CommIOData == null)
                        throw new NullReferenceException("io_datas is null.");
                    m_UpdateTokenSource = new CancellationTokenSource();
                    m_IOTimer.Start();
                }
                else
                {
                    if (m_UpdateTokenSource != null)
                        m_UpdateTokenSource.Cancel();
                }
            }
        }
        public abstract void ReplaceICommIOData(IList<ICommIOData> io_datas);
        public abstract Task UpdateIOAsync(CancellationTokenSource cts);
        protected abstract void OnInputPropertyChanged(object sender, PropertyChangedEventArgs args);

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="cclient"></param>
        protected ACommIOMonitoringStrategy(CommClient cclient)
        {
            CClient = cclient;
            Protocols = new List<IProtocol>();
            m_IOTimer = new DispatcherTimer(
                new TimeSpan(1 * 10000), 
                DispatcherPriority.Normal,
                UpdateIOTick, 
                Application.Current.Dispatcher) { IsEnabled = false };
        }

        /// <summary>
        /// IO 데이터그리드 반복 업데이트
        /// </summary>
        private async void UpdateIOTick(object sender, EventArgs args)
        {
            m_IOTimer.Stop();

            var DelayToken = new CancellationTokenSource().Token;
            var UpdateToken = m_UpdateTokenSource.Token;
            CancellationTokenSource.CreateLinkedTokenSource(DelayToken, UpdateToken);

            await UpdateIOAsync(m_UpdateTokenSource);
            await Task.Delay(CClient.IOUpdateInteval, DelayToken);

            if (!m_UpdateTokenSource.IsCancellationRequested)
                m_IOTimer.Start();
        }
    }
}