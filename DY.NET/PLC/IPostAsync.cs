using System.Threading.Tasks;

namespace DY.NET
{
    interface IPostAsync
    {
        Task<IProtocol> PostAsync(IProtocol protocol);
    }
}