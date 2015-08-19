using System;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IScannerSerialCommAsync : IConnect
    {
        Task<Delivery> ScanAsync();
        Task<Delivery> GetInfoAsync();
    }
}