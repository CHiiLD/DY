using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DY.WPF
{
    /// <summary>
    /// 시리얼포트 생성자 매개 변수 리스트 프로퍼티 제공 클래스
    /// </summary>
    public static class SerialPortOption
    {
        private static readonly List<int> BAND_RATE = new List<int>
        {
            2400, 
            4800, 
            9600, 
            19200, 
            38400, 
            57600, 
            115200, 
        };

        private static readonly List<int> DATA_BIT = new List<int>
        {
            7,
            8
        };

        public static List<int> BandRate
        {
            get
            {
                return BAND_RATE;
            }
        }

        public static List<int> DataBit
        {
            get
            {
                return DATA_BIT;
            }
        }
    }
}