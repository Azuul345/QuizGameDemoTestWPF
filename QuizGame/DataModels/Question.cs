namespace QuizGame.DataModels
{
    public class Question
    {
        public string Statement { get; } //set
        public string[] Answers { get; } //set
        public int CorrectAnswer; // set

        public string? ImagePath { get; set; }
        public string? Subject { get; set; }

        //params = don't have to write new all the time
        public Question(string statment, int correctanswer, params string[] answer)
        {
            Statement = statment;
            Answers = answer;
            CorrectAnswer = correctanswer;

        }
        public bool isCorrect(int selectIndex)
        {
            return selectIndex == CorrectAnswer;
        }
    }
}
