using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace DY.NET.TEST
{
    [TestClass]
    public class TaskCompanyTest
    {
        class IntegerCountingCommand : IAsyncCommand
        {
            public int Count { get; set; }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public async Task ExecuteAsync(object parameter)
            {
                await Task.Delay(10);
                Count++;
            }
        }

        /// <summary>
        /// TaskCompany 클래스의 Employ 메서드가 정상적으로 작동하는지 확인한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Work()
        {
            bool start = false; bool end = false; IntegerCountingCommand command;
            TaskCompany twd = new TaskCompany();
            ITaskWorker worker = new TaskWorker();
            worker.Work = command = new IntegerCountingCommand();
            int repeat = worker.Repeat = 10;
            worker.WorkStarted += (object sender, EventArgs args) => { start = true; };
            worker.WorkFinished += (object sender, EventArgs args) => { end = true; };

            string key = twd.Employ(worker);
            await Task.Delay(1000);

            Assert.AreEqual(repeat, command.Count);
            Assert.IsTrue(start);
            Assert.IsTrue(end);
            ITaskWorker humen = twd.Search(key); //작업을 모두 끝냈음으로 이미 디렉터의 워킹리스트에서 제거함
            Assert.IsNull(humen);
        }

        /// <summary>
        /// 비동기적으로 실행되는 작업을 중단시키고 리스트에서 작업을 제거한다.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DismissInWorking()
        {
            bool end = false;
            IntegerCountingCommand command;
            TaskCompany twd = new TaskCompany();
            ITaskWorker worker = new TaskWorker();
            worker.Work = command = new IntegerCountingCommand();
            int repeat = worker.Repeat = 1000; //10초 + alpha 걸림
            worker.WorkFinished += (object sender, EventArgs args) => { end = true; };

            string key = twd.Employ(worker);
            await Task.Delay(1000); //1초만 기다림

            Assert.IsTrue(twd.Dismiss(key));
            await Task.Delay(100);
            Assert.IsNull(twd.Search(key));
            Assert.IsTrue(end);
            Assert.IsTrue(command.Count < repeat);
        }
    }
}