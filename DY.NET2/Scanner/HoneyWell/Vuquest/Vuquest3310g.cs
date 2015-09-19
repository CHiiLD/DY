using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DY.NET.LSIS.XGT
{
    public class Vuquest3310g : IPostOffice, IScannerActor
    {
        public Stream Stream { get; set; }

        public virtual async Task<bool> ConnectAsync()
        {
            return false;
        }

        public virtual async Task DisconnectAsync()
        {
        }

        public virtual bool IsConnected()
        {
            return false;
        }

        public Task<byte[]> ScanAsync()
        {
            return null;
        }

        public virtual void Dispose()
        {

        }
    }
}
