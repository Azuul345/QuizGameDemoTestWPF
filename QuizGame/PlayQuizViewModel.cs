using QuizGame.DataModels;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace QuizGame
{
    // It notifies the UI when properties change.
    public class PlayQuizViewModel : INotifyPropertyChanged
    {
        public Quiz Quiz { get; set; }

        public Question CurrentQuestion { get; set; }

        public int SelectAnswerIndex { get; set; }

        public int CorrectAnswers { get; set; }

        public int TotalAnswered { get; set; }


        //added properties

        public List<WrongAnswerItem> WrongAnswers { get; private set; } = new();
        public Dictionary<string, int> WrongBySubject { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        public int CorrectCount { get; set; }




        public class WrongAnswerItem
        {
            public string Statement { get; set; }
            public string UserAnswer { get; set; }
            public string CorrectAnswer { get; set; }
            public string? ImagePath { get; set; }
        }

        public string ScoreText
        {
            get
            {
                int percent = 0;
                if (TotalAnswered > 0)
                {
                    percent = (int)((double)CorrectAnswers / TotalAnswered * 100);
                }
                return $"Correct Answers {CorrectAnswers} / {TotalAnswered} ({percent}) %";
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnProperyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public void NextQuestion(int selectedIndex)
        {
            TotalAnswered++;
            if (CurrentQuestion.isCorrect(selectedIndex))
            {
                CorrectAnswers++;
            }
            else
            {


                WrongAnswers.Add(new WrongAnswerItem
                {
                    Statement = CurrentQuestion.Statement,
                    UserAnswer = CurrentQuestion.Answers[selectedIndex],
                    CorrectAnswer = CurrentQuestion.Answers[CurrentQuestion.CorrectAnswer],
                    ImagePath = CurrentQuestion.ImagePath,

                });

                string subj;

                if (string.IsNullOrWhiteSpace(CurrentQuestion.Subject))
                {
                    subj = "Unspecified";
                }
                else
                {
                    subj = CurrentQuestion.Subject;
                }

                if (WrongBySubject.ContainsKey(subj))
                {
                    WrongBySubject[subj]++;
                }
                else
                {
                    WrongBySubject[subj] = 1;
                }
                OnProperyChanged(nameof(WrongBySubject));
                OnProperyChanged(nameof(WrongAnswers));
            }

            //SelectAnswerIndex = -1;
            CurrentQuestion = Quiz.GetRandomQuestion();
            OnProperyChanged("CurrentQuestion");
            OnProperyChanged("ScoreText");
        }



        public PlayQuizViewModel(Quiz quiz)
        {
            // 1. If it’s a mixed quiz  use it
            if (quiz.Title.StartsWith("Mixed ", StringComparison.OrdinalIgnoreCase))
            {

                Quiz = quiz;
            }
            else
            {
                // 2. Normal quiz loaded from disk → rebuild like before
                Quiz = new Quiz(quiz.Title);

                string baseName = new string(quiz.Title.Where(c => !char.IsWhiteSpace(c)).ToArray());
                string quizFolder = Path.Combine(QuizStorage.QuizzFolder, baseName + "Img");
                Directory.CreateDirectory(quizFolder);

                foreach (var q in quiz.Questions)
                {
                    string? resolved = null;

                    if (!string.IsNullOrWhiteSpace(q.ImagePath))
                    {
                        bool isUrl =
                            q.ImagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                            q.ImagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

                        if (isUrl)
                        {
                            resolved = q.ImagePath;
                        }
                        else
                        {
                            resolved = Path.Combine(quizFolder, q.ImagePath);
                        }
                    }

                    // IMPORTANT: use ToArray() here
                    var runtimeQ = new Question(q.Statement, q.CorrectAnswer, q.Answers.ToArray());
                    runtimeQ.ImagePath = resolved;
                    runtimeQ.Subject = q.Subject;
                    Quiz.Questions.Add(runtimeQ);
                }
            }

            // 3. ALWAYS start a fresh run for this VM
            WrongAnswers.Clear();
            WrongBySubject.Clear();
            TotalAnswered = 0;
            CorrectAnswers = 0;

            // 4. pick first question
            CurrentQuestion = Quiz.GetRandomQuestion();
            SelectAnswerIndex = -1;

            // 5. notify UI
            OnProperyChanged(nameof(CurrentQuestion));
            OnProperyChanged(nameof(ScoreText));
        }


        public void ResetRun()
        {
            // new instances → WPF must rebind
            WrongAnswers = new List<WrongAnswerItem>();
            WrongBySubject = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            TotalAnswered = 0;
            CorrectAnswers = 0;

            Quiz.RestartQuiz();
            CurrentQuestion = Quiz.GetRandomQuestion();
            SelectAnswerIndex = -1;

            OnProperyChanged(nameof(WrongAnswers));
            OnProperyChanged(nameof(WrongBySubject));
            OnProperyChanged(nameof(CurrentQuestion));
            OnProperyChanged(nameof(ScoreText));
        }


        public static string? FindOriginalImagePath(Question q)
        {
            // Try each quiz folder under Quizzs\*Img
            // 1. no question or no image → nothing to do
            if (q == null || string.IsNullOrWhiteSpace(q.ImagePath))
                return null;

            // 2. URL → use as-is
            if (q.ImagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                q.ImagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return q.ImagePath;

            // 3. look under ...\Quizzs for any *Img folder
            string quizzRoot = QuizStorage.QuizzFolder;
            if (!Directory.Exists(quizzRoot))
                return null;

            foreach (var folder in Directory.EnumerateDirectories(quizzRoot, "*Img", SearchOption.TopDirectoryOnly))
            {
                string candidate = Path.Combine(folder, q.ImagePath);
                if (File.Exists(candidate))
                    return candidate;
            }

            // 4. not found anywhere
            return null;
        }





    }
}


