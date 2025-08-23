using System.ComponentModel.DataAnnotations;

namespace Assg.Models.StudentModels
{
    public class StudentVerifyEmail
    {
        [Required(ErrorMessage ="Email is required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
