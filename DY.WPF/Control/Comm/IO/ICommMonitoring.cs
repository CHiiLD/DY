using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.WPF.SYSTEM.COMM;

namespace DY.WPF
{
    interface ICommMonitoring
    {
        CommClient Client { get; set; }
    }
}