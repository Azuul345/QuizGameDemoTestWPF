namespace QuizGame.DataModels
{
    public class Quiz
    {
        //public string Title { get; set; }

        public Random randomizer { get; set; }



        private List<Question> _questions;
        private string _title = string.Empty;
        public List<Question> Questions => _questions;
        public string Title => _title;

        public Quiz(string title = "")
        {
            _title = title;

            _questions = new List<Question>();
            randomizer = new Random();

        }

        public Question GetRandomQuestion()
        {
            if (Questions.Count == 0)
            {
                throw new InvalidOperationException("No questions available");
            }
            int index = randomizer.Next(0, Questions.Count);
            return Questions[index];
            //throw new NotImplementedException("A random Question needs to be returned here!");
        }

        public void AddQuestion(string statement, int correctAnswer, params string[] answers)
        {
            Question q = new Question(statement, correctAnswer, answers);
            Questions.Add(q);

        }

        public void RemoveQuestion(int index)
        {
            throw new NotImplementedException("Question at requested index need to be removed here!");
        }
    }
}
