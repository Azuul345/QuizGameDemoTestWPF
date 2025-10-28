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
        public void AddQuestionWithImage(string statement, int correctAnswer, string? imagePath, params string[] answers)
        {
            var q = new Question(statement, correctAnswer, answers);
            q.ImagePath = imagePath;
            Questions.Add(q);
        }


        public void RemoveQuestion(int index)
        {
            if (index < 0) return;
            if (index >= _questions.Count) return;

            var q = _questions[index];
            _questions.RemoveAt(index);

            if (questionAsked != null)
            {
                int askedIndex = questionAsked.IndexOf(q);
                if (askedIndex >= 0) questionAsked.RemoveAt(askedIndex);
            }
        }

        public void RestartQuiz()
        {
            questionAsked.Clear();
        }
    }
}
