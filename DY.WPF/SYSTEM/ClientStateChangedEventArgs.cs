using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DY.NET;
namespace DY.WPF.SYSTEM
{
    public class ClientStateChangedEventArgs : EventArgs
    {
        public IConnect Client { get; private set; }
    }
}