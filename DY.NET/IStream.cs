using System.Threading.Tasks;

namespace DY.NET
{
    public interface IStream : ITimeout
    {
        Task<int> WriteAsync(byte[] buffer, int offset, int count);
        Task<int> ReadAsync(byte[] buffer, int offset, int count);
    }
}
