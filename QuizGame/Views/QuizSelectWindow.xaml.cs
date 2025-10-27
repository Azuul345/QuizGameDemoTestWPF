using System.Windows;
using System.Windows.Controls;

namespace QuizGame.Views
{
    /// <summary>
    /// Interaction logic for QuizSelectWindow.xaml
    /// </summary>
    public partial class QuizSelectWindow : UserControl
    {
        public PlayQuizViewModel ViewModel { get; set; }
        public QuizSelectWindow()
        {
            InitializeComponent();
            var quizNames = new List<string>
            {
                "Test" // add more names here later
            };
            QuizList.ItemsSource = quizNames;
            if (QuizList.Items.Count > 0) QuizList.SelectedIndex = 0;
        }


        private void Play_Click(object sender, RoutedEventArgs e)
        {
            // NOTE: Later you can pass the selected name to ViewModel if needed.
            if (QuizList.SelectedItem is null) return;

            // CHANGED: Navigate to the actual play screen
            Window.GetWindow(this).Content = new PLayQuizView();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new MenuView();
        }


    }
}
