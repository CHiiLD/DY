using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DY.NET.LSIS.XGT
{
    public class XGTFEnetPostOffice : IPostOffice, IPoster
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

        public virtual Task<IProtocol> PostAsync(IProtocol protocol)
        {
            return null;
        }

        public virtual void Dispose()
        {

        }
    }
}
