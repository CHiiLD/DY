using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
namespace DY.NET.DATALOGIC.MATRIX
{
    public class Maxtrix200
    {
        private SerialPort _SerialPort;
        public class Builder : ASerialPortBuilder
        {
            public Builder(string name, int baud)
                : base(name, baud)
            {
            }
            public override object Build()
            {
                var m200 = new Maxtrix200() { _SerialPort = new SerialPort(_PortName, _BaudRate, _Parity, _DataBits, _StopBits) };
                m200._SerialPort.DataReceived += m200.OnDataRecieve;
                m200._SerialPort.ErrorReceived += m200.OnSerialErrorReceived;
                return m200;
            }
        }

        private void OnDataRecieve(object sender, SerialDataReceivedEventArgs args)
        {

        }

        private void OnSerialErrorReceived(object sender, SerialErrorReceivedEventArgs args)
        {

        }
    }
}
