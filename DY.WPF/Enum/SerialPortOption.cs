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

        public static List<int> BandRate
        {
            get
            {
                return BAND_RATE;
            }
        }

        private static readonly List<int> DATA_BIT = new List<int>
        {
            7,
            8
        };


        public static List<int> DataBit
        {
            get
            {
                return DATA_BIT;
            }
        }

        private static readonly List<string> COM = new List<string>
        {
            "COM1", 
            "COM2", 
            "COM3", 
            "COM4", 
            "COM5", 
            "COM6", 
            "COM7", 
            "COM8", 
            "COM9", 
            "COM10", 
            "COM11", 
            "COM12", 
            "COM13", 
            "COM14", 
            "COM15", 
            "COM16", 
            "COM17", 
            "COM18", 
            "COM19", 
            "COM20"  
        };

        public static List<string> Com
        {
            get
            {
                return COM;
            }
        }
    }
}