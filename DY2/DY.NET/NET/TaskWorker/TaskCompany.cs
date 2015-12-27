using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DY.NET
{
    public class TaskCompany
    {
        SortedDictionary<string, ITaskWorker> _tasks;

        public TaskCompany()
        {
            _tasks = new SortedDictionary<string, ITaskWorker>();
        }

        /// <summary>
        /// 새로운 ITaskWorker를 등록하고 예약된 작업을 실행합니다.
        /// </summary>
        /// <param name="worker"></param>
        /// <returns>worker에 대한 범용고유식별자, 이미 추가한 경우 null을 반환합니다.</returns>
        public string Employ(ITaskWorker worker)
        {
            if (worker == null || worker.Work == null)
                throw new ArgumentNullException();
            if (_tasks.ContainsValue(worker))
                return null;
            string id = Guid.NewGuid().ToString();
            _tasks.Add(id, worker);
            Workshop(id, worker); //비동기 수행
            return id;
        }

        public ITaskWorker Search(string id)
        {
            if (!_tasks.ContainsKey(id))
                return null;
            return _tasks[id];
        }

        public bool Dismiss(string id)
        {
            if (!_tasks.ContainsKey(id))
                return false;
            _tasks[id].Repeat = 0; //Loop를 정지
            return _tasks.Remove(id);
        }

        public async void Workshop(string id, ITaskWorker worker)
        {
            if (worker.StandBy > 0)
                await Task.Delay(worker.StandBy);
            if (worker.WorkStarted != null)
                worker.WorkStarted(worker, EventArgs.Empty);
            while (worker.Repeat > 0 || worker.Repeat == TaskWorker.ENDLESS_LOOP)
            {
                try
                {
                    if (worker.Work.CanExecute(worker))
                        await worker.Work.ExecuteAsync(worker);
                }
                catch (Exception exception)
                {
                    if (worker.ExceptionHappend != null)
                        worker.ExceptionHappend(worker, new ErrorEventArgs(exception));
                }
                finally
                {
                    if (worker.Repeat != TaskWorker.ENDLESS_LOOP)
                        worker.Repeat--;
                }
                if (worker.BreakTime > 0)
                    await Task.Delay(worker.BreakTime);
            }
            if (worker.WorkFinished != null)
                worker.WorkFinished(worker, EventArgs.Empty);
            Dismiss(id);
        }
    }
}
