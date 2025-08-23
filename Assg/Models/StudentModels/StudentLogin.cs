using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assg.Models.StudentModels
{
    public class StudentLogin
    {
        [Required(ErrorMessage = "Username or Email is required")]
        [MaxLength(20, ErrorMessage = "Maximum 20 characters allowed")]
        [DisplayName("Username or Email")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Maximum 20 or minimum 8 characters allowed")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }

    }
}
