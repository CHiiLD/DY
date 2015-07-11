using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace DY.NET.DATALOGIC.MATRIX
{
    /// <summary>
    /// Matrix200 바코드 리더기를 기준으로 만들어진 디바이스 통신 클래스
    /// Matrix210과 함께 Matrix 시리즈와 호환이 될 것으로 예상 (테스트는 안해봄)
    /// </summary>
    public partial class Matrix200 : Matrix200Command, IDisposable
    {
        private SerialPort m_SerialPort;
        private byte[] m_Buffer = new byte[4096];

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

        ~Matrix200()
        {
            Dispose();
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
                m_SerialPort.Open();
            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// 리더기에 종료 신호를 보낸 뒤
        /// 시리얼통신을 종료
        /// </summary>
        public void Close()
        {
            if (IsEnableSerial)
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
            GC.SuppressFinalize(this);
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

            // 오토런 모드의 타임아웃 시간을 1초로 설정한다.
            await SendAsync(H_CMD_ENTER_HOST_MODE, R_CMD_ENTER_HOST_MODE);
            await SendAsync(H_CMD_ENTER_PROGRAMMING_MODE, R_CMD_ENTER_PROGRAMMING_MODE);

            m_SerialPort.Write(CMD_LED_AUTOLEARN_TIMEOUT_1000MS, 0, CMD_LED_AUTOLEARN_TIMEOUT_1000MS.Length);
            m_SerialPort.Write(CMD_END_SINGLE_PARAMETER_SEQ, 0, CMD_END_SINGLE_PARAMETER_SEQ.Length);

            m_SerialPort.Write(CMD_EXIT_PROG_MODE_AND_DATA_STORAGE, 0, CMD_EXIT_PROG_MODE_AND_DATA_STORAGE.Length);
            await SendAsync(H_CMD_EXIT_HOST_MODE, R_CMD_EXIT_HOST_MODE);

            // VISISET MODE 접속
            await SendAsync(CMD_VISISET_CONNECT, new byte[] { 0x8E });
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
            byte[] bytes = await SendAsync(CMD_VISISET_CAPTURE, new byte[] { 0x03, 0x1A, 0x57 });
            string reply = Encoding.ASCII.GetString(bytes);
            Match match = Regex.Match(reply, @"\(([0-9]*)x([0-9]*)\)");
            Tuple<int, int> ret = null;
            if (match.Success)
            {
                string s_width = match.Groups[1].Value;
                string s_height = match.Groups[2].Value;
                int width, height;
                if (Int32.TryParse(s_width, out width) && Int32.TryParse(s_height, out height))
                    ret = new Tuple<int, int>(width, height);
            }
            return ret;
        }

        /// <summary>
        /// 메모리에 저장된 이미지를 분석하여 바코드 코드 정보를 얻는다
        /// </summary>
        /// <returns>성공할 시 ProcessingInfo 리턴, 그렇지 않으면 null 리턴</returns>
        public async Task<Matrix200Code> DecodingAsync()
        {
            if (!IsEnableSerial)
                return null;
            List<byte[]> list = new List<byte[]>();
            list.Add(new byte[] { 0x03, 0x03, 0x45 }); //성공 시
            list.Add(new byte[] { 0x03, 0x48, 0x05 }); //실패 시
            byte[] bytes = await SendAsync(CMD_VISISET_DECODING, list);
            string reply = Encoding.ASCII.GetString(bytes);
            return ProcessReply(reply);
        }

        public async Task OpenSetupAsync()
        {
            await SendAsync(CMD_VISISET_SETUP_OPEN, new byte[] { 0x03, 0x01, 0x17 });
        }

        public async Task CaptureForSetupAsync()
        {
            await m_SerialPort.BaseStream.WriteAsync(CMD_VISISET_SETUP_CAPTURE, 0, CMD_VISISET_SETUP_CAPTURE.Length);
            List<byte> storage = new List<byte>();
            int buf_size = 0;
            do
            {
                buf_size = await m_SerialPort.BaseStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
                for (int i = 0; i < buf_size; i++)
                    storage.Add(m_Buffer.ElementAt(i));
                if (storage.Count < 3)
                    continue;
                if (storage[storage.Count() - 3] == 0x03)
                    break;
            } while (buf_size != 0);
        }

        public async Task<string> SettingCodeForSetupAsync()
        {
            byte[] bytes = await SendAsync(CMD_VISISET_SETUP_CODESETTING, new byte[] { 0x03, 0x00, 0x84 });
            string reply = Encoding.ASCII.GetString(bytes);
            Match match = Regex.Match(reply, @"[0-9]\. (.*)");
            string symbol = "";
            if (match.Success)
                symbol = match.Groups[1].Value.Trim();
            string[] split = symbol.Split('\0');
            return split[0];
        }

        public async Task<bool> SavePermenentForSetupAsync()
        {
            byte[] bytes = await SendAsync(CMD_VISISET_SETUP_SAVE, new byte[] { 0x03, 0x19, 0xD8 });
            string reply = Encoding.ASCII.GetString(bytes);
            Match match = Regex.Match(reply, @"Checking configuration...OK");
            return match.Success;
        }

        public async Task CloseSetupAsync()
        {
            await SendAsync(CMD_VISISET_SETUP_CLOSE, new byte[] { 0x03, 0x00, 0x38 });
        }

        private async Task<byte[]> SendAsync(byte[] CMD, byte[] tail)
        {
            List<byte[]> list = new List<byte[]>();
            list.Add(tail);
            return await SendAsync(CMD, list);
        }

        private async Task<byte[]> SendAsync(byte[] CMD, List<byte[]> tailList)
        {
            await m_SerialPort.BaseStream.WriteAsync(CMD, 0, CMD.Length);
            List<byte> storage = new List<byte>();
            int buf_size = 0;
            do
            {
                buf_size = await m_SerialPort.BaseStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
#if false
                Console.WriteLine("**************************************************");
                for (int i = 0; i < buf_size; i++)
                {
                    Console.Write("{0:X2} ", m_Buffer[i]);
                    if (i % 8 == 0)
                        Console.WriteLine("");
                }
                Console.WriteLine("");
                //if (buf_size > 0)
                //{
                //    byte[] temp_buf = new byte[buf_size];
                //    Buffer.BlockCopy(m_Buffer, 0, temp_buf, 0, buf_size);
                //    Console.WriteLine(Encoding.ASCII.GetString(temp_buf));
                //}
                Console.WriteLine("**************************************************");
#endif
                for (int i = 0; i < buf_size; i++)
                    storage.Add(m_Buffer.ElementAt(i));
                bool isBreak = false;
                foreach (var item in tailList)
                {
                    if (storage.Count() < item.Count())
                        continue;
                    int cnt = 0;
                    for (int i = 0; i < item.Count(); i++)
                    {
                        if (storage[storage.Count() - 1 - i] == item[item.Count() - 1 - i])
                            cnt++;
                        else
                            break;
                    }
                    if (cnt == item.Count())
                    {
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                    break;
            } while (buf_size != 0);
            return storage.ToArray();
        }

        /// <summary>
        /// 바코드 종류를 리더기에 인식케 한다.
        /// 대략 3 ~ 30초 가량 시간이 소요된다.
        /// </summary>
        /// <returns></returns>
        public async Task<Matrix200Code> LearnCodeAsync()
        {
            if (!IsEnableSerial)
                return null;
            List<byte[]> list = new List<byte[]>();
            list.Add(new byte[] { 0x03, 0x03, 0x45 });
            list.Add(new byte[] { 0x03, 0x48, 0x05 });
            byte[] bytes = await SendAsync(CMD_VISISET_LEARN_START, list);
            string reply = Encoding.ASCII.GetString(bytes);
            Match match = Regex.Match(reply, @"Checking configuration...OK");
            return match.Success ? ProcessReply(reply) : null;
        }

        public async Task CancelLearnCodeAsync()
        {
            await m_SerialPort.BaseStream.WriteAsync(CMD_VISISET_LEARN_CANCLE, 0, CMD_VISISET_LEARN_CANCLE.Length);
            List<byte> storage = new List<byte>();
            int buf_size = 0;
            do
            {
                buf_size = await m_SerialPort.BaseStream.ReadAsync(m_Buffer, 0, m_Buffer.Length);
                for (int i = 0; i < buf_size; i++)
                    storage.Add(m_Buffer.ElementAt(i));

                if (storage.Count > 3)
                    if (storage[storage.Count - 3] == 0x03 &&
                        storage[storage.Count - 2] == 0x48 &&
                        storage[storage.Count - 1] == 0x05)
                        break;
            } while (buf_size != 0);
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
            {
                string code_str = match.Groups[1].Value.Trim().Substring(0, info.NumberofCharacters);
                info.Code = Encoding.ASCII.GetBytes(code_str);
            }
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