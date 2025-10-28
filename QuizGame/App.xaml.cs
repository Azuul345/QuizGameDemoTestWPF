using System.Windows;

namespace QuizGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // creates %LOCALAPPDATA%\QuizGame\Quizzs and seeds
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await QuizStorage.EnsureAppDataAsync();
        }
    }



}
