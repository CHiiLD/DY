using System;
using System.Threading.Tasks;

namespace DY.NET
{
    interface IScannerSerialCommAsync : IConnect
    {
        Task<object> ScanAsync();
        Task PrepareAsync();
    }
}