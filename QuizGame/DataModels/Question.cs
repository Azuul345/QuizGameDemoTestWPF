namespace QuizGame.DataModels
{
    public class Question
    {
        public string Statement { get; set; } = "";  // old { get; }
        public string[] Answers { get; set; } = Array.Empty<string>(); // old { get; } 
        public int CorrectAnswer { get; set; } // added { get; set; }

        public string? ImagePath { get; set; }
        public string? Subject { get; set; }

        //params = don't have to write new all the time
        public Question(string statment, int correctanswer, params string[] answer)
        {
            Statement = statment;
            Answers = answer;
            CorrectAnswer = correctanswer;

        }

        public Question() { }
        public bool isCorrect(int selectIndex)
        {
            return selectIndex == CorrectAnswer;
        }
    }
}
