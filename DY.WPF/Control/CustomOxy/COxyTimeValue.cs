using System;
using OxyPlot;

namespace DY.WPF
{
    public struct COxyTimeValue : ICodeGenerating
    {
        private TimeSpan m_Time;
        private double m_Value;

        public TimeSpan Time
        {
            get
            {
                return m_Time;
            }
        }

        public double Value
        {
            get
            {
                return m_Value;
            }
        }

        public COxyTimeValue(TimeSpan time, double value)
        {
            this.m_Time = time;
            this.m_Value = value;
        }

        public string ToCode()
        {
            return string.Format("{0},{1}", Time.ToString(), Value);
        }
    }
}
