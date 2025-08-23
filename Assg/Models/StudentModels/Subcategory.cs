using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assg.Models.StudentModels
{
    public class Subcategory
    {
        [Key]
        public int SubcategoryId { get; set; }

        public string SubcategoryName { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public List<Course> Course { get; set; } = new List<Course>();
    }

}
