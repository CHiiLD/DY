using System;
using NUnit.Framework;
using DY.NET.Honeywell.Vuquest;
using System.Threading.Tasks;

namespace DY.NET.Test
{
    /// <summary>
    /// 테스트 가정
    /// - Vuqeust3310g 스캐너의 모드가 시리얼모드가 설정된 상태로 가상시리얼포트로 컴퓨터에 연결되어 있다.
    /// - 스캐너의 30cm 앞에는 인식 가능한 바코드가 있다.
    /// - 스캐너의 고유 시리얼 넘버는 14351B07771 이다.
    /// </summary>
    //[Ignore]
    [TestFixture]
    public class Vuqeust3310gTest
    {
        private string m_Com = "COM5";
        private int m_Bandrate = 115200;
        private System.IO.Ports.Parity m_Parity = System.IO.Ports.Parity.None;
        private int m_DataBit = 8;
        private System.IO.Ports.StopBits m_Stopbit = System.IO.Ports.StopBits.One;

        [Test]
        public async void Open()
        {
            using (var scanner = new Vuquest3310g(m_Com, m_Bandrate, m_Parity, m_DataBit, m_Stopbit))
            {
                await scanner.OpenAsync();
                Assert.True(scanner.IsOpend());
            }
        }

        [Test]
        [ExpectedException(typeof(ReadTimeoutException))]
        public async void WhenScan_ExpectReadTimeoutException()
        {
            using (var scanner = new Vuquest3310g(m_Com, m_Bandrate, m_Parity, m_DataBit, m_Stopbit))
            {
                await scanner.OpenAsync();
                scanner.ReceiveTimeout = 1;
                byte[] code = null;
                code = (await scanner.ScanAsync()).Value as byte[];
            }
        }

        [Test]
        public async void Scan()
        {
            using (var scanner = new Vuquest3310g(m_Com, m_Bandrate, m_Parity, m_DataBit, m_Stopbit))
            {
                await scanner.OpenAsync();
                scanner.ReceiveTimeout = 100;
                byte[] code = (await scanner.ScanAsync()).Value as byte[];
                Assert.AreNotEqual(code, null);
            }
        }

        [Test]
        public async void GetProductSN()
        {
            using (var scanner = new Vuquest3310g(m_Com, m_Bandrate, m_Parity, m_DataBit, m_Stopbit))
            {
                string expectedSN = "14351B0771";
                await scanner.OpenAsync();
                int cnt = 100;
                for (int i = 0; i < cnt; i++)
                {
                    string sn = await scanner.GetProductSerialNumber();
                    Assert.AreEqual(sn, expectedSN);
                }
            }
        }
    }
}