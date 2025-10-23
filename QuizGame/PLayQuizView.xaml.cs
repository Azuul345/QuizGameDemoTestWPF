using System.Windows;
using System.Windows.Controls;

namespace QuizGame
{
    /// <summary>
    /// Interaction logic for PLayQuizView.xaml
    /// </summary>
    public partial class PLayQuizView : UserControl
    {
        public PlayQuizViewModel ViewModel { get; set; }
        public PLayQuizView()
        {
            InitializeComponent();
            ViewModel = new PlayQuizViewModel();
            DataContext = ViewModel;
        }
        public void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int selectIndex = int.Parse(button.Tag.ToString());
            ViewModel.NextQuestion(selectIndex);
        }

        //public void NextQuestion_Click(object sender, RoutedEventArgs e)
        //{
        //    ViewModel.NextQuestion();
        //}
    }
}
