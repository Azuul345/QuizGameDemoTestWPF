using System.Windows;
using System.Windows.Controls;
using static QuizGame.PlayQuizViewModel;

namespace QuizGame.Views
{
    /// <summary>
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    /// 
    public class ResultsViewModel // ADDED: simple VM just for this screen
    {
        public string ScoreText { get; set; }
        //AI help to find this
        public List<WrongAnswerItem> WrongAnswers { get; set; }
    }
    public partial class ResultsView : UserControl
    {
        private PlayQuizViewModel _sourceVm; //Source View Model 
        public ResultsView(PlayQuizViewModel viewModel)
        {
            InitializeComponent();
            _sourceVm = viewModel;

            DataContext = new ResultsViewModel
            {
                ScoreText = viewModel.ScoreText,
                WrongAnswers = viewModel.WrongAnswers
            };
        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            // Reset quiz state and start again
            _sourceVm.Quiz.RestartQuiz();             // ADDED uses Quiz.RestartQuiz()
            var newPlay = new PLayQuizView();         // start fresh
            Window.GetWindow(this).Content = newPlay;
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new MenuView();
        }
    }
}
