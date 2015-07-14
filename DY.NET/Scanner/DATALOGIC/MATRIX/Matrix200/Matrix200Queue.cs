using System;
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
            /// <summary>
            /// 사용하지 않는다.
            /// </summary>
            NONE,
            /// <summary>
            /// 디바이스와 통신 준비
            /// </summary>
            PREPARE,
            /// <summary>
            /// 디바이스와 통신 해제
            /// </summary>
            DISCONNECT,
            /// <summary>
            /// 스캔 및 코드 분석 시도 
            /// </summary>
            SCAN,
            /// <summary>
            /// 바코드 종류 인식 및 영구 저장
            /// </summary>
            CODE_VERIFICATION,
#if false
            SETUP,
            CODE_VERIFICATION_CANCEL
#endif
        }

        private ConcurrentQueue<Tuple<Todo, EventHandler<Matrix200EventArgs>>> m_Queue = new ConcurrentQueue<Tuple<Todo, EventHandler<Matrix200EventArgs>>>();
        private Matrix200 m_m200;
        private Todo m_CurTodo = Todo.NONE;

        /// <summary>
        /// 코드를 스캔하고 난 뒤에 발생되는 이벤트
        /// </summary>
        public EventHandler<Matrix200EventArgs> CodeScanned { get; set; }

        public Matrix200Queue(Matrix200 m200)
        {
            m_m200 = m200;
        }

        /// <summary>
        /// 큐 객체를 비운다
        /// </summary>
        public void Clear()
        {
            Tuple<Todo, EventHandler<Matrix200EventArgs>> mail;
            while (m_Queue.TryDequeue(out mail)) { }
        }

        /// <summary>
        /// 큐 객체에 핸들러를 추가한다.
        /// </summary>
        /// <param name="todo"></param>
        /// <param name="callback">응답 메세지 도착을 알리는 콜백 메세지</param>
        public void Enqueue(Todo todo, EventHandler<Matrix200EventArgs> callback)
        {
            if (todo == Todo.NONE)
                throw new ArgumentException("Todo.NONE is not used");
#if false
            /// 코드 식별 취소 명령을 보낼 때는 최우선 순위로 보낸다.
            if (todo == Todo.CODE_VERIFICATION_CANCEL)
            {
                if (m_CurTodo == Todo.CODE_VERIFICATION)
                    DirectWork(todo, callback);
                return;
            }
#endif
            if (todo == Todo.DISCONNECT)
                Clear();
            if (m_CurTodo != Todo.NONE)
            {
                m_Queue.Enqueue(new Tuple<Todo, EventHandler<Matrix200EventArgs>>(todo, callback));
                return;
            }
            Work(todo, callback);
        }

#if false
        private async void DirectWork(Todo todo, EventHandler<Matrix200DataReceivedEventArgs> callback)
        {
            await m_m200.CancelLearnCodeAsync();
            if (callback != null)
                callback(todo, new Matrix200DataReceivedEventArgs(todo, null));
        }
#endif

        private async void Work(Todo todo, EventHandler<Matrix200EventArgs> callback)
        {
            m_CurTodo = todo;
            Matrix200Code info = null;
            switch (todo)
            {
                case Todo.PREPARE:
                    await m_m200.PrepareAsync();
                    break;
                case Todo.DISCONNECT:
                    await m_m200.DisconnectAsync();
                    break;
                case Todo.SCAN:
                    info = await m_m200.ScanAsync() as Matrix200Code;
                    if (CodeScanned != null)
                        CodeScanned(this, new Matrix200EventArgs(todo, info));
                    break;
#if false
                case Todo.SETUP:
                    Console.WriteLine("OpenSetupAsync");
                    await m_m200.OpenSetupAsync();
                    Console.WriteLine("CaptureForSetupAsync");
                    await m_m200.CaptureForSetupAsync();
                    Console.WriteLine("SettingCodeForSetupAsync");
                    string symbol = await m_m200.SettingCodeForSetupAsync();
                    Console.WriteLine("SavePermenentForSetupAsync");
                    bool save_ok =  await m_m200.SavePermenentForSetupAsync();
                    Console.WriteLine("CloseSetupAsync");
                    await m_m200.CloseSetupAsync();
                    if (save_ok)
                        info = new Matrix200Code() { Symbology = symbol };
                    break;
#endif
                case Todo.CODE_VERIFICATION:
                    info = await m_m200.LearnCodeAsync();
                    break;
            }
            if (callback != null)
                callback(this, new Matrix200EventArgs(todo, info));
            m_CurTodo = Todo.NONE;

            if (m_Queue.Count > 0)
            {
                Tuple<Todo, EventHandler<Matrix200EventArgs>> mail;
                if (m_Queue.TryDequeue(out mail))
                    Enqueue(mail.Item1, mail.Item2);
            }
        }
    }
}