using System;
using System.IO;

namespace DY.NET
{
    public interface ITaskWorker
    {
        EventHandler<EventArgs> WorkStarted { get; set; }
        EventHandler<EventArgs> WorkFinished { get; set; }
        EventHandler<ErrorEventArgs> ExceptionHappend { get; set; }

        IAsyncCommand Work { get; set; }
        int Repeat { get; set; }
        int BreakTime { get; set; }
        int StandBy { get; set; }
    }

}
