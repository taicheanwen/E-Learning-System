using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assg.Models.StudentModels
{
    public class ChapterContentVM
    {
        public ChapterContent ChapterContent { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
        [Precision(5, 2)]
        public decimal CoursePrice { get; set; }
        public string VideoPath { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? UploadDateTime { get; set; }
        public string? SubcategoryId { get; set; }
        public string? SubcategoryName { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ChatQuestion> ChatQuestion { get; set; }
    }
}
