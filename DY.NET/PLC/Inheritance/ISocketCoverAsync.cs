using System.Threading.Tasks;

namespace DY.NET
{
    interface ISocketCoverAsync
    {
        Task<object> SendAsync(IProtocol protocol);
    }
}
