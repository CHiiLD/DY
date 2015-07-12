using System;
using System.Threading.Tasks;

namespace DY.NET
{
    interface IBarcodeSerialCommAsync : IDisposable
    {
        bool Connect();
        bool IsConnected();
        void Close();
        Task<object> ScanAsync();
        Task PrepareAsync();
    }
}