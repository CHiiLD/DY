namespace DY.NET
{
    public enum DeliveryError : int
    {
        SUCCESS = 0,
        WRITE_TIMEOUT = -1,
        READ_TIMEOUT = -2,
        DISCONNECT = -3
    }
}