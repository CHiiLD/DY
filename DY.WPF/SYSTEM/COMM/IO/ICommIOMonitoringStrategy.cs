using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.NET;
using NLog;
using DY.WPF.SYSTEM.COMM;

namespace DY.WPF.SYSTEM.IO
{
    public abstract class ACommIOMonitoringStrategy
    {
        protected static Logger LOG = LogManager.GetCurrentClassLogger();

        public CommClient CClient { get; protected set; }
        protected List<IProtocol> Protocols = new List<IProtocol>();
        protected IList<ICommIOData> CommIODatas;
        private Task UpdateTask = null;
        private volatile bool m_Run = false;

        public int TransferInteval
        {
            get
            {
                return CClient.TransferInteval;
            }
        }

        public int ResponseLatencyTime
        {
            get
            {
                return CClient.ResponseLatencyTime;
            }
        }

        public bool IsLooped
        {
            get
            {
                return m_Run;
            }
        }

        /// <summary>
        /// ICommIOData 데이터들로 프로토콜 객체를 생성
        /// </summary>
        /// <param name="io_datas">데이터</param>
        public abstract void UpdateProtocols(IList<ICommIOData> io_datas);

        /// <summary>
        /// 응답 프로토콜을 받아, ICommIOData 컬렉션을 UI쓰레드안에서 업데이트한다.
        /// </summary>
        /// <param name="io_datas">업데이트할 대상</param>
        /// <returns></returns>
        public abstract Task UpdateIOAsync(IList<ICommIOData> io_datas);

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="cclient"></param>
        protected ACommIOMonitoringStrategy(CommClient cclient)
        {
            CClient = cclient;
        }

        /// <summary>
        /// 루프 온/오프
        /// </summary>
        /// <param name="value">스위치</param>
        /// <returns></returns>
        public async Task SetLoopAsync(bool value)
        {
            m_Run = value;
            if (m_Run)
            {
                if (CommIODatas == null)
                    throw new NullReferenceException("io_datas is null.");
                UpdateTask = Update();
            }
            else
            {
                if (UpdateTask != null)
                {
                    LOG.Debug(CClient.Summary + " 업데이트 루프 종료까지 대기 시작");
                    await UpdateTask;
                }
                UpdateTask = null;
            }
        }

        /// <summary>
        /// IO 데이터그리드 반복 업데이트
        /// </summary>
        private Task Update()
        {
            Task thread = Task.Factory.StartNew(new Action(async () =>
            {
                LOG.Debug(CClient.Summary + " 업데이트 루프 시작");
                while (m_Run)
                {
                    if (!CClient.Socket.IsConnected())
                    {
                        LOG.Debug(CClient.Summary + " 통신 접속 해제에 의한 업데이트 중단");
                        break;
                    }
                    Task task = UpdateIOAsync(CommIODatas);
                    //응답대기시간안에 응답이 온다면
                    if (await Task.WhenAny(task, Task.Delay(ResponseLatencyTime)) == task)
                        await Task.Delay(TransferInteval); //다음 루프까지 대기
                }
                m_Run = false;
                LOG.Debug(CClient.Summary + " 업데이트 루프 종료");
            }));
            return thread;
        }
    }
}
