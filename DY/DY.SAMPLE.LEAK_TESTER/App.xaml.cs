using System.Windows;

namespace DY.SAMPLE.LEAK_TESTER
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ModelItemDirector.GetInstance().SaveToFile();
        }
    }
}
