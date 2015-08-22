using System;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IScannerAsync : IConnect
    {
        Task<Delivery> ScanAsync();
        Task<Delivery> GetInfoAsync();
    }
}