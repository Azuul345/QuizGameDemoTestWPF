using QuizGame.DataModels;
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
        //public QuizSelectWindow()
        //{
        //    InitializeComponent();
        //    var quizNames = new List<string>
        //    {
        //        "Test" // add more names here later
        //    };
        //    QuizList.ItemsSource = quizNames;
        //    if (QuizList.Items.Count > 0) QuizList.SelectedIndex = 0;
        //}
        private List<QuizDto> _quizzs = new(); // ADDED

        public QuizSelectWindow()
        {
            InitializeComponent();
            this.Loaded += QuizSelectWindow_Loaded; // ADDED
        }

        private async void QuizSelectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= QuizSelectWindow_Loaded;
            var _quizzs = await QuizStorage.LoadAllAsync();   // async per krav
            QuizList.ItemsSource = _quizzs;
            if (QuizList.Items.Count > 0) QuizList.SelectedIndex = 0;
        }


        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (QuizList.SelectedItem is not QuizDto dto) return;

            var vm = new PlayQuizViewModel(dto);
            Window.GetWindow(this)!.Content = new PLayQuizView(vm);
        }

        //private void QuizList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (QuizList.SelectedItem is not QuizDto dto) return;

        //    var vm = new PlayQuizViewModel(dto);          // build runtime quiz
        //    Window.GetWindow(this).Content = new PLayQuizView(vm); // navigate
        //}


        //private void Play_Click(object sender, RoutedEventArgs e)
        //{
        //    // NOTE: Later you can pass the selected name to ViewModel if needed.
        //    if (QuizList.SelectedItem is null) return;

        //    // CHANGED: Navigate to the actual play screen
        //    Window.GetWindow(this).Content = new PLayQuizView();
        //}

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new MenuView();
        }


    }
}
