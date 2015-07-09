using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace DY.NET.DATALOGIC.MATRIX
{
    /// <summary>
    /// Matrix200 바코드 리더기를 기준으로 만들어진 디바이스 통신 클래스
    /// Matrix210과 함께 Matrix 시리즈와 호환이 될 것으로 예상 (테스트는 안해봄)
    /// </summary>
    public class Matrix200 : Matrix200Command
    {
        private volatile SerialPort m_SerialPort;
        private byte[] m_Buffer;

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

        /// <summary>
        /// 시리얼 포트에 접속
        /// </summary>
        /// <returns>접속 여부</returns>
        public bool Connect()
        {
            if (m_SerialPort == null)
                return false;
            if (!m_SerialPort.IsOpen)
            {
                m_SerialPort.Open();
                m_Buffer = new byte[m_SerialPort.ReadBufferSize];
            }
            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// 리더기에 종료 신호를 보낸 뒤
        /// 시리얼통신을 종료
        /// </summary>
        public void Close()
        {
            if(IsEnableSerial)
            {
                Disconnect();
                m_SerialPort.Close();
            }
        }

        /// <summary>
        /// 시리얼포트 객체의 자원을 소멸
        /// </summary>
        public void Dispose()
        {
            m_SerialPort.Dispose();
            m_SerialPort = null;
        }

        /// <summary>
        /// 리더기에 접속
        /// 응답이 오기 까지 대략 1초 정도 걸림
        /// </summary>
        /// <returns></returns>
        public async Task PrepareAsync()
        {
            if (!IsEnableSerial)
                return;
            await m_SerialPort.BaseStream.WriteAsync(CMD_VISISET_CONNECT, 0, CMD_VISISET_CONNECT.Length);
            int buf_size = 0;
            do
            {
                buf_size = await m_SerialPort.BaseStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
                if (m_Buffer[buf_size - 1] == 0x8E)
                    break;
            } while (buf_size != 0);
        }

        /// <summary>
        /// 리더기에 연결 종료 메세지 전달 
        /// </summary>
        public void Disconnect()
        {
            if (!IsEnableSerial)
                m_SerialPort.Write(CMD_VISISET_DISCONNECT, 0, CMD_VISISET_DISCONNECT.Length);
        }

        /// <summary>
        /// 바코드의 사진을 촬영하여 리더기의 메모리에 저장 
        /// </summary>
        /// <returns>촬영된 이미지의 가로, 세로 길이</returns>
        public async Task<Tuple<int, int>> CaptureAsync()
        {
            if (!IsEnableSerial)
                return null;

            await m_SerialPort.BaseStream.WriteAsync(CMD_VISISET_CAPTURE, 0, CMD_VISISET_CAPTURE.Length);
            List<byte> storage = new List<byte>();
            int buf_size = 0;
            do
            {
                buf_size = await m_SerialPort.BaseStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
                for (int i = 0; i < buf_size; i++)
                    storage.Add(m_Buffer[i]);
                if (storage.Count > 3)
                    if (storage[storage.Count - 3] == 0x03 &&
                        storage[storage.Count - 2] == 0x1A &&
                        storage[storage.Count - 1] == 0x57)
                        break;
            } while (buf_size != 0);

            string reply = Encoding.ASCII.GetString(storage.ToArray());
            Match match = Regex.Match(reply, @"\(([0-9]*)x([0-9]*)\)");
            if (match.Success)
            {
                string s_width = match.Groups[1].Value;
                string s_height = match.Groups[2].Value;
                int width, height;
                if (Int32.TryParse(s_width, out width) && Int32.TryParse(s_height, out height))
                {
                    Tuple<int, int> ret = new Tuple<int, int>(width, height);
                    return ret;
                }
            }
            return null;
        }

        /// <summary>
        /// 메모리에 저장된 이미지를 분석하여 바코드 코드 정보를 얻는다
        /// </summary>
        /// <returns>성공할 시 ProcessingInfo 리턴, 그렇지 않으면 null 리턴</returns>
        public async Task<Matrix200Code> DecodingAsync()
        {
            if (!IsEnableSerial)
                return null;

            await m_SerialPort.BaseStream.WriteAsync(CMD_VISISET_DECODING, 0, CMD_VISISET_DECODING.Length);
            List<byte> storage = new List<byte>();
            int buf_size = 0;
            do
            {
                buf_size = await m_SerialPort.BaseStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
                for (int i = 0; i < buf_size; i++)
                    storage.Add(m_Buffer.ElementAt(i));
                if (storage.Count > 3)
                    if (storage[storage.Count - 3] == 0x03 &&
                        storage[storage.Count - 2] == 0x03 &&
                        storage[storage.Count - 1] == 0x45)
                        break;
            } while (buf_size != 0);
            string reply = Encoding.ASCII.GetString(storage.ToArray());
            return ProcessReply(reply);
        }

        /// <summary>
        /// 바코드 종류를 리더기에 인식케 한다.
        /// 대략 3 ~ 30초 가량 시간이 소요된다.
        /// </summary>
        /// <returns></returns>
        public async Task<Matrix200Code> LearnBarCodeAsync()
        {
            if (!IsEnableSerial)
                return null;

            await m_SerialPort.BaseStream.WriteAsync(CMD_VISISET_LEARN, 0, CMD_VISISET_LEARN.Length);
            List<byte> storage = new List<byte>();
            int buf_size = 0;
            do
            {
                buf_size = await m_SerialPort.BaseStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
                for (int i = 0; i < buf_size; i++)
                    storage.Add(m_Buffer.ElementAt(i));

                if (storage.Count > 3)
                    if (storage[storage.Count - 3] == 0x03 &&
                        storage[storage.Count - 2] == 0x03 &&
                        storage[storage.Count - 1] == 0x45)
                        break;
                if (storage.Count > 3)
                    if (storage[storage.Count - 3] == 0x03 &&
                        storage[storage.Count - 2] == 0x48 &&
                        storage[storage.Count - 1] == 0x05)
                        break;
            } while (buf_size != 0);
            string reply = Encoding.ASCII.GetString(storage.ToArray());
            Match match = Regex.Match(reply, @"Checking configuration...OK");
            return match.Success ? ProcessReply(reply) : null;
        }

        private Matrix200Code ProcessReply(string reply_s)
        {
            Matrix200Code info = new Matrix200Code();
            //코드
            Match match = Regex.Match(reply_s, @"New Code \((.+)\)");
            if (match.Success)
                info.NewCode = match.Groups[1].Value.Trim();
            //심볼
            match = Regex.Match(reply_s, @"Symbology: (.+)");
            if (match.Success)
                info.Symbology = match.Groups[1].Value.Trim();
            //데이터 문자 개수
            match = Regex.Match(reply_s, @"Number of Characters: (.+)");
            if (match.Success)
                Int32.TryParse(match.Groups[1].Value.Trim(), out info.NumberofCharacters);
            //코드 센터 포지션
            match = Regex.Match(reply_s, @"Code Center Position: \(([0-9]*),([0-9]*)\)");
            if (match.Success)
            {
                Int32.TryParse(match.Groups[1].Value.Trim(), out info.CodeCenterPosition.X);
                Int32.TryParse(match.Groups[2].Value.Trim(), out info.CodeCenterPosition.Y);
            }
            //픽셀 요소
            match = Regex.Match(reply_s, @"Pixel per Element: (.+)");
            if (match.Success)
                Single.TryParse(match.Groups[1].Value.Trim(), out info.PixelPerElement);
            //디코딩 소요 시간 
            match = Regex.Match(reply_s, @"Decoding Time \(ms\): ([0-9]*)");
            if (match.Success)
            {
                int d_time;
                if (Int32.TryParse(match.Groups[1].Value.Trim(), out d_time))
                    info.DecodingTime = new TimeSpan(0, 0, 0, 0, d_time);
            }
            //코드 오리엔테이션
            match = Regex.Match(reply_s, @"Code Orientation: ([0-9])");
            if (match.Success)
                Int32.TryParse(match.Groups[1].Value.Trim(), out info.CodeOrientation);
            //데이터
            match = Regex.Match(reply_s, @"Data: ([\S ]*)");
            if (match.Success)
                info.Data = match.Groups[1].Value.Trim().Substring(0, info.NumberofCharacters);
            //코드 바운딩 좌표
            match = Regex.Match(reply_s, @"Code Bounds: TL\[([0-9]*),([0-9]*)\].+TR\[([0-9]*),([0-9]*)\].+BL\[([0-9]*),([0-9]*)\].+BR\[([0-9]*),([0-9]*)\]");
            if (match.Success)
            {
                Int32.TryParse(match.Groups[1].Value.Trim(), out info.CodeBounds.TL.X);
                Int32.TryParse(match.Groups[2].Value.Trim(), out info.CodeBounds.TL.Y);
                Int32.TryParse(match.Groups[3].Value.Trim(), out info.CodeBounds.TR.X);
                Int32.TryParse(match.Groups[4].Value.Trim(), out info.CodeBounds.TR.Y);
                Int32.TryParse(match.Groups[5].Value.Trim(), out info.CodeBounds.BL.X);
                Int32.TryParse(match.Groups[6].Value.Trim(), out info.CodeBounds.BL.Y);
                Int32.TryParse(match.Groups[7].Value.Trim(), out info.CodeBounds.BR.X);
                Int32.TryParse(match.Groups[8].Value.Trim(), out info.CodeBounds.BR.Y);
            }
            //이미지 인식 퀄리티
            match = Regex.Match(reply_s, @"Image Exposure Quality: ([0-9]*)%");
            if (match.Success)
                Int32.TryParse(match.Groups[1].Value.Trim(), out info.ExposureQuality);
            //프로세싱 소요 시간
            match = Regex.Match(reply_s, @"Image Processing Time \(ms\): ([0-9]*)");
            if (match.Success)
            {
                int p_time;
                if (Int32.TryParse(match.Groups[1].Value.Trim(), out p_time))
                    info.ProcessingTime = new TimeSpan(0, 0, 0, 0, p_time);
            }
            //기타 작업
            string[] reply_a = reply_s.Split('\n');
            foreach (var r in reply_a)
            {
                if (r.Contains(':'))
                {
                    match = Regex.Match(r, @"(.*):(.*)");
                    if (match.Success)
                    {
                        string key = match.Groups[1].Value.Trim();
                        string value = match.Groups[2].Value.Trim();
                        info.RawData.Add(key, value);
                    }
                }
            }
            return info;
        }
    }
}