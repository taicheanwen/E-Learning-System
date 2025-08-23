using System.ComponentModel.DataAnnotations.Schema;

namespace Assg.Models.StudentModels
{
    public class ChatQuestion
    {
        public int Id { get; set; }
        
        public string QuestionTitle { get; set; }
        public string QuestionContent { get; set; }
        public int UserId { get; set; }
        public string? CourseId { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<ChatAnswer> Answers { get; set; }
        public int ChapterContentId { get; set; }
        [ForeignKey("ChapterContentId")]
        public ChapterContent? ChapterContent { get; set; }
        public Course? Course { get; set; }
        [ForeignKey("UserId")]
        public virtual UserAccount? UserAccount { get; set; }
    }
    
    public class ChatAnswer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("UserAccountId")]
        public virtual UserAccount? UserAccount { get; set; }
        public virtual ChatQuestion Question { get; set; }
    }
}
