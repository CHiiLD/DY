using System;

namespace DY.NET.HONEYWELL.VUQUEST
{
    public class Vuquest3310gDataReceivedEventArgs : EventArgs
    {
        public byte[] Code
        {
            get;
            private set;
        }

        public Vuquest3310gDataReceivedEventArgs(byte[] data)
        {
            Code = data;
        }
    }
}
