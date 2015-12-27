using System.Threading.Tasks;

namespace DY.NET
{
    public interface IAsyncCommand
    {
        bool CanExecute(object parameter);
        Task ExecuteAsync(object parameter);
    }
}