using QuizGame.DataModels;
using System.ComponentModel;
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

        public string ScoreText
        {
            get
            {
                int percent = 0;
                if (TotalAnswered > 0)
                {
                    percent = (int)((double)CorrectAnswers / TotalAnswered * 100);
                }
                return $"Correct Answers {CorrectAnswers} / {TotalAnswered} ({percent}) ";
            }
        }

        //public string ScoreText { get; set; }

        public PlayQuizViewModel()
        {
            Quiz = new Quiz("Test");
            Quiz.AddQuestion("Capital of Sweden", 0, "Stockholm", "Gbg", "mlm");
            Quiz.AddQuestion("Color of skye", 2, "Red", "Yellow", "Blue");
            Quiz.AddQuestion("Cat legs", 1, "5", "4", "3");

            CurrentQuestion = Quiz.GetRandomQuestion();
            SelectAnswerIndex = -1;
            OnProperyChanged("CurrentQuestion");
            //OnProperyChanged("ScoreText");


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
            //SelectAnswerIndex = -1;
            CurrentQuestion = Quiz.GetRandomQuestion();
            OnProperyChanged("CurrentQuestion");
            OnProperyChanged("ScoreText");
        }

    }
}
