using System;

namespace DY.WPF.SYSTEM.COMM
{
    public interface ICommControlTowerTabItem : IDisposable
    {
        CommClient CClient { get; set; }
        /// <summary>
        /// TabItem으로 선택될 때 
        /// </summary>
        EventHandler<EventArgs> Selected { get; set; }

        /// <summary>
        /// 다른 TabItem을 선택할 때
        /// </summary>
        EventHandler<EventArgs> Unselected { get; set; }
    }
}