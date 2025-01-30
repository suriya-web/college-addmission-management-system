using System.ComponentModel.DataAnnotations;

namespace collegeAdmission.Models
{
    public class LoginViewModel
    {
        internal readonly object? LastName;
        internal readonly object? FirstName;
        internal readonly object? DateOfBirth;
        internal readonly object? Gender;
        internal readonly object? EmailAddress;
        internal readonly object? Address;
        internal readonly object? City;
        internal readonly object? State;
        internal object? Contact;

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)] 
        public string Password { get; set; }
    }

}
