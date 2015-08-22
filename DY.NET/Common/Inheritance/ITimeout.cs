namespace DY.NET
{
    public interface ITimeout
    {
        int WriteTimeoutMaximum { get; set; }
        int WriteTimeoutMinimum { get; set; }
        
        int ReadTimeoutMaximum { get; set; }
        int ReadTimeoutMinimum { get; set; }

        int WriteTimeout { get; set; }
        int ReadTimeout { get; set; }
    }
}