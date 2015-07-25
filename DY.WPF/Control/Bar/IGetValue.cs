using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DY.NET;

namespace DY.WPF
{
    public interface IGetValue : ITag
    {
        object GetValue();
    }
}
