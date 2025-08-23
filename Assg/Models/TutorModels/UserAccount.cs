using System.ComponentModel.DataAnnotations;
using Assg.Models.StudentModels;
namespace Assg.Models.TutorModels;

public class UserAccount
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; }

    public ICollection<ChatQuestion> ChatQuestions { get; set; }

}

