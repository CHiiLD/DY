using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using DY.NET;
using NLog;

namespace DY.WPF.SYSTEM.COMM
{
    public abstract class ACommIOMonitoringStrategy
    {
        protected static Logger LOG = LogManager.GetCurrentClassLogger();

        public CommClient CClient { get; protected set; }

        private CancellationTokenSource m_UpdateTokenSource;
        private Task m_UpdateTask;
        private volatile bool m_Run;
        
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
        }

        public abstract void ReplaceICommIOData(IList<ICommIOData> io_datas);
        public abstract Task UpdateIOAsync();

        /// <summary>
        /// Update Thread Run/Stop
        /// </summary>
        /// <param name="isRun">스위치</param>
        /// <returns></returns>
        public async Task SetRunAsync(bool isRun)
        {
            m_Run = isRun;
            if (isRun)
            {
                if (CommIOData == null)
                    throw new NullReferenceException("io_datas is null.");
                m_UpdateTokenSource = new CancellationTokenSource();
                m_UpdateTask = Task.Factory.StartNew(new Action(Update), m_UpdateTokenSource.Token);
            }
            else
            {
                if (m_UpdateTokenSource != null)
                    m_UpdateTokenSource.Cancel();
                if (m_UpdateTask != null)
                    await m_UpdateTask;
                m_UpdateTask = null;
            }
        }

        /// <summary>
        /// IO 데이터그리드 반복 업데이트
        /// </summary>
        private async void Update()
        {
            LOG.Debug(CClient.Summary + " 업데이트 루프 시작");
            while (!m_UpdateTokenSource.Token.IsCancellationRequested)
            {
                await UpdateIOAsync();
                var DelayTokenSource = new CancellationTokenSource();
                CancellationTokenSource.CreateLinkedTokenSource(DelayTokenSource.Token, m_UpdateTokenSource.Token);
                await Task.Delay(CClient.IOUpdateInteval, DelayTokenSource.Token);
            }
            LOG.Debug(CClient.Summary + " 업데이트 루프 종료");
        }
    }
}