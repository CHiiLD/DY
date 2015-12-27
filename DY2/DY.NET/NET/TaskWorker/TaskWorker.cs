using System;
using System.IO;

namespace DY.NET
{

    public class TaskWorker : ITaskWorker
    {
        public const int ENDLESS_LOOP = -1;
        int _standBy;
        int _breakTime;

        public EventHandler<EventArgs> WorkStarted { get; set; }
        public EventHandler<EventArgs> WorkFinished { get; set; }
        public EventHandler<ErrorEventArgs> ExceptionHappend { get; set; }

        public IAsyncCommand Work { get; set; }

        public int Repeat { get; set; }

        public int BreakTime
        {
            get
            {
                return _breakTime;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                _breakTime = value;
            }
        }

        public int StandBy
        {
            get
            {
                return _standBy;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                _standBy = value;
            }
        }

        public TaskWorker()
        {
            Repeat = ENDLESS_LOOP;
            BreakTime = 10;
            StandBy = 0;
        }
    }
}
