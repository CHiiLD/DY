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

        protected List<IProtocol> Protocols { get; set; }
        protected IList<ICommIOData> CommIOData { get; set; }

        private CancellationTokenSource m_TokenSource;
        private Task m_UpdateTask;
        private volatile bool m_Run;

        public int TransferInteval { get { return CClient.TransferInteval; } }
        public int ResponseLatencyTime { get { return CClient.ResponseLatencyTime; } }
        public bool IsUpdated { get { return m_Run; } }

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
        public async Task SetLoopAsync(bool isRun)
        {
            m_Run = isRun;
            if (isRun)
            {
                if (CommIOData == null)
                    throw new NullReferenceException("io_datas is null.");
                m_TokenSource = new CancellationTokenSource();
                m_UpdateTask = Update(m_TokenSource.Token);
            }
            else
            {
                m_TokenSource.Cancel();
                if (m_UpdateTask != null)
                    await m_UpdateTask;
                m_UpdateTask = null;
            }
        }

        /// <summary>
        /// IO 데이터그리드 반복 업데이트
        /// </summary>
        private Task Update(CancellationToken token)
        {
            Task thread = Task.Factory.StartNew(new Action(async () =>
            {
                LOG.Debug(CClient.Summary + " 업데이트 루프 시작");
                while (!token.IsCancellationRequested)
                    await UpdateIOAsync();
                LOG.Debug(CClient.Summary + " 업데이트 루프 종료");
            }), token);
            return thread;
        }
    }
}