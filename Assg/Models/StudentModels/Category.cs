using System.ComponentModel.DataAnnotations;

namespace Assg.Models.StudentModels
{
    public class Category
    {
        [Key]
        public string? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string? SubcategoryId { get; set; }

        public string? SubcategoryName { get; set; }
        public ICollection<Course>? Courses { get; set; }


    }
}
