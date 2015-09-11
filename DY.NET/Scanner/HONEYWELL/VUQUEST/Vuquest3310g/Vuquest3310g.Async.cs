using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Nito.AsyncEx;

namespace DY.NET.HONEYWELL.VUQUEST
{
    public partial class Vuquest3310g
    {
        /// <summary>
        /// 해외의 유명 택배 배달회사이름.
        /// Delivery의 Stopwatch에 의한 성능저하를 피하기 위해 Stopwatch기능만 제거한 DHL inner class를 사용한다.
        /// </summary>
        private class DHL
        {
            public object Package { get; set; }
            public DeliveryError Error { get; set; }

            public DHL()
            {
                Error = DeliveryError.SUCCESS;
            }

            public DHL Packing(DeliveryError error)
            {
                Error = error;
                return this;
            }

            public DHL Packing()
            {
                return this;
            }
        }

        /// <summary>
        /// 바코드 스캔을 시도한다.
        /// </summary>
        /// <returns>Delivery 객체</returns>
        public async Task<Delivery> ScanAsync()
        {
            Delivery delivery = new Delivery();
            using (await Locker.LockAsync())
            {
                delivery.Error = await AddPrefixCRAsync(); //CR 설정
                if (delivery.Error != DeliveryError.SUCCESS)
                    return delivery.Packing();

                delivery.Error = await SetScanReadTimeout(ReadTimeout); //Reading Timeout 설정
                if (delivery.Error != DeliveryError.SUCCESS)
                    return delivery.Packing();

                DHL deliww = await ActivateAsync(); //스캔 시작
                if (deliww.Error != DeliveryError.SUCCESS)
                    return delivery.Packing(deliww.Error);
                delivery.Package = deliww.Package;
            }
            return delivery.Packing();
        }

        /// <summary>
        /// 스캐너의 정보(스펙)를 가져온다.
        /// </summary>
        /// <returns>Delivery 객체</returns>
        public async Task<Delivery> GetInfoAsync()
        {
            Delivery delivery = new Delivery();
            using (await Locker.LockAsync())
            {
                DHL dhl = await SendCommendCodeAsync(UTI_SHOW_SOFTWARE_REVERSION);
                if (dhl.Error != DeliveryError.SUCCESS)
                    return delivery.Packing(dhl.Error);
                delivery.Package = dhl.Package;
            }
            return delivery.Packing();
        }

        /// <summary>
        /// 스캐너에 명령어를 보내고 그 적용된 결과를 받는다.
        /// </summary>
        /// <param name="cmd">명령어</param>
        /// <returns>DeliveryWithoutStopwatch 객체</returns>
        private async Task<DHL> SendCommendCodeAsync(byte[] cmd)
        {
            byte[] req_code = GetRequestCommandCode(cmd);
            byte[] res_code = GetResponseCommandCode(cmd);
            DHL dhl = new DHL();
            if (!await WriteAsync(req_code, 0, req_code.Length))
                return dhl.Packing(DeliveryError.WRITE_TIMEOUT);
            int idx = 0;
            while (true)
            {
                int size = await ReadAsync(m_Buffer, idx, m_Buffer.Length - idx);
                if (size < 0)
                    return dhl.Packing(DeliveryError.READ_TIMEOUT);
                idx += size;
                if (idx >= res_code.Length)
                {
                    byte[] reply = new byte[res_code.Length];
                    Array.Copy(m_Buffer, idx - reply.Length, reply, 0, reply.Length);
                    if (reply.SequenceEqual(res_code))
                    {
                        if (idx - reply.Length > 0)
                        {
                            byte[] data = new byte[idx - reply.Length];
                            Array.Copy(m_Buffer, 0, data, 0, data.Length);
                            dhl.Package = data;
                        }
                        break;
                    }
                }
            }
            return dhl.Packing();
        }

        /// <summary>
        /// 스캐너에 보낼 명령어를 반환한다.
        /// </summary>
        /// <param name="cmd">명령어</param>
        /// <returns>요청 명령어 코드</returns>
        private byte[] GetRequestCommandCode(byte[] cmd)
        {
            List<byte> code = new List<byte>();
            code.AddRange(PREFIX);
            code.AddRange(cmd);
            code.Add(DOT);
            return code.ToArray();
        }

        /// <summary>
        /// 스캐너로부터 받을(받을 예정인) 응답 명령어 코드를 반환한다.
        /// </summary>
        /// <param name="cmd">명령어</param>
        /// <returns>응답 명령어 코드</returns>
        private byte[] GetResponseCommandCode(byte[] cmd)
        {
            List<byte> code = new List<byte>();
            code.AddRange(cmd);
            code.Add(ACK);
            code.Add(DOT);
            return code.ToArray();
        }

