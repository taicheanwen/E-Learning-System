namespace Assg.Models.StudentModels
{
    public class ChapterContent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; }
        public byte[]? FileData { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string courseId { get; set; }
        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }
        public ICollection<QuizQuestion>? QuizQuestions { get; set; } = new List<QuizQuestion>();
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ChatQuestion> ChatQuestion { get; set; }
    }

    public class QuizQuestion
    {
        public string? Id { get; set; }
        public string? name { get; set; }
        public string? courseId { get; set; }
        public string? description { get; set; }
        public int? ChapterContentId { get; set; }
        public DateTime createdDate { get; set; }
        public List<Questions> questions { get; set; }
        public ChapterContent ChapterContent { get; set; }
        public Course course { get; set; }
        public ICollection<QuizAnswer> QuizAnswers { get; set; }
    }

    public class Questions
    {
        public int Id { get; set; }
        public string quizId { get; set; }
        public string? Question { get; set; }
        public string? CorrectAnswer { get; set; }
        public List<string>? Options { get; set; } = new List<string>();
        public byte[]? FileData { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public QuizQuestion quizQuestion { get; set; }
    }
}
