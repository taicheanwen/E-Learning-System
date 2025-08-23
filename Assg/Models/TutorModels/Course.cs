//using System.ComponentModel.DataAnnotations;
//using Assg.Models.StudentModels;
//namespace Assg.Models.TutorModels;

//public class Course
//{
//    public string Id { get; set; }

//    [Required]
//    public string Name { get; set; }

//    [Range(0.01, double.MaxValue, ErrorMessage = "The price must be a positive value.")]
//    public decimal Price { get; set; }
//    public string? categoryId { get; set; }
//    public string? categoryName { get; set; }
//    public string? subcategoryId { get; set; }
//    public string? subcategoryName { get; set; }
//    public string? subtitle { get; set; }
//    public string? description { get; set; }
//    public int Participant { get; set; }
//    public DateTime? createdDate { get; set; }
//    public Category? Category { get; set; }
//    public ICollection<Chapter>? Chapters { get; set; }
//    public ICollection<QuizQuestion>? QuizQuestions { get; set; }
//    public ICollection<ChatQuestion>? ChatQuestions { get; set; }
//}
//public class Category
//{
//    [Key]
//    public string? CategoryId { get; set; }
//    public string? CategoryName { get; set; }
//    public string? SubcategoryId { get; set; }
//    public string? SubcategoryName { get; set; }

//    public ICollection<Course>? Courses { get; set; }
//}
