using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    /// <summary>
    /// 간격
    /// 행동 제어
    /// 통신
    /// Task를 활용한 멀티스레드 통신
    /// 객체의 정지와 실행의 자유로움
    /// 
    /// Dictionary<IProtocolStream, Actions> Streams
    /// 
    /// class Actions {
    /// IList<Action> actions;
    /// 
    /// bool m_Continue;
    /// 
    /// bool Run
    /// {
    ///     get { return m_Continue; } 
    /// }
    /// 
    /// Work(IProtocolStream stream)
    /// {
    ///     while(m_Continue)
    ///     {
    ///         foreach(var a in actions)
    ///         {
    ///             if(!m_Continue) break;
    ///             await a.DoAsync(this, stream);
    ///         }
    ///     }
    /// }
    /// 
    /// LeaveTheOffice()
    /// {
    ///     m_Continue = false;
    /// }
    /// }
    /// 
    /// 액션의 플로우차트
    /// 읽기 -> 행동 부합 검사(Func delegate) -> NO -.
    ///   ^------------------------------------------:
    /// YES -> 위 검사가 필요한 만큼 실행 -> 액션 종료 
    /// -> 나온 결과를 원하는 목표에 적용하기
    /// 
    /// ActionBase class
    /// abstract Task 
    /// 
    /// 
    /// </summary>
    public class Workspace
    {
        private Dictionary<IProtocolStream, Work> streams;

    }

    public class Work
    {
        private IList<Action> m_Actions;
        private bool m_Continue;
        private bool m_Run;

        public bool IsRun
        {
            get
            {
                return m_Run;
            }
        }

        public Work()
        {
            m_Actions = new List<Action>();
            m_Continue = false;
            m_Run = false;
        }

        public void Stop()
        {
            m_Continue = false;
        }

        private async Task WorkingAysnc(IProtocolStream stream)
        {
            m_Continue = m_Run = true;
            while (m_Continue)
            {
                foreach (var a in m_Actions)
                {
                    if (!m_Continue) 
                        break;
                    await a.DoAsync(this, stream);
                }
            }
            m_Run = false;
        }
    }

    public abstract class Action
    {
        public abstract Task DoAsync(Work actions, IProtocolStream stream);
    }
}