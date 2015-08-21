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
        private class DeliveryWithoutStopwatch
        {
            public object Package { get; set; }
            public DeliveryError Error { get; set; }

            public DeliveryWithoutStopwatch()
            {
                Error = DeliveryError.SUCCESS;
            }

            public DeliveryWithoutStopwatch Packing(DeliveryError error)
            {
                Error = error;
                return this;
            }

            public DeliveryWithoutStopwatch Packing()
            {
                return this;
            }
        }

        public async Task<Delivery> ScanAsync()
        {
            Delivery delivery = new Delivery();
            using (await m_AsyncLock.LockAsync())
            {
                delivery.Error = await AddPrefixCRAsync();
                if (delivery.Error != DeliveryError.SUCCESS)
                    return delivery.Packing();

                delivery.Error = await SetScanReadTimeout(ReadTimeout);
                if (delivery.Error != DeliveryError.SUCCESS)
                    return delivery.Packing();

                DeliveryWithoutStopwatch deliww = await ActivateAsync();
                if (deliww.Error != DeliveryError.SUCCESS)
                    return delivery.Packing(deliww.Error);
                delivery.Package = deliww.Package;
            }
            return delivery.Packing();
        }

        public async Task<Delivery> GetInfoAsync()
        {
            Delivery delivery = new Delivery();
            using (await m_AsyncLock.LockAsync())
            {
                DeliveryWithoutStopwatch deliww = await SendCommendCodeAsync(UTI_SHOW_SOFTWARE_REVERSION);
                if (deliww.Error != DeliveryError.SUCCESS)
                    return delivery.Packing(deliww.Error);
                delivery.Package = deliww.Package;
            }
            return delivery.Packing();
        }

        private async Task<DeliveryWithoutStopwatch> SendCommendCodeAsync(byte[] cmd)
        {
            byte[] req_code = GetRequestCommandCode(cmd);
            byte[] res_code = GetResponseCommandCode(cmd);
            DeliveryWithoutStopwatch delivery = new DeliveryWithoutStopwatch();
            if (!await WriteAsync(req_code, 0, req_code.Length))
                return delivery.Packing(DeliveryError.WRITE_TIMEOUT);
            int idx = 0;
            while (true)
            {
                int size = await ReadAsync(m_Buffer, idx, m_Buffer.Length - idx);
                if (size < 0)
                    return delivery.Packing(DeliveryError.READ_TIMEOUT);
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
                            delivery.Package = data;
                        }
                        break;
                    }
                }
            }
            return delivery.Packing();
        }

        private byte[] GetRequestCommandCode(byte[] cmd)
        {
            List<byte> code = new List<byte>();
            code.AddRange(PREFIX);
            code.AddRange(cmd);
            code.Add(DOT);
            return code.ToArray();
        }

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

        private async Task<DeliveryWithoutStopwatch> ActivateAsync()
        {
            DeliveryWithoutStopwatch delivery = new DeliveryWithoutStopwatch();
            if (!await WriteAsync(SPC_TRIGGER_ACTIVATE, 0, SPC_TRIGGER_ACTIVATE.Length))
                return delivery.Packing(DeliveryError.WRITE_TIMEOUT);
            int idx = 0;
            do
            {
                int size = await ReadAsync(m_Buffer, idx, m_Buffer.Length - idx);
                if (size < 0)
                {
                    await SendCommendCodeAsync(UTI_SHOW_SOFTWARE_REVERSION);
                    return delivery.Packing(DeliveryError.READ_TIMEOUT);
                }
                idx += size;
            } while (m_Buffer[idx - 1] != CR);
            byte[] buffer = new byte[idx - 1];
            Array.Copy(m_Buffer, 0, buffer, 0, buffer.Length);
            delivery.Package = buffer;
            return delivery.Packing(DeliveryError.SUCCESS);
        }

#if false
        private async Task<DeliveryWithoutStopwatch> DeactivateAsync()
        {
            DeliveryWithoutStopwatch delivery = new DeliveryWithoutStopwatch();
            if (!await WriteAsync(SPC_TRIGGER_DEACTIVATE, 0, SPC_TRIGGER_DEACTIVATE.Length))
            {
                LOG.Error(Description + ": 스캐너 스캔 비활성화 요청 중 스트림 버퍼 쓰기 에러가 발생");
                return delivery.Packing(DeliveryError.WRITE_TIMEOUT);
            }
            else
            {
                return delivery.Packing(DeliveryError.SUCCESS);
            }
        }
#endif
    }
}
