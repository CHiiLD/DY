using System;
using System.Threading.Tasks;

namespace DY.NET
{
    interface IScannerSerialCommAsync : IConnect
    {
        Task<Delivery> ScanAsync();
        Task PrepareAsync();
    }
}