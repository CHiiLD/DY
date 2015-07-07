using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DY.NET.DATALOGIC.MATRIX
{
    public class Matrix200 : Matrix200Command
    {
        private volatile SerialPort m_SerialPort;

        public bool IsEnableSerial
        {
            get
            {
                if (m_SerialPort == null)
                    return false;
                return m_SerialPort.IsOpen;
            }
        }

        private Matrix200()
        {
        }

        public static Matrix200 CreateMaxtrix200HostMode(string com, int baudrate)
        {
            var instance = new Matrix200();
            instance.m_SerialPort = new SerialPort(com, baudrate);
            return instance;
        }

        public bool Connect()
        {
            if (m_SerialPort == null)
                return false;
            if (!m_SerialPort.IsOpen)
                m_SerialPort.Open();
            return m_SerialPort.IsOpen;
        }

        public void Close()
        {
            m_SerialPort.Close();
        }

        public void Dispose()
        {
            m_SerialPort.Dispose();
            m_SerialPort = null;
        }

        public async Task PrepareAsync()
        {
            await m_SerialPort.BaseStream.WriteAsync(CMD_VISISET_CONNECT, 0, CMD_VISISET_CONNECT.Length);
            byte[] buf = new byte[m_SerialPort.ReadBufferSize];
            List<byte> storage = new List<byte>();
            int buf_size = 0;
            do
            {
                buf_size = await m_SerialPort.BaseStream.ReadAsync(buf, 0, buf.Length);
                for (int i = 0; i < buf_size; i++)
                    storage.Add(buf.ElementAt(i));
                if (buf[buf_size - 1] == 0x8E)
                    break;
            } while (buf_size != 0);
#if DEBUG
            string ret = Encoding.ASCII.GetString(storage.ToArray());
            Console.WriteLine(ret + " " + storage.Last());
#endif
        }

        public void Disconnect()
        {
            if (IsEnableSerial)
                m_SerialPort.Write(CMD_VISISET_DISCONNECT, 0, CMD_VISISET_DISCONNECT.Length);
        }

        public void Capture()
        {
            if (IsEnableSerial)
                m_SerialPort.Write(CMD_VISISET_CAPTURE, 0, CMD_VISISET_CAPTURE.Length);
        }

        public void Decoding()
        {
            if (IsEnableSerial)
                m_SerialPort.Write(CMD_VISISET_DECODING, 0, CMD_VISISET_DECODING.Length);
        }
    }
}