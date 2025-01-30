using System.ComponentModel.DataAnnotations;

namespace collegeAdmission.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
