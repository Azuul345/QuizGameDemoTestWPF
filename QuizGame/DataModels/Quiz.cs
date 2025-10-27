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


        //added property
        List<Question> questionAsked = new List<Question>();
        public int TotalQuestions => Questions.Count;
        public int AskedCount => questionAsked.Count;

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

            if (questionAsked.Count == Questions.Count)
            {
                return null;
            }

            while (true)
            {
                int index = randomizer.Next(0, Questions.Count);
                if (!questionAsked.Contains(Questions[index]))
                {
                    questionAsked.Add(Questions[index]);
                    return Questions[index];
                }
            }
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

        public void RestartQuiz()
        {
            questionAsked.Clear();
        }
    }
}
