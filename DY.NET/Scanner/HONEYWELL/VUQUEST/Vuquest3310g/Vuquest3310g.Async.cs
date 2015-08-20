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
        private readonly AsyncLock m_AsyncLock = new AsyncLock();
        private bool m_IsPrepared = false;
        /// <summary>
        /// 스캔에 필요한 리더기 파라미터를 설정한다. 
        /// 스캔에 앞서 먼저 한번 호출해야한다.
        /// </summary>
        /// <returns></returns>
        public async Task<DeliveryError> PrepareAsync()
        {
            if (m_IsPrepared)
                return DeliveryError.SUCCESS;

            Delivery delivery = new Delivery();
            if (!IsConnected())
                return DeliveryError.DISCONNECT;
            List<byte> syn = new List<byte>();
            List<byte> rep = new List<byte>();
            byte[] reply;

            Dictionary<byte[], string> CMDDIC = new Dictionary<byte[], string>();
            CMDDIC.Add(PSS_ADD_CR_SUFIX_ALL_SYMBOL, "CR sufix setting error");
            CMDDIC.Add(SPC_TRIGGER_READ_TIMEOUT_300000MS, "Timeout setting error");

            using (await m_AsyncLock.LockAsync())
            {
                foreach (var CMD in CMDDIC)
                {
                    syn.Clear();
                    syn.AddRange(PREFIX);
                    syn.AddRange(CMD.Key);
                    syn.Add(DOT);

                    rep.Clear();
                    rep.AddRange(CMD.Key);
                    rep.Add(ACK);
                    rep.Add(DOT);

                    if (!await WriteAsync(syn.ToArray(), 0, syn.Count))
                        return DeliveryError.WRITE_TIMEOUT;

                    int size = await ReadAsync(m_Buffer, 0, m_Buffer.Length);
                    if (size < 0)
                        return DeliveryError.READ_TIMEOUT;

                    reply = new byte[size];
                    Array.Copy(m_Buffer, reply, size);

                    if (!rep.SequenceEqual(reply))
                        throw new Exception(CMD.Value);
                }
            }
            m_IsPrepared = true;
            return DeliveryError.SUCCESS;
        }

        public async Task<Delivery> ScanAsync()
        {
            Delivery delivery = new Delivery();
            delivery.Error = await PrepareAsync();
            if (DeliveryError.SUCCESS != delivery.Error)
                return delivery.Packing();

            using (await m_AsyncLock.LockAsync())
            {
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
            return delivery.Packing(DeliveryError.SUCCESS);
        }

        public async Task<Delivery> GetInfoAsync()
        {
            Delivery delivery = new Delivery();
            if (DeliveryError.SUCCESS != await PrepareAsync())
                return null;
            List<byte> syn = new List<byte>();
            syn.AddRange(PREFIX);
            syn.AddRange(UTI_SHOW_SOFTWARE_REVERSION);
            syn.Add(DOT);
            using (await m_AsyncLock.LockAsync())
            {
                m_BufferIdx = 0;
                if (!await WriteAsync(syn.ToArray(), 0, syn.Count))
                    return delivery.Packing(DeliveryError.WRITE_TIMEOUT);
                do
                {
                    int size = await ReadAsync(m_Buffer, m_BufferIdx, m_Buffer.Length - m_BufferIdx);
                    if (size < 0)
                        return delivery.Packing(DeliveryError.READ_TIMEOUT);
                    m_BufferIdx += size;
                } while (m_Buffer[m_BufferIdx - 1] != (byte)0x2E);
                byte[] buffer = new byte[m_BufferIdx - 1];
                Array.Copy(m_Buffer, 0, buffer, 0, buffer.Length);
                delivery.Package = buffer;
            }
            return delivery.Packing(DeliveryError.SUCCESS);
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
            m_BufferIdx = 0;
            do
            {
                int size = await ReadAsync(m_Buffer, m_BufferIdx, m_Buffer.Length - m_BufferIdx);
                if (size < 0)
                    throw new TimeoutException(ERROR_READ_TIMEOUT);
                m_BufferIdx += size;
            } while (m_Buffer[m_BufferIdx - 1] != CR);
            byte[] buffer = new byte[m_BufferIdx - 1];
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
