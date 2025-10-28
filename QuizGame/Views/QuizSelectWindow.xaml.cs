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

        private List<QuizDto> _quizzs = new(); // ADDED
        //new
        private List<QuizDto> _all = new();
        public QuizSelectWindow()
        {
            InitializeComponent();
            this.Loaded += QuizSelectWindow_Loaded; // ADDED

            //var subjects = _all
            //.SelectMany(q => q.Questions.Select(qq => qq.Subject ?? ""))
            //.Where(x => !string.IsNullOrWhiteSpace(x))
            //.Distinct(StringComparer.OrdinalIgnoreCase)
            //.OrderBy(x => x)
            //.ToList();
            //subjects.Insert(0, "All");
            //SubjectFilter.ItemsSource = subjects;
            //SubjectFilter.SelectedIndex = 0;

            //QuizList.ItemsSource = _all;
            //if (QuizList.Items.Count > 0) QuizList.SelectedIndex = 0;
        }

        private void SubjectFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubjectFilter.SelectedItem is not string pick || pick == "All")
            {
                QuizList.ItemsSource = _all;
                return;
            }

            // keep it simple: a quiz appears if ANY question has that subject
            var filtered = _all.Where(q => q.Questions.Any(qq =>
                               string.Equals(qq.Subject, pick, StringComparison.OrdinalIgnoreCase)));
            QuizList.ItemsSource = filtered.ToList();
            if (QuizList.Items.Count > 0) QuizList.SelectedIndex = 0;
        }

        private async void QuizSelectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= QuizSelectWindow_Loaded;
            //var _quizzs = await QuizStorage.LoadAllAsync();   // async per krav
            //QuizList.ItemsSource = _quizzs;
            //if (QuizList.Items.Count > 0) QuizList.SelectedIndex = 0;
            _all = await QuizStorage.LoadAllAsync();   // FIX: assign to _all
            QuizList.ItemsSource = _all;
            if (QuizList.Items.Count > 0) QuizList.SelectedIndex = 0;

            // build subject list from _all AFTER load
            var subjects = _all
                .SelectMany(q => q.Questions)
                .Select(qq => qq.Subject)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            subjects.Insert(0, "All");
            SubjectFilter.ItemsSource = subjects;
            SubjectFilter.SelectedIndex = 0;
        }


        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (QuizList.SelectedItem is not QuizDto dto) return;

            var pick = SubjectFilter.SelectedItem as string;

            if (!string.IsNullOrWhiteSpace(pick) && pick != "All")
            {
                var filtered = new QuizDto
                {
                    Title = dto.Title,
                    Questions = dto.Questions
                        .Where(q => string.Equals(q.Subject, pick, StringComparison.OrdinalIgnoreCase))
                        .ToList()
                };

                if (filtered.Questions.Count == 0)
                {
                    MessageBox.Show("No questions for subject: " + pick);
                    return;
                }

                var vmFiltered = new PlayQuizViewModel(filtered);
                Window.GetWindow(this)!.Content = new PLayQuizView(vmFiltered);
                return;
            }
            var vm = new PlayQuizViewModel(dto);
            Window.GetWindow(this)!.Content = new PLayQuizView(vm);
        }



        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new MenuView();
        }


    }
}



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