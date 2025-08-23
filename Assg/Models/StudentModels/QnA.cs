using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assg.Models.StudentModels
{
    public class QnA
    {
        [Key]
        public int QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? TutorId { get; set; }
        public string? TutorName { get; set; }
        public string? TutorReply { get; set; }
        public int UserAccountId { get; set; }
        public UserAccount? UserAccount { get; set; }
        public int CourseId { get; set; }
        public Course? Course { get; set; }

    }
}
