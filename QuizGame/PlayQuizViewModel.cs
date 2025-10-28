using QuizGame.DataModels;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace QuizGame
{
    public class PlayQuizViewModel : INotifyPropertyChanged
    {
        public Quiz Quiz { get; set; }

        public Question CurrentQuestion { get; set; }

        public int SelectAnswerIndex { get; set; }

        public int CorrectAnswers { get; set; }

        public int TotalAnswered { get; set; }


        //added properties
        public List<WrongAnswerItem> WrongAnswers { get; } = new();
        public int CorrectCount { get; set; }

        public Dictionary<string, int> WrongBySubject { get; } = new(StringComparer.OrdinalIgnoreCase);



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

                // ADDED: store info about the wrong answer for results screen later
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
            }

            //SelectAnswerIndex = -1;
            CurrentQuestion = Quiz.GetRandomQuestion();
            OnProperyChanged("CurrentQuestion");
            OnProperyChanged("ScoreText");
        }

        public PlayQuizViewModel(QuizDto dto)
        {
            Quiz = new Quiz(dto.Title);

            string baseName = new string(dto.Title.Where(c => !char.IsWhiteSpace(c)).ToArray());
            string quizFolder = Path.Combine(QuizStorage.QuizzFolder, baseName + "Img");
            Directory.CreateDirectory(quizFolder);

            foreach (var q in dto.Questions)
            {
                string? resolved = null;


                if (!string.IsNullOrWhiteSpace(q.ImagePath))
                {
                    bool isUrl =
                        q.ImagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        q.ImagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

                    if (isUrl)
                    {
                        // URL image → use directly
                        resolved = q.ImagePath;
                    }
                    else
                    {
                        // Local image in quiz folder
                        resolved = Path.Combine(quizFolder, q.ImagePath);
                    }
                }
                var runtimeQ = new Question(q.Statement, q.CorrectAnswer, q.Answers);
                runtimeQ.ImagePath = resolved;
                runtimeQ.Subject = q.Subject;
                Quiz.Questions.Add(runtimeQ);

            }

            CurrentQuestion = Quiz.GetRandomQuestion();
            SelectAnswerIndex = -1;
            OnProperyChanged("CurrentQuestion");
        }

    }
}


