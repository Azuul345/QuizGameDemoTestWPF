using QuizGame.DataModels;
using System.Windows;
using System.Windows.Controls;

namespace QuizGame.Views
{
    /// <summary>
    /// Interaction logic for CreateView.xaml
    /// </summary>
    public partial class CreateView : UserControl
    {
        private readonly List<Question> _buffer = new();
        public CreateView()
        {
            InitializeComponent();
            QuestionsList.ItemsSource = _buffer;
        }
        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            // read inputs
            string statement;
            if (StatementInput.Text == null)
            {
                statement = "";
            }
            else
            {
                statement = StatementInput.Text.Trim();
            }

            string a0;
            if (Ans0Input.Text == null)
            {
                a0 = "";
            }
            else
            {
                a0 = Ans0Input.Text.Trim();
            }

            string a1;
            if (Ans1Input.Text == null)
            {
                a1 = "";
            }
            else
            {
                a1 = Ans1Input.Text.Trim();
            }

            string a2;
            if (Ans2Input.Text == null)
            {
                a2 = "";
            }
            else
            {
                a2 = Ans2Input.Text.Trim();
            }

            if (CorrectIndexInput.SelectedItem == null)
            {
                MessageBox.Show("Select the correct answer index (1, 2, or 3).");
                return;
            }

            var item = (ComboBoxItem)CorrectIndexInput.SelectedItem;
            int correct;
            if (!int.TryParse(item.Content.ToString(), out correct))
            {
                MessageBox.Show("Correct index must be a number (1, 2, or 3).");
                return;
            }
            string subject;
            if (string.IsNullOrWhiteSpace(SubjectInput.Text))
            {
                subject = null;
            }
            else
            {
                subject = SubjectInput.Text.Trim();
            }

            string imageUrl;
            if (string.IsNullOrWhiteSpace(ImageUrlInput.Text))
            {
                imageUrl = null;
            }
            else
            {
                imageUrl = ImageUrlInput.Text.Trim();
            }

            // minimal checks
            if (statement.Length == 0 || a0.Length == 0 || a1.Length == 0 || a2.Length == 0)
            {
                MessageBox.Show("Fill statement and all three answers.");
                return;
            }
            if (correct < 1 || correct > 3)
            {
                MessageBox.Show("Correct index must be 1, 2, or 3.");
                return;
            }
            correct = correct - 1;

            // add to buffer
            _buffer.Add(new Question
            {
                Statement = statement,
                Answers = new[] { a0, a1, a2 },
                CorrectAnswer = correct,
                Subject = subject,
                ImagePath = imageUrl     // URL allowed. For local images, paste a filename.
            });

            QuestionsList.Items.Refresh();

            // clear inputs for next question
            StatementInput.Clear();
            Ans0Input.Clear();
            Ans1Input.Clear();
            Ans2Input.Clear();
            SubjectInput.Clear();
            ImageUrlInput.Clear();
            CorrectIndexInput.SelectedIndex = 0;
            StatementInput.Focus();
        }

        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            var q = QuestionsList.SelectedItem as Question;
            if (q != null)
            {
                _buffer.Remove(q);
                QuestionsList.Items.Refresh();
            }
        }
        //
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            string title;
            if (TitleInput.Text == null)
            {
                title = "";
            }
            else
            {
                title = TitleInput.Text.Trim();
            }

            if (title.Length == 0)
            {
                MessageBox.Show("Enter a quiz title.");
                return;
            }

            if (_buffer.Count == 0)
            {
                MessageBox.Show("Add at least one question.");
                return;
            }

            // build data transfer object and save
            var quizDto = new Quiz
            {
                Title = title,
                Questions = _buffer.ToList()
            };

            await QuizStorage.SaveAsync(quizDto);
            MessageBox.Show("Quiz saved.");

            var win = Window.GetWindow(this);
            if (win != null)
            {
                win.Content = new QuizSelectWindow(); // reload list on next screen load
            }
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new MenuView();
        }

        // Dark Magic going on here Looks empty but when selecting the right answer it works. leaving it as is. 
        private void CorrectIndexInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
