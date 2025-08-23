using System.ComponentModel.DataAnnotations;

namespace Assg.Models.StudentModels
{
    public class EmailVM
    {
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsBodyHtml { get; set; }
    }
}
