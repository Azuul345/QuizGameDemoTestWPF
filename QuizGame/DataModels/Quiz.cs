namespace QuizGame.DataModels
{
    public class Quiz
    {
        //public string Title { get; set; }

        public Random randomizer = new();  // old   { get; set; }


        public string Title { get; set; } = "";
        public List<Question> Questions { get; set; } = new();

        //added property
        List<Question> questionAsked = new List<Question>();
        public int TotalQuestions => Questions.Count;
        public int AskedCount => questionAsked.Count;



        public Quiz(string title = "")
        {
            Title = title;
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




        public void AddQuestion(string statement, int correctAnswer, string? imagePath, params string[] answers)
        {
            Question q = new Question(statement, correctAnswer, answers);
            if (!string.IsNullOrWhiteSpace(imagePath)) q.ImagePath = imagePath;

            Questions.Add(q);

        }


        public void RemoveQuestion(int index)
        {
            if (index < 0) return;
            if (index >= Questions.Count) return;

            var q = Questions[index];
            Questions.RemoveAt(index);

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
