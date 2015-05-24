using System;

namespace DY.SAMPLE.LEAK_TESTER
{
    public class LeakResultReceivedEventArgs : EventArgs
    {
        public ModelItem Item { get; private set; }
        public LeakResultReceivedEventArgs(ModelItem item)
        {
            Item = new ModelItem(item);
        }
    }
}