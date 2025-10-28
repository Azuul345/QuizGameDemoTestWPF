namespace QuizGame.DataModels
{
    public class QuizDto
    {
        public string Title { get; set; } = "";
        public List<QuestionDto> Questions { get; set; } = new();
    }

    public class QuestionDto
    {
        public string Statement { get; set; } = "";
        public string[] Answers { get; set; } = Array.Empty<string>();
        public int CorrectAnswer { get; set; }
        public string? ImagePath { get; set; }
    }

}
