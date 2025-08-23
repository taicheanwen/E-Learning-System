using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assg.Models.StudentModels
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int? ReviewRate { get; set; }
        public string? ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
        public int PaymentId { get; set; }
        public Payment? Payment { get; set; }
        public int ChapterContentId { get; set; }
        [ForeignKey("ChapterContentId")]
        public ChapterContent? ChapterContent { get; set; }
        public int UserId { get; set; }
    }
}
