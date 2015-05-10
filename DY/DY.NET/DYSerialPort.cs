using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DY.NET
{
    /// <summary>
    /// SerialPort 상속 클래스. 
    /// 요청 응답 프로토콜 객체의 관리를 위해서 만들어짐
    /// </summary>
    public class DYSerialPort : SerialPort, ITag
    {
        public DYSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            Encoding = Encoding.ASCII;
        }

        public DYSerialPort() : base()
        {

        }

        public int Tag
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public object UserData
        {
            get;
            set;
        }

        public IProtocol ReqtProtocol
        {
            get;
            set;
        }

        public IProtocol RecvProtocol
        {
            get;
            set;
        }

        public void ProtocolClear()
        {
            ReqtProtocol = null;
            RecvProtocol = null;
        }
    }
}
