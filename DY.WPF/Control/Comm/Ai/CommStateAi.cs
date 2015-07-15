using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Shapes;
using System.Windows.Media;

namespace DY.WPF
{
    /// <summary>
    /// 통신 상태 표현을 위한 Vector Image Path 연결을 제공하는 클래스
    /// </summary>
    public static class CommStateAi
    {
        private static readonly Path CIRCLE_GREEN_CONNECTED = new AICommCheck().Path;
        public static Path Connected
        {
            get
            {
                return CIRCLE_GREEN_CONNECTED;
            }
        }

        private static readonly Path CIRCLE_RED_CONNECT_FAIL = new AICommCross().Path;
        public static Path ConnectFailure
        {
            get
            {
                return CIRCLE_RED_CONNECT_FAIL;
            }
        }

        private static readonly Path CIRCLE_GRAY_IDLE = new AICommCircle().Path;
        public static Path Idle
        {
            get
            {
                return CIRCLE_GRAY_IDLE;
            }
        }
    }
}
