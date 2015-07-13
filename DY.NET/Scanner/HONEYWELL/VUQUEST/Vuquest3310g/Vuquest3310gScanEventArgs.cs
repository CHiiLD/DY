using System;

namespace DY.NET.HONEYWELL.VUQUEST
{
    public class Vuquest3310gScanEventArgs : EventArgs
    {
        public byte[] Code
        {
            get;
            private set;
        }

        public Vuquest3310gScanEventArgs(byte[] data)
        {
            Code = data;
        }
    }
}
