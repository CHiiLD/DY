using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DY.WPF
{
    public static class SerialPortOption
    {
        private static readonly ObservableCollection<int> BAND_RATE = new ObservableCollection<int>
        {
            2400, 
            4800, 
            9600, 
            19200, 
            38400, 
            57600, 
            115200, 
        };

        public static ObservableCollection<int> BandRate
        {
            get
            {
                return BAND_RATE;
            }
        }

        private static readonly ObservableCollection<int> DATA_BIT = new ObservableCollection<int>
        {
            7,
            8
        };


        public static ObservableCollection<int> DataBit
        {
            get
            {
                return DATA_BIT;
            }
        }

        private static readonly ObservableCollection<string> COM = new ObservableCollection<string>
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

        public static ObservableCollection<string> Com
        {
            get
            {
                return COM;
            }
        }
    }
}