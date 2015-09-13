using System.IO.Ports;

namespace DY.NET.HONEYWELL.VUQUEST
{
    public partial class Vuquest3310g
    {
        public class Builder : ASerialPortBuilder
        {
            public Builder(string name, int baud)
                : base(name, baud)
            {
            }

            public override object Build()
            {
                var v3310g = new Vuquest3310g(new SerialPort(PortName, BaudRate, ParityBit, DataBit, StopBit));
                return v3310g;
            }
        }
    }
}