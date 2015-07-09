using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Concurrent;

namespace DY.NET.DATALOGIC.MATRIX
{
    /// <summary>
    /// Matrix200과의 통신을 안정적으로 하기 위한 메세지 큐 랩핑 클래스
    /// </summary>
    public class Matrix200Queue
    {
        public enum Todo
        {
            PREPARE,
            DISCONNECT,
            SCAN,
            SYMBOL_VERIFICATION
        }

        private ConcurrentQueue<Tuple<Todo, EventHandler<Matrix200DataReceivedEventArgs>>> m_Queue = new ConcurrentQueue<Tuple<Todo, EventHandler<Matrix200DataReceivedEventArgs>>>();
        private bool m_Working = false;
        private Matrix200 m_m200;

        /// <summary>
        /// 코드를 스캔하고 난 뒤에 발생되는 이벤트
        /// </summary>
        public EventHandler<Matrix200DataReceivedEventArgs> CodeScanned { get; set; }

        public Matrix200Queue(Matrix200 m200)
        {
            m_m200 = m200;
        }

        /// <summary>
        /// 큐 객체를 비운다
        /// </summary>
        public void Clear()
        {
            Tuple<Todo, EventHandler<Matrix200DataReceivedEventArgs>> mail;
            while (m_Queue.TryDequeue(out mail)) { }
        }

        /// <summary>
        /// 큐 객체에 핸들러를 추가한다.
        /// </summary>
        /// <param name="todo"></param>
        /// <param name="callback">응답 메세지 도착을 알리는 콜백 메세지</param>
        public void Enqueue(Todo todo, EventHandler<Matrix200DataReceivedEventArgs> callback)
        {
            if (m_Working)
            {
                m_Queue.Enqueue(new Tuple<Todo, EventHandler<Matrix200DataReceivedEventArgs>>(todo, callback));
                return;
            }
            Work(todo, callback);
        }

        private async void Work(Todo todo, EventHandler<Matrix200DataReceivedEventArgs> callback)
        {
            m_Working = true;
            {
                Matrix200Code info = null;
                switch (todo)
                {
                    case Todo.PREPARE:
                        await m_m200.PrepareAsync();
                        break;
                    case Todo.DISCONNECT:
                        m_m200.Disconnect();
                        Clear();
                        break;
                    case Todo.SCAN:
                        await m_m200.CaptureAsync();
                        info = await m_m200.DecodingAsync();
                        if (CodeScanned != null)
                            CodeScanned(this, new Matrix200DataReceivedEventArgs(todo, info));
                        break;
                    case Todo.SYMBOL_VERIFICATION:
                        info = await m_m200.LearnBarCodeAsync();
                        break;
                }
                if (callback != null)
                    callback(this, new Matrix200DataReceivedEventArgs(todo, info));
            }
            m_Working = false;

            if (m_Queue.Count > 0)
            {
                Tuple<Todo, EventHandler<Matrix200DataReceivedEventArgs>> mail;
                if (m_Queue.TryDequeue(out mail))
                    Enqueue(mail.Item1, mail.Item2);
            }
        }
    }
}