        /// <summary>
        /// 스캔에 필요한 리더기 파라미터를 설정한다. 
        /// 스캔에 앞서 먼저 한번 호출해야한다.
        /// </summary>
        /// <returns></returns>
        private async Task<DeliveryError> AddPrefixCRAsync()
        {
            if (m_IsPrepared)
                return DeliveryError.SUCCESS;
            if (!IsConnected())
                return DeliveryError.DISCONNECT;
            DeliveryError error = (await SendCommendCodeAsync(PSS_ADD_CR_SUFIX_ALL_SYMBOL)).Error;
            if (error != DeliveryError.SUCCESS)
            {
                LOG.Debug(Description + ": " + "CR sufix setting error");
                return error;
            }
            m_IsPrepared = true;
            return DeliveryError.SUCCESS;
        }

        /// <summary>
        /// 스캐너의 스캔 타임아웃을 설정한다. 
        /// 최대 30초까지 설정할 수 있다.
        /// </summary>
        /// <param name="timeout">타임아웃</param>
        /// <returns>타임아웃 적용 성공 여부</returns>
        private async Task<DeliveryError> SetScanReadTimeout(int timeout)
        {
            if (!m_IsTimeoutChanged)
                return DeliveryError.SUCCESS;
            if (!IsConnected())
                return DeliveryError.DISCONNECT;

            string timeout_str = timeout.ToString();
            List<byte> list = new List<byte>();
            list.AddRange(SPC_TRIGGER_READ_TIMEOUT_N);
            foreach (char i in timeout_str)
                list.Add((byte)i);

            DeliveryError error = (await SendCommendCodeAsync(list.ToArray())).Error;
            if (error != DeliveryError.SUCCESS)
            {
                LOG.Debug(Description + ": " + "Barcode read timeout setting error");
                return error;
            }
            m_IsTimeoutChanged = false;
            return DeliveryError.SUCCESS;
        }

        /// <summary>
        /// 스캐너에 버퍼를 채워 보낸다.
        /// </summary>
        /// <param name="buffer">버퍼</param>
        /// <param name="offset">오프셋</param>
        /// <param name="count">개수</param>
        /// <returns>성공 여부</returns>
        private async Task<bool> WriteAsync(byte[] buffer, int offset, int count)
        {
            var stream = m_SerialPort.BaseStream;
            var cts = new CancellationTokenSource();
            bool ok = false;
            Task write_task = stream.WriteAsync(buffer, offset, count, cts.Token);
            if (await Task.WhenAny(write_task, Task.Delay(WriteTimeout, cts.Token)) == write_task)
            {
                await write_task;
                ok = true;
            }
            if (!cts.IsCancellationRequested)
                cts.Cancel();
            return ok;
        }

        /// <summary>
        /// 스캐너의 응답을 받는다.
        /// </summary>
        /// <param name="buffer">버퍼</param>
        /// <param name="offset">오프셋</param>
        /// <param name="count">개수</param>
        /// <returns>읽은 바이트의 개수</returns>
        private async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            var stream = m_SerialPort.BaseStream;
            var cts = new CancellationTokenSource();
            Task read_task = stream.ReadAsync(buffer, offset, count, cts.Token);
            int size = -1;
            if (await Task.WhenAny(read_task, Task.Delay(ReadTimeout, cts.Token)) == read_task)
                size = await (Task<int>)read_task;
            if (!cts.IsCancellationRequested)
                cts.Cancel();
            return size;
        }

        /// <summary>
        /// 스캐너의 읽기 모드를 활성화하고 읽어들인 코드를 반환한다.
        /// </summary>
        /// <returns>DeliveryWithoutStopwatch 객체</returns>
        private async Task<DHL> ActivateAsync()
        {
            DHL dhl = new DHL();
            if (!await WriteAsync(SPC_TRIGGER_ACTIVATE, 0, SPC_TRIGGER_ACTIVATE.Length))
                return dhl.Packing(DeliveryError.WRITE_TIMEOUT);
            int idx = 0;
            do
            {
                int size = await ReadAsync(m_Buffer, idx, m_Buffer.Length - idx);
                if (size < 0)
                {
                    await SendCommendCodeAsync(UTI_SHOW_SOFTWARE_REVERSION);
                    return dhl.Packing(DeliveryError.READ_TIMEOUT);
                }
                idx += size;
            } while (m_Buffer[idx - 1] != CR);
            byte[] buffer = new byte[idx - 1];
            Array.Copy(m_Buffer, 0, buffer, 0, buffer.Length);
            dhl.Package = buffer;
            return dhl.Packing(DeliveryError.SUCCESS);
        }
    }
}
