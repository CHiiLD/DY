using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

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
        private bool m_Run;

        protected List<IProtocol> Protocols { get; set; }
        protected IList<ICommIOData> CommIOData { get; set; }

        public int TransferInteval { get { return CClient.IOUpdateInteval; } }
        public int ResponseLatencyTime { get { return CClient.WriteTimeout; } }
        public bool IsUpdated { get { return m_Run; } }

        public EventHandler<DeliveryArrivalEventArgs> DeliveryArrived;

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
            m_IOTimer.Stop();
        }

        public abstract void ReplaceICommIOData(IList<ICommIOData> io_datas);
        public abstract Task UpdateIOAsync(CancellationToken token);

        /// <summary>
        /// Update Thread Run/Stop
        /// </summary>
        /// <param name="isRun">스위치</param>
        /// <returns></returns>
        public void SetRunAsync(bool isRun)
        {
            m_Run = isRun;
            if (isRun)
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

        /// <summary>
        /// IO 데이터그리드 반복 업데이트
        /// </summary>
        private async void UpdateIOTick(object sender, EventArgs args)
        {
            m_IOTimer.Stop();

            CancellationTokenSource DelayTokenSource = new CancellationTokenSource();
            CancellationTokenSource.CreateLinkedTokenSource(DelayTokenSource.Token, m_UpdateTokenSource.Token);

            await UpdateIOAsync(m_UpdateTokenSource.Token);
            await Task.Delay(CClient.IOUpdateInteval, DelayTokenSource.Token);

            if (!m_UpdateTokenSource.IsCancellationRequested)
                m_IOTimer.Start();
        }
    }
}