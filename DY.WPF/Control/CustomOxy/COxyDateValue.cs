using System;

namespace DY.WPF
{
    public struct COxyDateValue
    {
        private DateTime m_Date;
        private double m_Value;

        public DateTime Date
        {
            get
            {
                return m_Date;
            }
        }

        public double Value
        {
            get
            {
                return m_Value;
            }
        }

        public COxyDateValue(DateTime date, double value)
        {
            this.m_Date = date;
            this.m_Value = value;
        }

        public COxyDateValue(double value)
        {
            this.m_Date = DateTime.Now;
            this.m_Value = value;
        }
    }
}
