using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidLibrary;

namespace DY.NET.HONEYWELL.VUQUEST
{
    /// <summary>
    /// 허니웰 VUQUEST 3310g 바코드 리더기 통신 클래스
    /// </summary>
    public class Vuquest3310g : IDisposable
    {
        private const int VENDOR_ID = 0x0C2E;
        private int[] m_ProductIds;
        private HidDevice m_Device;

        private static readonly byte[] CHAR_ACTIVATION_MODE_ON = new byte[] {
            (byte)'H', (byte)'S', (byte)'T', (byte)'A', (byte)'C', (byte)'H', (byte)'.'
        };
        private static readonly byte[] CHAR_ACTIVATION_LASER_TIMEOUT = new byte[] {
            (byte)'H', (byte)'S', (byte)'T', (byte)'C', (byte)'D', (byte)'T', (byte)'.'
        };
        private static readonly byte[] CHAR_ACTIVATION_MODE_OFF = new byte[] {
            (byte)'H', (byte)'S', (byte)'T', (byte)'D', (byte)'C', (byte)'H', (byte)'.'
        };

        private Vuquest3310g() { }

        public static Vuquest3310g CreateVuquest3310g(int[] productIds)
        {
            var instance = new Vuquest3310g();
            instance.m_ProductIds = productIds;
            instance.m_Device = HidDevices.Enumerate(VENDOR_ID, instance.m_ProductIds).FirstOrDefault();
            if (instance.m_Device == null)
                throw new Exception("Not find product info");
            return instance;
        }

        ~Vuquest3310g()
        {
            Dispose();
        }

        public async Task<string> ScanAsync()
        {
            return await ScanAsync(0);
        }

        public async Task<string> ScanAsync(int timeout)
        {
            //캐릭터 액티베이션 모드 명령어 전송
            bool act_on = await m_Device.WriteAsync(CHAR_ACTIVATION_MODE_ON);
            if (!act_on)
            {
                Console.WriteLine("Vuquest3310g write failure(ACT)");
                return null;
            }
            //읽기 대기
            HidDeviceData deviceData = await m_Device.ReadAsync();
            if (deviceData.Status != HidDeviceData.ReadStatus.Success)
            {
                Console.WriteLine("Vuquest3310g read failure");
                return null;
            }
            //캐릭터 디액티베이션 모드 명령어 전송
            bool act_off = await m_Device.WriteAsync(CHAR_ACTIVATION_MODE_OFF);
            if (!act_off)
            {
                Console.WriteLine("Vuquest3310g write failure(DEACT)");
                return null;
            }
            return Encoding.ASCII.GetString(deviceData.Data);
        }

        public bool Connect()
        {
            if (m_ProductIds == null)
                return false;
            if (!m_Device.IsOpen) 
                m_Device.OpenDevice();
            m_Device.MonitorDeviceEvents = true;
            return true;
        }

        public void Close()
        {
            if (m_Device != null)
                m_Device.CloseDevice();
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
    }
}