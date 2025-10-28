using System.Windows;
using System.Windows.Controls;
using static QuizGame.PlayQuizViewModel;

namespace QuizGame.Views
{
    /// <summary>
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    /// 
    public class ResultsViewModel // simple View Model just for this screen
    {
        public string ScoreText { get; set; }

        public List<WrongAnswerItem> WrongAnswers { get; set; }
    }
    public partial class ResultsView : UserControl
    {
        private PlayQuizViewModel _sourceVm; //Source View Model 
        public ResultsView(PlayQuizViewModel viewModel)
        {
            InitializeComponent();
            _sourceVm = viewModel;

            DataContext = viewModel;
            //    new ResultsViewModel
            //{
            //    ScoreText = viewModel.ScoreText,
            //    WrongAnswers = viewModel.WrongAnswers
            //};
        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            var vm = (PlayQuizViewModel)DataContext;

            vm.TotalAnswered = 0;
            vm.CorrectAnswers = 0;
            vm.WrongAnswers.Clear();
            vm.Quiz.RestartQuiz();
            vm.CurrentQuestion = vm.Quiz.GetRandomQuestion();
            vm.OnProperyChanged("CurrentQuestion");
            vm.OnProperyChanged("ScoreText");

            // IMPORTANT: pass vm into the Play view
            Window.GetWindow(this)!.Content = new PLayQuizView(vm);

            // Reset quiz state and start again
            //_sourceVm.Quiz.RestartQuiz();             // ADDED uses Quiz.RestartQuiz()
            //var newPlay = new PLayQuizView(vm);         // start fresh
            //Window.GetWindow(this).Content = newPlay;
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new MenuView();
        }
    }
}
