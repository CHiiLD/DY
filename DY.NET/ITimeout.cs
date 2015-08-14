namespace DY.NET
{
    public interface ITimeout
    {
        int WriteTimeout { get; set; }
        int ReadTimeout { get; set; }
    }
}