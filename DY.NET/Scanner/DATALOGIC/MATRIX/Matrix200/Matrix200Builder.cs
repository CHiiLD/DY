using System.IO.Ports;

namespace DY.NET.DATALOGIC.MATRIX
{
    public partial class Matrix200
    {
        /// <summary>
        /// 빌더 패턴의 Matrix200 객체생성 클래스
        /// </summary>
        public class Builder
        {
            protected string _PortName;
            protected int _BaudRate = 115200;
            protected Parity _Parity = System.IO.Ports.Parity.None;
            protected int _DataBits = 8;
            protected StopBits _StopBits = System.IO.Ports.StopBits.One;

            public Builder(string name, int baud)
            {
                _PortName = name;
                _BaudRate = baud;
            }

            public Builder Parity(Parity parity)
            {
                _Parity = parity;
                return this;
            }

            public Builder DataBits(int databits)
            {
                _DataBits = databits;
                return this;
            }

            public Builder StopBits(StopBits stopbits)
            {
                _StopBits = stopbits;
                return this;
            }

            public Matrix200 Build()
            {
                var m200 = new Matrix200() { m_SerialPort = new SerialPort(_PortName, _BaudRate, _Parity, _DataBits, _StopBits) };
                return m200;
            }
        }
    }
}
