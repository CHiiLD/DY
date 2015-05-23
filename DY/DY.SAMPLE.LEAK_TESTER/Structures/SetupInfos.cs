using System;
using System.IO.Ports;

namespace DY.SAMPLE.LEAK_TESTER
{
    public class EthernetInfo
    {
        public string PLC = "192.168.0.90";
        public string Printer = "192.168.0.9";

        public EthernetInfo()
        {

        }

        public EthernetInfo(EthernetInfo info)
        {
            PLC = info.PLC;
            Printer = info.Printer;
        }
    }

    public class SerialPortInfo
    {
        public string PortName = "COM<N>";
        public int BaudRate = 9600;
        public Parity Parity = Parity.None;
        public int DataBits = 8;
        public StopBits StopBits = StopBits.One;
        public ushort LocalPort = 00;

        public SerialPortInfo()
        {

        }

        public SerialPortInfo(SerialPortInfo info)
        {
            PortName = info.PortName;
            BaudRate = info.BaudRate;
            Parity = info.Parity;
            DataBits = info.DataBits;
            StopBits = info.StopBits;
            LocalPort = info.LocalPort;
        }
    }

    public class DurationTime
    {
        public TimeSpan BeginTime;
        public TimeSpan EndTime;

        public DurationTime()
        {

        }

        public DurationTime(DurationTime time)
        {
            BeginTime = time.BeginTime;
            EndTime = time.EndTime;
        }
    }

    public class DayNightTimeInfo
    {
        public DurationTime Day;
        public DurationTime Night;

        public DayNightTimeInfo(DayNightTimeInfo info)
        {
            Day = new DurationTime(info.Day);
            Night = new DurationTime(info.Night);
        }

        public DayNightTimeInfo()
        {
            Day = new DurationTime();
            Night = new DurationTime();

            Day.BeginTime = new TimeSpan(08, 00, 00);
            Day.EndTime = new TimeSpan(19, 59, 59);
            Night.BeginTime = new TimeSpan(20, 00, 00);
            Night.EndTime = new TimeSpan(7, 59, 59);
        }
    }
}
