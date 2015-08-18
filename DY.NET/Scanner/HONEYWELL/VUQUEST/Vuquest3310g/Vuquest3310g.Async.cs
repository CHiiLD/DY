using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DY.NET.HONEYWELL.VUQUEST
{
    public partial class Vuquest3310g
    {
        /// <summary>
        /// 스캔에 필요한 리더기 파라미터를 설정한다. 
        /// 스캔에 앞서 먼저 한번 호출해야한다.
        /// </summary>
        /// <returns></returns>
        public async Task PrepareAsync()
        {
            if (!IsConnected())
                return;
            List<byte> syn = new List<byte>();
            List<byte> rep = new List<byte>();
            byte[] reply;

            Dictionary<byte[], string> CMDDIC = new Dictionary<byte[], string>();
            CMDDIC.Add(PSS_ADD_CR_SUFIX_ALL_SYMBOL, "CR sufix setting error");
            CMDDIC.Add(SPC_TRIGGER_READ_TIMEOUT_300000MS, "Timeout setting error");

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

                int write_ret = await WriteAsync(syn.ToArray(), 0, syn.Count);
                if (write_ret < 0)
                    throw new TimeoutException(ERROR_WRITE_TIMEOUT);

                int size = await ReadAsync(m_Buffer, 0, m_Buffer.Length);
                if (size < 0)
                    throw new TimeoutException(ERROR_READ_TIMEOUT);

                reply = new byte[size];
                Array.Copy(m_Buffer, reply, size);

                if (!rep.SequenceEqual(reply))
                    throw new Exception(CMD.Value);
            }
        }

        public async Task<Delivery> ScanAsync()
        {
            Delivery delivery = new Delivery();
            byte[] code = null;
            try
            {
                code = await ActivateAsync();
                await DeactivateAsync();
            }
            catch (TimeoutException timeout_exception)
            {
                if (timeout_exception.Message == ERROR_WRITE_TIMEOUT)
                    delivery.Error = DeliveryError.WRITE_TIMEOUT;
                else if (timeout_exception.Message == ERROR_READ_TIMEOUT)
                    delivery.Error = DeliveryError.READ_TIMEOUT;
#if DEBUG
                Debug.Assert(false);
#endif
            }
            delivery.Package = code;
            return delivery.Packing();
        }

        public async Task<byte[]> ActivateAsync()
        {
            if (m_IsActivate)
                return null;
            m_IsActivate = true;

            int write_ret = await WriteAsync(SPC_TRIGGER_ACTIVATE, 0, SPC_TRIGGER_ACTIVATE.Length);
            if (write_ret < 0)
                throw new TimeoutException(ERROR_WRITE_TIMEOUT);
            m_BufferIdx = 0;
            do
            {
                int size = await ReadAsync(m_Buffer, m_BufferIdx, m_Buffer.Length - m_BufferIdx);
                if (size < 0)
                    throw new TimeoutException(ERROR_READ_TIMEOUT);
                m_BufferIdx += size;
            } while (m_Buffer.Last() != CR);
            byte[] buffer = new byte[m_BufferIdx - 1];
            Array.Copy(m_Buffer, 0, buffer, 0, buffer.Length);

            return buffer;
        }

        public async Task DeactivateAsync()
        {
            int write_ret = await WriteAsync(SPC_TRIGGER_DEACTIVATE, 0, SPC_TRIGGER_DEACTIVATE.Length);
            if (write_ret < 0)
                throw new TimeoutException(ERROR_WRITE_TIMEOUT);

            m_IsActivate = false;
        }
    }
}
