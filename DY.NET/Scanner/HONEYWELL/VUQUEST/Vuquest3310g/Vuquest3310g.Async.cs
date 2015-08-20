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
        public async Task<Delivery> ScanAsync()
        {
            Delivery delivery = new Delivery();
            using (await m_AsyncLock.LockAsync())
            {
                delivery.Error = await PrepareAsync();
                if (delivery.Error != DeliveryError.SUCCESS)
                    return delivery.Packing();

                byte[] code = null;
                try
                {
                    code = await ActivateAsync();
                }
                catch (TimeoutException timeout_exception)
                {
                    if (timeout_exception.Message == ERROR_WRITE_TIMEOUT)
                        delivery.Error = DeliveryError.WRITE_TIMEOUT;
                    else if (timeout_exception.Message == ERROR_READ_TIMEOUT)
                        delivery.Error = DeliveryError.READ_TIMEOUT;
                }
                catch (Exception exception)
                {
                    LOG.Debug(Description + " Scan 중 예외발생: " + exception.Message + "\n" + exception.StackTrace);
                }
                if (delivery.Error != DeliveryError.SUCCESS)
                    await DeactivateAsync();
                delivery.Package = code;
            }
            return delivery.Packing();
        }

        public async Task<Delivery> GetInfoAsync()
        {
            Delivery delivery = new Delivery();
            using (await m_AsyncLock.LockAsync())
            {
                delivery.Error = await PrepareAsync();
                if (delivery.Error != DeliveryError.SUCCESS)
                    return delivery.Packing();

                var delivery2 = await SendCommendCodeAsync(UTI_SHOW_SOFTWARE_REVERSION);
                if (delivery2.Error != DeliveryError.SUCCESS)
                    return delivery.Packing(delivery2.Error);
                else
                    delivery.Package = delivery2.Package;
            }
            return delivery.Packing();
        }

        private async Task<Delivery> SendCommendCodeAsync(byte[] cmd)
        {
            byte[] req_code = GetRequestCommandCode(cmd);
            byte[] res_code = GetResponseCommandCode(cmd);
            Delivery delivery = new Delivery();
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
                        if(idx - reply.Length > 0)
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
        private async Task<DeliveryError> PrepareAsync()
        {
            if (m_IsPrepared)
                return DeliveryError.SUCCESS;
            if (!IsConnected())
                return DeliveryError.DISCONNECT;
            Dictionary<byte[], string> commends = new Dictionary<byte[], string>();
            commends.Add(PSS_ADD_CR_SUFIX_ALL_SYMBOL, "CR sufix setting error");
            commends.Add(SPC_TRIGGER_READ_TIMEOUT_300000MS, "Timeout setting error");
            foreach (var cmd in commends)
            {
                DeliveryError error = (await SendCommendCodeAsync(cmd.Key)).Error;
                if (error != DeliveryError.SUCCESS)
                {
                    LOG.Debug(Description + ": " + cmd.Value);
                    return error;
                }
            }
            m_IsPrepared = true;
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

        private async Task<byte[]> ActivateAsync()
        {
            if (!await WriteAsync(SPC_TRIGGER_ACTIVATE, 0, SPC_TRIGGER_ACTIVATE.Length))
                throw new TimeoutException(ERROR_WRITE_TIMEOUT);
            int idx = 0;
            do
            {
                int size = await ReadAsync(m_Buffer, idx, m_Buffer.Length - idx);
                if (size < 0)
                    throw new TimeoutException(ERROR_READ_TIMEOUT);
                idx += size;
            } while (m_Buffer[idx - 1] != CR);
            byte[] buffer = new byte[idx - 1];
            Array.Copy(m_Buffer, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        private async Task DeactivateAsync()
        {
            if (!await WriteAsync(SPC_TRIGGER_DEACTIVATE, 0, SPC_TRIGGER_DEACTIVATE.Length))
                throw new TimeoutException(ERROR_WRITE_TIMEOUT);
        }
    }
}
