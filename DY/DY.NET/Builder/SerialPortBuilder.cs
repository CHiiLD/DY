using System.IO.Ports;

namespace DY.NET
{
    /// <summary>
    /// 시리얼포트 초기화를 위한 빌더 패턴 추상 클래스
    /// </summary>
    public abstract class SerialPortBuilder
    {
        protected string _PortName;
        protected int _BaudRate = 9600;
        protected Parity _Parity = System.IO.Ports.Parity.None;
        protected int _DataBits = 8;
        protected StopBits _StopBits = System.IO.Ports.StopBits.One;

        protected SerialPortBuilder(string name, int baud)
        {
            _PortName = name;
            _BaudRate = baud;
        }

        public SerialPortBuilder Parity(Parity parity)
        {
            _Parity = parity;
            return this;
        }

        public SerialPortBuilder DataBits(int databits)
        {
            _DataBits = databits;
            return this;
        }

        public SerialPortBuilder StopBits(StopBits stopbits)
        {
            _StopBits = stopbits;
            return this;
        }

        public abstract object Build();
    }
}
