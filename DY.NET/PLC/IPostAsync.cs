using System.Threading.Tasks;
using System.Threading;

namespace DY.NET
{
    /// <summary>
    /// 페러렐 비동기 Write/Read를 위한 인터페이스
    /// </summary>
    public interface IPostAsync : ITimeout
    {
        Task<Delivery> PostAsync(IProtocol request);
        Task<Delivery> PostAsync(IProtocol request, CancellationTokenSource token);
    }
}