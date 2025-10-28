using QuizGame.DataModels;
using System.Windows;
using System.Windows.Controls;

namespace QuizGame.Views
{
    /// <summary>
    /// Interaction logic for EditView.xaml
    /// </summary>
    public partial class EditView : UserControl
    {
        private List<QuizDto> _all = new List<QuizDto>(); // ? 
        private Quiz _runtimeQuiz;
        private List<QuestionDto> _editableQuestions = new List<QuestionDto>();
        private QuizDto _current;                 // selected quiz
        private int _selectedIndex = -1;          // selected question index

        public EditView()
        {
            InitializeComponent();
            this.Loaded += EditView_Loaded;
        }

        private async void EditView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= EditView_Loaded;

            _all = await QuizStorage.LoadAllAsync();
            QuizPicker.ItemsSource = _all;
            if (QuizPicker.Items.Count > 0) QuizPicker.SelectedIndex = 0;
        }

        private void QuizPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuizPicker.SelectedItem is QuizDto dto)
            {
                _current = dto;

                _editableQuestions = _current.Questions.ToList();
                QuestionsList.ItemsSource = _editableQuestions;
                QuestionsList.SelectedIndex = -1;
                _selectedIndex = -1;

                RebuildRuntimeFromEditable();
                ClearForm();
            }
        }
        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            // read inputs without ?: or ??
            string s;
            if (StatementInput.Text == null)
            {
                s = "";
            }
            else
            {
                s = StatementInput.Text.Trim();
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
            if (Ans1Input.Text == null) a1 = "";
            else a1 = Ans1Input.Text.Trim();

            string a2;
            if (Ans2Input.Text == null) a2 = "";
            else a2 = Ans2Input.Text.Trim();

            if (s.Length == 0 || a0.Length == 0 || a1.Length == 0 || a2.Length == 0)
            {
                MessageBox.Show("Fill statement and all three answers.");
                return;
            }

            int correct = CorrectPick.SelectedIndex;
            if (correct < 0 || correct > 2)
            {
                MessageBox.Show("Pick the correct answer 1–3.");
                return;
            }

            string subject;
            if (SubjectInput.Text == null) subject = null;
            else
            {
                subject = SubjectInput.Text.Trim();
                if (subject.Length == 0) subject = null;
            }

            string image;
            if (ImageInput.Text == null) image = null;
            else
            {
                image = ImageInput.Text.Trim();
                if (image.Length == 0) image = null;
            }

            _editableQuestions.Add(new QuestionDto
            {
                Statement = s,
                Answers = new[] { a0, a1, a2 },
                CorrectAnswer = correct,
                Subject = subject,
                ImagePath = image
            });

            QuestionsList.Items.Refresh();
            QuestionsList.SelectedIndex = _editableQuestions.Count - 1;
            RebuildRuntimeFromEditable();
        }
        private void QuestionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedIndex = QuestionsList.SelectedIndex;
            if (_selectedIndex < 0) { ClearForm(); return; }

            var q = _editableQuestions[_selectedIndex];
            StatementInput.Text = q.Statement;
            Ans0Input.Text = q.Answers[0];
            Ans1Input.Text = q.Answers[1];
            Ans2Input.Text = q.Answers[2];

            // store as 1..3 in UI
            CorrectPick.SelectedIndex = q.CorrectAnswer; // CorrectAnswer is 0..2

            if (q.Subject == null) SubjectInput.Text = "";
            else SubjectInput.Text = q.Subject;

            if (q.ImagePath == null) ImageInput.Text = "";
            else ImageInput.Text = q.ImagePath;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (_current == null)
            {
                return;
            }
            if (_selectedIndex < 0)
            {
                MessageBox.Show("Select a question to update.");
                return;
            }
            // read fields
            string statement = GetTextOrEmpty(StatementInput);
            string a0 = GetTextOrEmpty(Ans0Input);
            string a1 = GetTextOrEmpty(Ans1Input);
            string a2 = GetTextOrEmpty(Ans2Input);

            if (statement.Length == 0 || a0.Length == 0 || a1.Length == 0 || a2.Length == 0)
            {
                MessageBox.Show("Fill statement and all three answers.");
                return;
            }

            int correct = ReadCorrectIndex();     // returns 0..2 or -1
            if (correct < 0) { MessageBox.Show("Pick correct answer 1, 2 or 3."); return; }

            string subject = GetTrimOrNull(SubjectInput);
            string image = GetTrimOrNull(ImageInput);

            // update dto
            var q = _editableQuestions[_selectedIndex];
            q.Statement = statement;
            q.Answers = new[] { a0, a1, a2 };
            q.CorrectAnswer = correct;
            q.Subject = subject;
            q.ImagePath = image;

            QuestionsList.Items.Refresh();
            RebuildRuntimeFromEditable();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedIndex < 0)
            {
                MessageBox.Show("Select a question to delete.");
                return;
            }
            if (_selectedIndex >= _editableQuestions.Count)
            {
                return;
            }

            //_editableQuestions.RemoveAt(_selectedIndex);
            //QuestionsList.Items.Refresh();

            if (_runtimeQuiz != null)
            {
                _runtimeQuiz.RemoveQuestion(_selectedIndex); // ADDED: use your method

                // ADDED: rebuild editable list from runtime quiz after removal
                _editableQuestions.Clear();
                for (int i = 0; i < _runtimeQuiz.Questions.Count; i++)
                {
                    var rq = _runtimeQuiz.Questions[i];
                    var dto = new QuestionDto();
                    dto.Statement = rq.Statement;

                    // copy three answers safely
                    string a0 = "";
                    string a1 = "";
                    string a2 = "";
                    if (rq.Answers != null && rq.Answers.Length > 0) a0 = rq.Answers[0];
                    if (rq.Answers != null && rq.Answers.Length > 1) a1 = rq.Answers[1];
                    if (rq.Answers != null && rq.Answers.Length > 2) a2 = rq.Answers[2];
                    dto.Answers = new[] { a0, a1, a2 };

                    dto.CorrectAnswer = rq.CorrectAnswer;
                    if (rq.Subject == null) dto.Subject = null; else dto.Subject = rq.Subject;
                    if (rq.ImagePath == null) dto.ImagePath = null; else dto.ImagePath = rq.ImagePath;

                    _editableQuestions.Add(dto);
                }

                QuestionsList.Items.Refresh();
            }

            QuestionsList.SelectedIndex = -1;
            _selectedIndex = -1;

            // clear the form fields
            StatementInput.Text = "";
            Ans0Input.Text = "";
            Ans1Input.Text = "";
            Ans2Input.Text = "";
            SubjectInput.Text = "";
            ImageInput.Text = "";
            CorrectPick.SelectedIndex = 0;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            // allow title change in picker
            if (QuizPicker.SelectedItem is QuizDto dto)
            {
                _current.Title = dto.Title; // picker displays Title; keep as-is

            }
            //_current.Questions = _editableQuestions.ToArray();
            _current.Questions = _editableQuestions.ToList();
            await QuizStorage.SaveAsync(_current);
            MessageBox.Show("Quiz saved.");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new MenuView();
        }

        private static string GetTextOrEmpty(TextBox tb)
        {
            if (tb.Text == null)
            {
                return "";
            }
            else
            {
                return tb.Text.Trim();
            }
        }
        private static string GetTrimOrNull(TextBox tb)
        {
            if (tb.Text == null)
            {
                return null;
            }
            string s = tb.Text.Trim();
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            else
            {
                return s;
            }
        }

        private int ReadCorrectIndex()
        {
            if (CorrectPick.SelectedItem is ComboBoxItem item)
            {
                string s = item.Content.ToString();
                if (int.TryParse(s, out int oneBased))
                {
                    int idx = oneBased - 1; // convert 1..3 -> 0..2
                    if (idx >= 0 && idx <= 2) return idx;
                }
            }
            return -1;
        }
        private void ClearForm()
        {
            StatementInput.Text = "";
            Ans0Input.Text = "";
            Ans1Input.Text = "";
            Ans2Input.Text = "";
            SubjectInput.Text = "";
            ImageInput.Text = "";
            CorrectPick.SelectedIndex = 0;
        }

        private void RebuildRuntimeFromEditable() // ADDED
        {
            if (_current == null)
            {
                _runtimeQuiz = null;
                return;
            }

            _runtimeQuiz = new Quiz(_current.Title);

            for (int i = 0; i < _editableQuestions.Count; i++)
            {
                var d = _editableQuestions[i];
                var rq = new Question(d.Statement, d.CorrectAnswer, d.Answers);

                if (d.ImagePath == null) rq.ImagePath = null;
                else rq.ImagePath = d.ImagePath;

                if (d.Subject == null) rq.Subject = null;
                else rq.Subject = d.Subject;

                _runtimeQuiz.Questions.Add(rq);
            }
        }
    }
}
