
namespace DY.NET
{
    public enum DeliveryError : int
    {
        SUCCESS = 0,
        DISCONNECT = -1,
        WRITE_TIMEOUT = -2,
        READ_TIMEOUT = -3,
    }
}