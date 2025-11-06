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

        private List<Quiz> _quizzs = new(); // ADDED
        //new
        private List<Quiz> _all = new();
        public QuizSelectWindow()
        {
            InitializeComponent();
            this.Loaded += QuizSelectWindow_Loaded; // ADDED


        }

        private void SubjectFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            // collect selected subjects
            var selectedSubjects = SubjectFilter.SelectedItems
                .OfType<string>()
                .Where(s => s != "All")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // no selection or All → show all quizzes
            if (selectedSubjects.Count == 0)
            {
                QuizList.ItemsSource = _all;
                return;
            }

            // show quizzes that contain ANY of the selected subjects
            var filteredQuizzes = _all
                .Where(q => q.Questions.Any(qq =>
                            !string.IsNullOrWhiteSpace(qq.Subject) &&
                            selectedSubjects.Contains(qq.Subject)))
                .ToList();

            QuizList.ItemsSource = filteredQuizzes;
            if (QuizList.Items.Count > 0) QuizList.SelectedIndex = 0;

        }

        private async void QuizSelectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= QuizSelectWindow_Loaded;

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
            //  collect selected subjects
            var selectedSubjects = SubjectFilter.SelectedItems
                .OfType<string>()
                .Where(s => s != "All")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // CASE A: user picked at least one subject, build mixed quiz
            if (selectedSubjects.Count > 0)
            {
                // collect all matching questions from all quizzes
                var matchedQuestions =
                    (from quiz in _all
                     from q in quiz.Questions
                     where !string.IsNullOrWhiteSpace(q.Subject)
                           && selectedSubjects.Contains(q.Subject)
                     // build new Question to safely modify paths
                     let resolvedImage = PlayQuizViewModel.FindOriginalImagePath(q)
                     select new Question(q.Statement, q.CorrectAnswer, q.Answers.ToArray())
                     {
                         Subject = q.Subject,
                         ImagePath = PlayQuizViewModel.FindOriginalImagePath(q)
                     })
                    // 🔹 remove duplicates by statement text (case-insensitive, trimmed)
                    .GroupBy(q => q.Statement.Trim(), StringComparer.OrdinalIgnoreCase)
                    .Select(g => g.First())
                    .ToList();

                if (matchedQuestions.Count == 0)
                {
                    MessageBox.Show("No questions found for selected subjects.");
                    return;
                }

                var mixedQuiz = new Quiz
                {
                    Title = "Mixed " + string.Join("_", selectedSubjects),
                    Questions = matchedQuestions
                };

                var vmOne = new PlayQuizViewModel(mixedQuiz);
                Window.GetWindow(this)!.Content = new PLayQuizView(vmOne);
                return;
            }

            // CASE B: no subjects selected → fallback to selected quiz
            if (QuizList.SelectedItem is not Quiz pickQuiz)
                return;

            var vmTwo = new PlayQuizViewModel(pickQuiz);
            Window.GetWindow(this)!.Content = new PLayQuizView(vmTwo);
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new MenuView();
        }


    }
}


