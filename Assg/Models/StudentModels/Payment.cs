using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assg.Models.StudentModels
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public int ChapterContentId { get; set; }
        [ForeignKey("ChapterContentId")]
        public ChapterContent? ChapterContent { get; set; }
        public Cart? Cart { get; set; }
        public Course? Course { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}-\d{4}$", ErrorMessage = "Card number must be in the format XXXX-XXXX-XXXX-XXXX.")]
        public string CardNo { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be a 3-digit number.")]
        public int CVV { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Cardholder name must contain only letters and spaces.")]
        public string CardHolderName { get; set; }
        [Required]
        public DateOnly CardExpiredDate { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be positive.")]
        public decimal PaymentAmount { get; set; }

        public ICollection<Review>? Reviews { get; set; }
    }
}
