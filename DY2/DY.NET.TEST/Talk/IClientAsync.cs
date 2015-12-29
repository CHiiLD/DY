using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IClientAsync : ITimeoutProperty
    {
        Task<bool> OpenAsync();
        Task CloseAsync();
        Task<bool> CanTalkAsync();
        Task<object> TalkAsync(object parameter);
    }
}
