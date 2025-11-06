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

        }


        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            var vm = (PlayQuizViewModel)DataContext;

            vm.ResetRun();

            // go back to play with the same quiz but fresh state
            Window.GetWindow(this)!.Content = new PLayQuizView(vm);
        }




        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new MenuView();
        }
    }
}
