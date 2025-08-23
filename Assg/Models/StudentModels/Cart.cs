using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assg.Models.StudentModels
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public int UserAccountId { get; set; }
        public UserAccount? Account { get; set; }
        public double ChapterContentPrice { get; set; }

        public int ChapterContentId { get; set; }
        public string ChapterContentName { get; set; }
        public string ChapterContentDescription { get; set; }
        [ForeignKey("ChapterContentId")]
        public ChapterContent? ChapterContent { get; set; }
        public double Subtotal { get; set; }
        public bool IsInCart { get; set; }
    }
}
